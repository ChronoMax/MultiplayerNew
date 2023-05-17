using UnityEngine;
using Unity.Netcode;
using TMPro;

public class PlayerNetwork : NetworkBehaviour
{
    [Header("Health")]
    public NetworkVariable<int> health = new NetworkVariable<int>();
    public TMP_Text healthText, timerText;
    public bool hit, dead;

    [SerializeField] GameObject playerComponents, gameplayUI, menuUI, mainCam;
    [SerializeField] Transform cameraTransform;
    [SerializeField] TextMeshProUGUI playerNameText;
    [SerializeField] float timeBetweenFire;

    [Header("VFX/SFX")]
    [SerializeField] ParticleSystem gunshot, blood;
    [SerializeField] AudioSource shotshound;
    
    float verticalLookRotation = 0.0f;
    float fireTimer;
    float moveSpeed = 5.0f;
    float lookSpeed = 2.0f;
    string playerName;

    public override void OnNetworkSpawn() //when the player spawns and the player is the owner
    {
        if (IsOwner)
        {
            playerNameText.text = playerName;
            health.Value = 100;
            mainCam.SetActive(false);
            playerComponents.SetActive(true);
            Screen.lockCursor = true;
        }
    }

    private void Update()
    {
        if (!IsOwner && IsServer) //the server can do the following code:
        {
            if (hit)
            {
                BloodVFXClientRpc();
                hit = false;
            }

            if (dead)
            {
                DespawnClientRpc();
                dead = false;
            }
        }

        if (!IsOwner) return; //from here the code will only run for his owner

        healthText.text = "Health: " + health.Value.ToString();
        timerText.text = "Reload: " + fireTimer.ToString("f1");
        
        /*
         * HANDELING PAUSE MENU
         */
        if (Input.GetKeyDown(KeyCode.Escape)) //handeling of the pause menu
        {
            Screen.lockCursor = false;
            if (gameplayUI.activeInHierarchy)
            {
                gameplayUI.SetActive(false);
                menuUI.SetActive(true);
            }
            else
            {
                gameplayUI.SetActive(true);
                menuUI.SetActive(false);
            }
        }
        else if (Input.GetMouseButtonDown(0) && Screen.lockCursor != true && !menuUI.activeInHierarchy)
        {
            Screen.lockCursor = true;
        }

        /*
         * HANDELING SHOOTING
         */
        if (Input.GetMouseButtonDown(0) && !menuUI.activeInHierarchy) //if the user is active in game the player can shoot a bullet
        {
            if (fireTimer <= 0)
            {
                ShootBulletServerRpc(10, cameraTransform.position, cameraTransform.forward);
                fireTimer = timeBetweenFire;
            }
        }

        if (fireTimer > 0) //checks if the timer
        {
            fireTimer -= Time.deltaTime;
        }

        /*
         * PLAYER MOVEMENT
         */
        // Get user input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        if (!menuUI.activeInHierarchy)
        {
            // Move the player
            Vector3 movement = transform.forward * vertical + transform.right * horizontal;
            movement.y = 0.0f;
            transform.position += movement.normalized * moveSpeed * Time.deltaTime;

            // Rotate the camera
            verticalLookRotation += mouseY * lookSpeed;
            verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90.0f, 90.0f);
            cameraTransform.localEulerAngles = new Vector3(-verticalLookRotation, 0.0f, 0.0f);
            transform.localEulerAngles = new Vector3(0.0f, transform.localEulerAngles.y + mouseX * lookSpeed, 0.0f);
        }
    }

    public void Respawn() //respawns the player (local) and sets the position
    {
        mainCam.SetActive(false);
        gameObject.transform.position = Vector3.zero;
        playerComponents.SetActive(true);

        RespawnServerRpc(100);
    }

    //All the messages from the server to the clients
    #region ClientRPC
    [ClientRpc]
    public void BloodVFXClientRpc() //plays the blood animation for every player
    {
        blood.Play();
    }

    [ClientRpc]
    public void ShotClientRpc() //plays the gunshot sound and VFX for every player
    {
        gunshot.Play();
        shotshound.Play();
    }

    [ClientRpc]
    public void RespawnClientRpc() //sets the gameobject (player component) active for all players
    {
        gameObject.SetActive(true);
    }

    [ClientRpc]
    public void DespawnClientRpc() //sets the main cam to active and unlocks the cursor, sets the player to deactive for every player
    {
        if (IsOwner)
        {
            mainCam.SetActive(true);
            Screen.lockCursor = false;
        }
        gameObject.SetActive(false);
    }
    #endregion

    //All the messages from clients to the server
    #region ServerRPC
    [ServerRpc]
    public void RespawnServerRpc(int value) //respawns the client for the server and resets the health for the player
    {
        RespawnClientRpc();

        Debug.Log("Setting health to 100 for player: " + OwnerClientId);
        PlayerNetwork player = gameObject.GetComponent<PlayerNetwork>();
        player.health.Value += value;
    }

    [ServerRpc]
    public void ShootBulletServerRpc(int damage, Vector3 pos, Vector3 dir) //shoots a bullet (raycast) (via server) and the player that gets hit gets the damage
    {
        ShotClientRpc();
        if (Physics.Raycast(pos, dir, out RaycastHit hit) && hit.transform.TryGetComponent(out PlayerNetwork player))
        {
            player.health.Value -= damage;
            player.hit = true;

            if (player.hit && player.health.Value == 0)
            {
                player.dead = true;
            }
        }
    }
    #endregion
}
