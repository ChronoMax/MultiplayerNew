using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
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
