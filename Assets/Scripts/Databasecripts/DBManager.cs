using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBManager {

    public static string username;
    public static int score;

    public  bool logedIn { get { return username != null; } } 

    public static void LogOut()
    {
        username = null;
    }

}
