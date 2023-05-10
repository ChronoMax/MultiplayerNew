using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class PlayerMenuCanvas : MonoBehaviour
{
    public void DisconnectButtonPressed()
    {
        Debug.Log("Disconnect Button pressed!");
        GetComponentInParent<PlayerNetwork>().DespawnPlayerServerRPC();
        NetworkManager.Singleton.Shutdown(true);
        SceneManager.LoadScene("MainMenuTest");
    }

    public void DisconnectDeath()
    {
        NetworkManager.Singleton.Shutdown(true);
        SceneManager.LoadScene("MainMenuTest");
    }

    public void RespawnButtonpressed()
    {
        NetworkManager.Singleton.StartClient();
    }
}
