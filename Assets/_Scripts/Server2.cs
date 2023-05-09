using UnityEngine;

public class Server2 : MonoBehaviour
{
    void Start()
    {
        Debug.Log("Game started.");
    }

    void Update()
    {
        Debug.Log("Updating game state...");
    }

    public static void Test()
    {
        Debug.Log("This is a test");
    }
}