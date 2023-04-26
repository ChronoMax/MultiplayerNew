using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField]
    TMP_Text playerText;

    [SerializeField]
    GameObject login_registerCanvas;

    [SerializeField]
    Button loginButton, registerButton;

    [SerializeField]
    TMP_InputField username, password;

    [SerializeField]
    GameObject playCanvas;

    [SerializeField]
    Login loginscript;

    public bool loginCompleted = false;

    public void Start()
    {
        playCanvas.SetActive(false);
        playerText.text = "Player: " + DBManager.username;
    }

    public void PlayButtonPressed()
    {
        SceneManager.LoadScene("");
    }

    public void LoginButtonPressed()
    {
        StartCoroutine(login_request("login"));
    }

    public void RegisterButtonPressed()
    {
        StartCoroutine(register_request("register"));
    }

    IEnumerator login_request(string login_or_register)
    {
        while (!loginCompleted)
        {
            loginscript.CallDatabase(username.text, password.text, login_or_register);
            playCanvas.SetActive(true);
            login_registerCanvas.SetActive(false);

            yield return new WaitForSeconds(1);

            if (loginCompleted)
            {
                playerText.text = "Player: " + DBManager.username;
            }
        }
    }

    IEnumerator register_request(string login_or_register)
    {
        loginscript.CallDatabase(username.text, password.text, login_or_register);
        playCanvas.SetActive(true);
        login_registerCanvas.SetActive(false);
        yield return null;
    }

    void handleInteractable()
    {
        loginButton.interactable = false;
        registerButton.interactable = false;
    }
}
