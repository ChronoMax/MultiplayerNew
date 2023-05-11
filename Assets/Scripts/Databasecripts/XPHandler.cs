using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class XPHandler : MonoBehaviour
{
    [SerializeField]
    TMP_Text playerText;

    [SerializeField]
    TMP_Text scoreText;

    [SerializeField]
    string databaseURL;

    private void Awake()
    {
        if (DBManager.username == null)
        {
            SceneManager.LoadScene("MainMenu");
        }

        playerText.text = "Player: " + DBManager.username;
        scoreText.text = "Score: " + DBManager.score;
    }

    public void SaveButtonPressed()
    {
        StartCoroutine(SavePlayerData());
    }

    IEnumerator SavePlayerData()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", DBManager.username);
        form.AddField("score", DBManager.score);

        WWW www = new WWW(databaseURL, form);
        yield return www;

        if (www.text == "0")
        {
            Debug.Log("Game Saved!");
        }
        else
        {
            Debug.Log("Saving failed. Error: " + www.text);
        }
    }

    public void IncreaseLevel()
    {
        DBManager.score++;
        scoreText.text = "Score: " + DBManager.score;
    }
}
