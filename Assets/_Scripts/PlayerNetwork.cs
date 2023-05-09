using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using TMPro;

public class PlayerNetwork : NetworkBehaviour
{
    public GameObject playerComponents;

    public NetworkVariable<int> health = new NetworkVariable<int>();

    public float moveSpeed = 5.0f;
    public float lookSpeed = 2.0f;

    private float verticalLookRotation = 0.0f;
    public Transform cameraTransform;

    [SerializeField] string playerName;
    [SerializeField] TextMeshProUGUI playerNameText;

    public TMP_Text healthText;


    public override void OnNetworkSpawn()
    {
        Screen.lockCursor = true;

        playerNameText.text = playerName;
        health.Value = 100;
    }

    private void Update()
    {
        if (!IsOwner) return;

        playerComponents.SetActive(true);

        healthText.text = health.Value.ToString();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Screen.lockCursor = false;
        }
        else if (Input.GetMouseButtonDown(0) && Screen.lockCursor != true)
        {
            Screen.lockCursor = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            ShootBulletServerRpc(10, cameraTransform.position, cameraTransform.forward);
        }

        if (health.Value == 0)
        {
            DespawnPlayerServerRPC();
        }

        // Get user input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

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

    [ServerRpc]
    public void ShootBulletServerRpc(int damage, Vector3 pos, Vector3 dir)
    {
        print("Raycast send");
        if (Physics.Raycast(pos, dir, out RaycastHit hit) && hit.transform.TryGetComponent(out PlayerNetwork player))
        {
            print("Damage done");
            player.health.Value -= damage;
        }
    }

    [ServerRpc]
    public void DespawnPlayerServerRPC()
    {
        this.gameObject.GetComponent<NetworkObject>().Despawn();
    }

    [ClientRpc] //are to be runt from server to clients
    public void RaycastClientRPC()
    {

    }
}
