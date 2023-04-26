using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DatabaseHandler : MonoBehaviour
{
    [SerializeField]
    string login_databaseURL, register_databaseURL;
    public MainMenuHandler MainMenuHandlerscript;

    //Links to the other scripts.
    public void CallDatabase(string username, string password, string login_or_register)
    {
        StartCoroutine(LoginRegisterPlayer(username, password, login_or_register));
    }

    /*
     * When the tag 'login' or 'register' is given the methode will create
     * a webrequest and sends the username and password to get used in the php script.
     */
    IEnumerator LoginRegisterPlayer(string username, string password, string login_or_register)
    {
        string database = "";
        switch (login_or_register)
        {
            case "login":
                database = login_databaseURL;
            break;
            case "register":
                database = register_databaseURL;
                break;
            default:
                break;
        }

        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);
        WWW www = new WWW(database, form);
        yield return www;

        if (database == login_databaseURL)
        {
            if (www.text[0] == '0')
            {
                DBManager.username = username;
                DBManager.score = int.Parse(www.text.Split('\t')[1]);
                MainMenuHandlerscript.requestCompleted = true;
            }
            else
            {
                Debug.Log("Login failed. Error: " + www.text);
            }
        }
        else if (database == register_databaseURL)
        {
            if (www.text == "")
            {
                Debug.Log("User created succesfully.");
                MainMenuHandlerscript.requestCompleted = true;
            }
            else
            {
                Debug.Log(www.text);
                Debug.Log("User cannot be created. Error: " + www.text);
            }
        }
    }
}
