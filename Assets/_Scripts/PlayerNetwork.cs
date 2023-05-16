using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using TMPro;
using UnityEngine.VFX;
using UnityEngine.SceneManagement;

public class PlayerNetwork : NetworkBehaviour
{
    [Header("Health")]
    public NetworkVariable<int> health = new NetworkVariable<int>();
    public TMP_Text healthText, timerText;
    public bool hit, dead = false;

    [SerializeField] GameObject playerComponents, gameplayUI, menuUI;

    private float moveSpeed = 5.0f;
    private float lookSpeed = 2.0f;
    private float verticalLookRotation = 0.0f;

    [SerializeField] Transform cameraTransform;

    [Header("Playername")]
    [SerializeField] string playerName;
    [SerializeField] TextMeshProUGUI playerNameText;

    [SerializeField] float timeBetweenFire;
    float firetimer;

    [Header("VFX/SFX")]
    public AudioSource gunAudio;
    public ParticleSystem gunshot, blood;

    public GameObject maincam;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            playerNameText.text = playerName;
            health.Value = 100;
            maincam.SetActive(false);
            playerComponents.SetActive(true);

            Screen.lockCursor = true;
        }
    }

    private void Update()
    {
        if (!IsOwner && IsServer)
        {
            if (hit)
            {
                BloodVFXClientRpc();
                hit = false;
            }

            if (dead)
            {
                despawnclientrpc();
                dead = false;
            }

        }

        if (!IsOwner) return;

        //if (health.Value == 0)
        //{
        //    maincam.SetActive(true);
        //    Screen.lockCursor = false;
        //}

        healthText.text = "Health: " + health.Value.ToString();
        timerText.text = "Reload: " + firetimer.ToString("f1");

        if (Input.GetKeyDown(KeyCode.Escape))
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

        if (Input.GetMouseButtonDown(0) && !menuUI.activeInHierarchy)
        {
            if (firetimer <= 0)
            {
                ShootBulletServerRpc(10, cameraTransform.position, cameraTransform.forward);
                firetimer = timeBetweenFire;
            }
        }

        if (firetimer > 0)
        {
            firetimer -= Time.deltaTime;
        }

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

    public void Respawn()
    {
        maincam.SetActive(false);
        gameObject.transform.position = new Vector3(0, 0, 0);
        playerComponents.SetActive(true);
        respawnServerRpc(100);
        //RespawnClientRpc();
    }

    [ServerRpc]
    public void ShootBulletServerRpc(int damage, Vector3 pos, Vector3 dir)
    {
        print("Raycast send");
        ShotClientRpc();
        if (Physics.Raycast(pos, dir, out RaycastHit hit) && hit.transform.TryGetComponent(out PlayerNetwork player))
        {
            print("Damage done");
            player.health.Value -= damage;
            player.hit = true;

            if (player.hit && player.health.Value == 0)
            {
                player.dead = true;
            }
        }
    }

    [ClientRpc]
    public void BloodVFXClientRpc()
    {
        blood.Play();
    }

    [ClientRpc]
    public void ShotClientRpc()
    {
        gunshot.Play();
        gunAudio.Play();
    }

    [ClientRpc]
    public void RespawnClientRpc()
    {
        gameObject.SetActive(true);
    }

    [ServerRpc]
    public void respawnServerRpc(int value)
    {
        RespawnClientRpc();
        Debug.Log("Setting health to 100");
        PlayerNetwork player = gameObject.GetComponent<PlayerNetwork>();
        player.health.Value += value;
    }

    [ClientRpc]
    public void despawnclientrpc()
    {
        if (IsOwner)
        {
            maincam.SetActive(true);
            Screen.lockCursor = false;
        }
        gameObject.SetActive(false);
    }
}
