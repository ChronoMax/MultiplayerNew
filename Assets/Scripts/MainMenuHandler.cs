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

    DatabaseHandler databaseHandler;
    public bool requestCompleted = false;

    /*
     * At start, find the database-script.
     * set the play button on inactive.
     * set the text.
     */
    public void Start()
    {
        databaseHandler = GameObject.Find("Canvas").GetComponent<DatabaseHandler>();
        playCanvas.SetActive(false);
        playerText.text = "Player: " + DBManager.username;
    }

    //Play button behavior.
    public void PlayButtonPressed()
    {
        SceneManager.LoadScene("");
    }

    //loginbutton behavior with tag 'login'
    public void LoginButtonPressed()
    {
        StartCoroutine(database_request("login"));
    }

    //registerbutton behavior with tag 'register'
    public void RegisterButtonPressed()
    {
        StartCoroutine(database_request("register"));
    }

    /* 
     * using www for webrequest.
     * sends the name and password for login or registering.
     * returns the player name when logged-in.
     * sets the play button to active.
     * turns-of the login-registration form.
     */
    IEnumerator database_request(string login_or_register)
    {
        while (!requestCompleted)
        {
            databaseHandler.CallDatabase(username.text, password.text, login_or_register);

            yield return new WaitForSeconds(1);

            if (requestCompleted && login_or_register != "register")
            {
                playerText.text = "Player: " + DBManager.username;
                playCanvas.SetActive(true);
                login_registerCanvas.SetActive(false);
            }
        }
    }
}
