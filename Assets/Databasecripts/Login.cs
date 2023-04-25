using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    [SerializeField]
    TMP_InputField username, password;

    [SerializeField]
    Button LoginButton;

    [SerializeField]
    string databaseURL;

    public void CallLogin()
    {
        StartCoroutine(LoginPlayer());
    }

    IEnumerator LoginPlayer()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username.text);
        form.AddField("password", password.text);
        WWW www = new WWW(databaseURL, form);
        yield return www;

        if (www.text[0] == '0')
        {
            DBManager.username = username.text;
            DBManager.score = int.Parse(www.text.Split('\t')[1]);
            SceneManager.LoadScene(0);
        }
        else
        {
            Debug.Log("Login failed. Error: " + www.text);
        }
    }

    public void LoginButtonClicked()
    {
       LoginButton.interactable = (username.text.Length >= 8 && password.text.Length >= 8);
    }

    public void BackButtonclicked()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
