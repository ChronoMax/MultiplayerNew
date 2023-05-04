using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BulletSpawner : NetworkBehaviour
{
    [SerializeField]
    GameObject bullet;

    [SerializeField]
    Transform initialTransform;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && IsOwner)
        {
            SpawnBulletServerRPC(initialTransform.position, initialTransform.rotation);
        }
        else if (!IsOwner)
        {
            return;
        }
    }

    [ServerRpc]
    private void SpawnBulletServerRPC(Vector3 position, Quaternion rotation)
    {
        GameObject instantiatedBullet =  Instantiate(bullet, position, rotation);
        instantiatedBullet.GetComponent<NetworkObject>().Spawn();
    }
}
