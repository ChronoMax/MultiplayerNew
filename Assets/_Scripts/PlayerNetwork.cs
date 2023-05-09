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

    [SerializeField]  float rayDistance = 100f;

    public TMP_Text healthText;

    public struct myCustomData : INetworkSerializable
        {
        public int _int;
        public bool _bool;
        public FixedString128Bytes message;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _int);
            serializer.SerializeValue(ref _bool);
            serializer.SerializeValue(ref message);
        }
    }

    private NetworkVariable<myCustomData> randomNumber = new NetworkVariable<myCustomData>(
    new myCustomData {
        _int = 63,
        _bool = true,
    }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        playerNameText.text = playerName;
        health.Value = 100;
        healthText.text = health.Value.ToString();

        randomNumber.OnValueChanged += (myCustomData previousValue, myCustomData newValue) =>
        {
            Debug.Log(OwnerClientId + ": " + newValue._int + newValue._bool + newValue.message);
        };
    }

    private void Update()
    {
        if (!IsOwner) return;

        playerComponents.SetActive(true);

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

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        if (other.GetComponent<Bullet>() && health.Value != 0)
        {
            health.Value = health.Value - 10;
            healthText.text = health.Value.ToString();
        }
        else if (health.Value == 0)
        {
            DespawnPlayerServerRPC();
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
