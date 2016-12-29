using UnityEngine;
using System.Collections;

using SimpleJSON;

public class Test : MonoBehaviour {

    string userName = "hojjatjafary@gmail.com";
    string password = "hjjpass";
    string access_token;

	// Use this for initialization
	void Start ()
    {
        StartCoroutine(TournamaticClient.Instance.Register(userName, "Hojjat", "Jafary", password, RegisterResult));

        StartCoroutine(TournamaticClient.Instance.GetTournamentDraws("66", GetTournamentsResult));

    }

    void GetTournamentsResult(JSONNode responseNode)
    {
        if (responseNode["status"] == "ok")
            Debug.Log("GetTournamentsResult: " + responseNode.ToString());
        else
            Debug.Log("Getting Tournaments error: " + responseNode.ToString());
    }

    void RegisterResult(JSONNode responseNode)
    {
        Debug.Log("RegisterResult: " + responseNode.ToString());
        if (responseNode["status"] == "ok")
        {
            Debug.Log(responseNode["firstName"]);
            Debug.Log(responseNode["lastName"]);
        }

        StartCoroutine(TournamaticClient.Instance.Login(userName, password, LoginResult));
    }

    void LoginResult(JSONNode responseNode)
    {
        Debug.Log("LoginResult: " + responseNode.ToString());

        access_token = responseNode["access_token"];

        StartCoroutine(TournamaticClient.Instance.Logout(access_token, LogoutResult));
    }

    void LogoutResult(JSONNode responseNode)
    {
        Debug.Log("LogoutResult: " + responseNode.ToString());
    }
}
