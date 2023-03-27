using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public TMP_Text playerText;
    public void Start()
    {
        playerText.text = "Player: " + DBManager.username;
    }

    public void LoadSceneRegister()
    {
        SceneManager.LoadScene(1);
    }
    public void LoadSceneLogin()
    {
        SceneManager.LoadScene(2);
    }
    public void LoadScenePlayGame()
    {
        SceneManager.LoadScene(3);
    }
}
