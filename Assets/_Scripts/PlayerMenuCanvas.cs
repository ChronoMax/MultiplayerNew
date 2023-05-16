using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class PlayerMenuCanvas : MonoBehaviour
{
    public GameObject player, cam;

    public void DisconnectButtonPressed()
    {
        Debug.Log("Disconnect Button pressed!");
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("MainMenu_Pr");
    }

    public void DisconnectDeath()
    {
        SceneManager.LoadScene("MainMenu_Pr");
    }

    public void RespawnButtonpressed()
    {
        //NetworkManager.Singleton.StartClient();
        player.GetComponent<PlayerNetwork>().Respawn();
    }
}
