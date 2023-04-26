using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
public class RegisterForm : MonoBehaviour
{
    [SerializeField]
    public TMP_InputField username, password;

    [SerializeField]
    public Button registerButton;

    [SerializeField]
    string databaseURL;

    public void CallRegister()
    {
        StartCoroutine(Register());
    }

    IEnumerator Register()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username.text);
        form.AddField("password", password.text);
        WWW www = new WWW(databaseURL, form);
        yield return www;

        if (www.text == "")
        {
            Debug.Log("User created succesfully.");
            SceneManager.LoadScene(0);
        }
        else
        {
            Debug.Log(www.text);
            Debug.Log("User cannot be created. Error: " + www.text);
        }
    }

    public void RegisterButtonClicked()
    {
        registerButton.interactable = (username.text.Length >= 8 && password.text.Length >= 8);
    }

    public void BackButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
