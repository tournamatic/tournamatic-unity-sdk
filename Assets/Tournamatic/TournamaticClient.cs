using UnityEngine;

// Older Unity versions.
//using Networking = UnityEngine.Experimental.Networking;
using Networking = UnityEngine.Networking;

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
            JSONNode dataNode = null;
            if (request.downloadHandler.data.Length != 0)
                dataNode = JSON.Parse(request.downloadHandler.text);

            JSONNode objectNode = new JSONClass();
            objectNode.Add("data", dataNode);

            if (request.responseCode == 200)
                objectNode.Add("status", "ok");       // This is not correct if the result was array.
            else
            {
                objectNode.Add("status", "error");
                objectNode.Add("code", request.responseCode.ToString());
            }

            if (callback != null)
                callback(objectNode);
        }
    }

    private IEnumerator Post(string rest_url, WWWForm form, ClientCallback callback)
    {
        Networking.UnityWebRequest request = Networking.UnityWebRequest.Post(rest_url, form);

        yield return Post(request, callback);
    }

    private IEnumerator Post(Networking.UnityWebRequest request, ClientCallback callback)
    {
        yield return request.Send();

        publishResponse(request, callback);
    }

    private IEnumerator Get(string rest_url, ClientCallback callback)
    {
        Networking.UnityWebRequest request = Networking.UnityWebRequest.Get(rest_url);

        yield return Get(request, callback);
    }

    private IEnumerator Get(Networking.UnityWebRequest request, ClientCallback callback)
    {
        yield return request.Send();

        publishResponse(request, callback);
    }

    private IEnumerator Put(string rest_url, string payload, ClientCallback callback)
    {
        byte[] data = System.Text.Encoding.UTF8.GetBytes(payload);
        Networking.UnityWebRequest request = Networking.UnityWebRequest.Put(rest_url, data);

        yield return Put(request, callback);
    }

    private IEnumerator Put(Networking.UnityWebRequest request, ClientCallback callback)
    {
        yield return request.Send();

        publishResponse(request, callback);
    }

    private Networking.UnityWebRequest CreatePostJson(string url, string jsonData)
    {
        var request = new Networking.UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new Networking.UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new Networking.DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");

        return request;
    }

    public void Register(string userName, string firstName, string lastName, string password, ClientCallback callback)
    {
        string rest_url = base_url + "/api/account/register";

        WWWForm form = new WWWForm();

        form.AddField("userName", userName);
        form.AddField("firstName", firstName);
        form.AddField("lastName", lastName);
        form.AddField("password", password);
        form.AddField("confirmPassword", password);

        StartCoroutine(Post(rest_url, form, callback));
    }

    public void Login(string userName, string password, ClientCallback callback)
    {
        string rest_url = base_url + "/token";

        WWWForm form = new WWWForm();

        form.AddField("grant_type", "password");
        form.AddField("userName", userName);
        form.AddField("password", password);

        StartCoroutine(Post(rest_url, form, callback));
    }

    public void Logout(string access_token, ClientCallback callback)
    {
        string rest_url = base_url + "/api/account/logout";

        WWWForm form = new WWWForm();
        form.AddField("nothing", "nothing");

        Networking.UnityWebRequest request = Networking.UnityWebRequest.Post(rest_url, form);
        request.SetRequestHeader("Authorization", "Bearer " + access_token);

        StartCoroutine(Post(request, callback));
    }

    public void GetTournamentsInCategory(int categoryId, ClientCallback callback)
    {
        string rest_url = base_url + "/api/categories/" + categoryId + "/tournaments";

        StartCoroutine(Get(rest_url, callback));
    }

    public void GetAllTournaments(ClientCallback callback)
    {
        string rest_url = base_url + "/api/tournaments";

        StartCoroutine(Get(rest_url, callback));
    }

    public void GetTournament(int tournamentId, ClientCallback callback)
    {
        string rest_url = base_url + "/api/tournaments/" + tournamentId;

        StartCoroutine(Get(rest_url, callback));
    }

    public void EnterDivision(string access_token, int tournamentId, int drawId, string data, ClientCallback callback)
    {
        string rest_url = base_url + "/api/tournaments/" + tournamentId + "/draws/" + drawId + "/teams";

        var request = CreatePostJson(rest_url, data);
        
        request.SetRequestHeader("Authorization", "Bearer " + access_token);

        StartCoroutine(Post(request, callback));

    }

    public void GetUserInfo(string access_token, ClientCallback callback)
    {
        string rest_url = base_url + "/api/Account/UserInfo";

        Networking.UnityWebRequest request = Networking.UnityWebRequest.Get(rest_url);
        request.SetRequestHeader("Authorization", "Bearer " + access_token);

        StartCoroutine(Get(request, callback));

    }

    public void GetTeams(int tournamentId, int drawId, ClientCallback callback)
    {
        string rest_url = base_url + "/api/tournaments/" + tournamentId + "/draws/" + drawId + "/teams";

        StartCoroutine(Get(rest_url, callback));
    }

    public void GetDraws(int drawId, ClientCallback callback)
    {
        string rest_url = base_url + "/api/tournaments/" + drawId + "/draws";

        StartCoroutine(Get(rest_url, callback));
    }

    public void GetMatches(int tournamentId, int drawId, int roundId, int poolId, ClientCallback callback)
    {
        string rest_url = base_url + "/api/tournaments/" + tournamentId + "/draws/" + drawId + "/matches/" + roundId + "/" + poolId;

        StartCoroutine(Get(rest_url, callback));
    }

    public void EnterMatch(string access_token, int tournamentId, int drawId, ClientCallback callback)
    {
        string rest_url = base_url + "/api/tournaments/" + tournamentId + "/draws/" + drawId + "/teams";

        WWWForm form = new WWWForm();
        form.AddField("nothing", "nothing");

        Networking.UnityWebRequest request = Networking.UnityWebRequest.Post(rest_url, form);
        request.SetRequestHeader("Authorization", "Bearer " + access_token);

        StartCoroutine(Post(request, callback));
    }

    public void GetRounds(int tournamentId, int drawId, ClientCallback callback)
    {
        string rest_url = base_url + "/api/tournaments/" + tournamentId + "/draws/" + drawId + "/rounds";

        StartCoroutine(Get(rest_url, callback));
    }

    public void GetMatches(int tournamentId, int drawId, int roundId, ClientCallback callback)
    {
        string rest_url = base_url + "/api/tournaments/" + tournamentId + "/draws/" + drawId + "/matches/" + roundId;

        StartCoroutine(Get(rest_url, callback));
    }

    public void RecordScore(string access_token, int tournamentId, int drawId, string data, ClientCallback callback)
    {
        string rest_url = base_url + "/api/tournaments/" + tournamentId + "/draws/" + drawId + "/matches";

        byte[] payload = System.Text.Encoding.UTF8.GetBytes(data);
        Networking.UnityWebRequest request = Networking.UnityWebRequest.Put(rest_url, payload);
        request.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");
        request.SetRequestHeader("Authorization", "Bearer " + access_token);

        StartCoroutine(Put(request, callback));
    }
    
    public void GetLeaderboard(int tournamentId, int drawId, int roundId, int poolId, ClientCallback callback)
    {
        string rest_url = base_url + "/api/tournaments/" + tournamentId + "/draws/" + drawId + "/" + roundId + "/" + poolId + "/scoreboard";

        StartCoroutine(Get(rest_url, callback));
    }

}
