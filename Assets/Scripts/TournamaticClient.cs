using UnityEngine;
using Networking = UnityEngine.Experimental.Networking;

using System.Collections;

using SimpleJSON;

public class TournamaticClient : MonoBehaviour
{
    const string base_url = "https://tournamatic.com";

    const string connectionErrorJson = "{\"status\" : \"error\", \"reason\" : \"connection_error\"}";

    public delegate void ClientCallback(JSONNode responseNode);

    private static TournamaticClient mInstance;
    static public TournamaticClient Instance
    {
        get
        {
            if (mInstance == null)
            {
                GameObject go = new GameObject();
                go.name = "TournamaticClient";
                mInstance = go.AddComponent<TournamaticClient>();
                DontDestroyOnLoad(go);
            }
            return mInstance;
        }
    }


    void publishResponse(Networking.UnityWebRequest request, ClientCallback callback)
    {
        if (request.isError)
        {
            JSONNode node = JSON.Parse(connectionErrorJson);

            callback(node);
        }
        else
        {
            JSONNode node = new JSONClass();
            if (request.downloadHandler.data.Length != 0)
                node = JSON.Parse(request.downloadHandler.text);

            if (request.responseCode == 200)
                node.Add("status", "ok");       // This is not correct if the result was array.
            else
                node.Add("status", "error");

            callback(node);
        }
    }

    public IEnumerator Register(string userName, string firstName, string lastName, string password, ClientCallback callback)
    {
        string rest_url = base_url + "/api/account/register";

        WWWForm form = new WWWForm();

        form.AddField("userName", userName);
        form.AddField("firstName", firstName);
        form.AddField("lastName", lastName);
        form.AddField("password", password);
        form.AddField("confirmPassword", password);

        Networking.UnityWebRequest request = Networking.UnityWebRequest.Post(rest_url, form);
        yield return request.Send();

        publishResponse(request, callback);
    }

    public IEnumerator Login(string userName, string password, ClientCallback callback)
    {
        string rest_url = base_url + "/token";

        WWWForm form = new WWWForm();

        form.AddField("grant_type", "password");
        form.AddField("userName", userName);
        form.AddField("password", password);

        Networking.UnityWebRequest request = Networking.UnityWebRequest.Post(rest_url, form);
        yield return request.Send();

        publishResponse(request, callback);
    }

    public IEnumerator Logout(string access_token, ClientCallback callback)
    {
        string rest_url = base_url + "/api/account/logout";

        Networking.UnityWebRequest request = Networking.UnityWebRequest.Post(rest_url, "logout");
        request.SetRequestHeader("Authorization", "Bearer " + access_token);

        yield return request.Send();

        publishResponse(request, callback);
    }

    public IEnumerator GetTournaments(ClientCallback callback)
    {
        string rest_url = base_url + "/api/tournaments";

        Networking.UnityWebRequest request = Networking.UnityWebRequest.Get(rest_url);
        yield return request.Send();

        publishResponse(request, callback);
    }

    public IEnumerator GetTournamentDraws(string tournamentId, ClientCallback callback)
    {
        string rest_url = base_url + "/api/tournaments/" + tournamentId + "/draws";

        Networking.UnityWebRequest request = Networking.UnityWebRequest.Get(rest_url);
        yield return request.Send();

        publishResponse(request, callback);
    }


}
