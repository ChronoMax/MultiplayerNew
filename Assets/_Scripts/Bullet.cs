using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Bullet : NetworkBehaviour
{
    [SerializeField]
    private float speed = 200f;

    [SerializeField]
    private int despawnAfter = 10;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        GetComponent<Rigidbody>().velocity = this.transform.forward * speed;
    }

    private void Start()
    {
        if (!IsServer)

        StartCoroutine(Despawn());
    }

    private IEnumerator Despawn()
    {
        yield return new WaitForSeconds(despawnAfter);
        DespawnBulletServerRpc();
    }

    [ServerRpc]
    void DespawnBulletServerRpc()
    {
        this.gameObject.GetComponent<NetworkObject>().Despawn();
    }
}
