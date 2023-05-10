using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class PlayerMenuCanvas : MonoBehaviour
{
    public void DisconnectButtonPressed()
    {
        NetworkManager.Singleton.Shutdown(true);
        SceneManager.LoadScene("MainMenuTest");
    }
}
