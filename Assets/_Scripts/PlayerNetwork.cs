using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] Transform objectToSpawn;
    [SerializeField] Transform spawnedObjectTransform;

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
        randomNumber.OnValueChanged += (myCustomData previousValue, myCustomData newValue) =>
        {
            Debug.Log(OwnerClientId + ": " + newValue._int + newValue._bool + newValue.message);
        };
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.T))
        {
            spawnedObjectTransform = Instantiate(objectToSpawn);
            spawnedObjectTransform.GetComponent<NetworkObject>().Spawn(true);
           //TestServerRPC();
            /*
            randomNumber.Value = new myCustomData
            {
                _int = 10,
                _bool = true,
                message = "Hello",
            };
            */
        }

        Vector3 moveDir = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.W))
        {
            moveDir.z = +1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDir.z = -1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDir.x = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDir.x = +1f;
        }

        float moveSpeed = 3f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Y))
        {
            spawnedObjectTransform.GetComponent<NetworkObject>().Despawn(true);
            Destroy(spawnedObjectTransform.gameObject);
        }
    }

    [ServerRpc]
    public void TestServerRPC()
    {
        Debug.Log(OwnerClientId +  " Server RPC");
    }

    [ClientRpc] //are to be runt from server to clients
    public void ClientRPC()
    {
        Debug.Log(OwnerClientId + " CLient RPC");
    }
}
