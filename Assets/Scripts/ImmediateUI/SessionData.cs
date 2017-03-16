using UnityEngine;
using System.Collections;

public class SessionData : MonoBehaviour 
{
    public static SessionData Instance;

    public string access_token;
    public int tournamentCategory = 24;
    public int tournamentId;
    public int drawId;
    

    public string userName;
    public string firstName;
    public string lastName;
    public string password;
    public string userId;
    public int teamId;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
