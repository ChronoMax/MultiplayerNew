using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.Collections;

public class Server : MonoBehaviour
{
    private void Start()
    {
        // Set the application target frame rate to unlimited
        Application.targetFrameRate = -1;

        // Start the server in headless mode
        var args = System.Environment.GetCommandLineArgs();
        bool isHeadless = false;
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-batchmode")
            {
                isHeadless = true;
                break;
            }
        }

        if (isHeadless)
        {
            QualitySettings.vSyncCount = 0;
            Application.runInBackground = true;
            NetworkManager.Singleton.StartServer();
            print("Help");
        }
        else
        {
            Debug.LogError("This script should only be run in headless mode using the -batchmode flag!");
        }
    }
}
