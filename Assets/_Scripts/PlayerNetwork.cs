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
    public bool hit = false;

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

    public override void OnNetworkSpawn()
    {
        Screen.lockCursor = true;

        playerNameText.text = playerName;
        health.Value = 100;

        menuUI.SetActive(false);
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
        }

        if (!IsOwner) return;

        playerComponents.SetActive(true);

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

        if (health.Value == 0)
        {
            Screen.lockCursor = false;
            DespawnPlayerServerRPC();
            NetworkManager.Singleton.Shutdown();
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

    [ServerRpc]
    public void DespawnPlayerServerRPC()
    {
        Debug.Log("Despawning player:" + OwnerClientId);
        this.gameObject.GetComponent<NetworkObject>().Despawn();
    }
}
