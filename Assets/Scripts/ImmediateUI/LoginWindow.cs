
using UnityEngine;
using SimpleJSON;

public class LoginWindow : WindowBase<LoginWindow>
{
    string userName;
    string password;

    enum State
    {
        Normal,
        Loading,
        Error
    }
    State mCurrentState;

    void Awake()
    {
        Instance = this;

        // Set window properties.
        wndWidth = 800;
        wndHeight = 400;
        windowTitle = "Sign in";
        windowFun = window;
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        userName = PlayerPrefs.GetString("user_name", "");
        password = PlayerPrefs.GetString("password", "");
    }


    public override void Show()
    {
        base.Show();

        mCurrentState = State.Normal;
    }

    protected override void OnGUI()
    {
        GUI.skin.textField.fontSize = 28;
        GUI.skin.textField.alignment = TextAnchor.MiddleLeft;
        GUI.skin.window.fontSize = 18;
        GUI.skin.label.fontSize = 20;
        base.OnGUI();
    }

    void window(int id)
    {
        if (mCurrentState == State.Loading)
        {
            GUI.skin.label.fontSize = 25;
            GUILayout.Label(loadingMessage, GUILayout.Height(100));
            GUI.skin.label.fontSize = 15;
        }
        else if (mCurrentState == State.Error)
        {
            GUI.skin.label.fontSize = 20;
            GUILayout.Label(errorMessage, GUILayout.Height(wndHeight));
            GUI.skin.label.fontSize = 15;

            if (GUILayout.Button("Back"))
            {
                mCurrentState = State.Normal;
            }

        }
        else if (mCurrentState == State.Normal)
        {
            showLoginScreen();
        }
    }

    void showLoginScreen()
    {
        GUILayout.Space(45);
        GUILayout.BeginVertical();

        GUILayout.Label("Email:");
        userName = GUILayout.TextField(userName, 100, GUILayout.Height(50));
        GUILayout.Space(10);
        GUILayout.Label("Password:");
        password = GUILayout.PasswordField(password, "*"[0], 100, GUILayout.Height(50));

        GUILayout.Space(20);
        if (GUILayout.Button("Login", GUILayout.Height(60)))
        {
            doLogin();
        }

        GUILayout.Space(16);

        if (GUILayout.Button("Back", GUILayout.Height(60)))
        {
            doCancel();
        }

        GUILayout.Space(20);

        if (!string.IsNullOrEmpty(errorMessage))
        {
            Color origColor = GUI.contentColor;
            GUI.contentColor = Color.red;
            GUILayout.Label(errorMessage);
            GUI.contentColor = origColor;
        }

        GUILayout.EndVertical();
    }

    void doCancel()
    {
        this.Hide();
        SigninWindow.Instance.Show();
    }

    void doLogin()
    {
        PlayerPrefs.SetString("user_name", userName);
        PlayerPrefs.SetString("password", password);

        mCurrentState = State.Loading;
        loadingMessage = "Logging in...";

        TournamaticClient.Instance.Login(userName, password, onLoginResult);

        SessionData.Instance.userName = userName;
        SessionData.Instance.password = password;
    }

    void onLoginResult(JSONNode responseNode)
    {
        if (responseNode["status"].Value == "error")
        {
            errorMessage = "Login failed!" + "\n" + responseNode.ToString();
            mCurrentState = State.Error;
        }
        else
        {
            Debug.Log("Login success.");
            Debug.Log(responseNode.ToString());
            loadingMessage = "Logged in, Getting user info...";

            JSONNode data = responseNode["data"];

            SessionData.Instance.access_token = data["access_token"].Value;
            SessionData.Instance.firstName = data["firstName"].Value;
            SessionData.Instance.lastName = data["lastName"].Value;

            TournamaticClient.Instance.GetUserInfo(SessionData.Instance.access_token, onUserInfoResult);
        }
    }

    void onUserInfoResult(JSONNode responseNode)
    {
        if (responseNode["status"].Value == "error")
        {
            errorMessage = "Getting user info failed!" + "\n" + responseNode.ToString();
            mCurrentState = State.Error;
        }
        else
        {
            Debug.Log("UserInfo success.");
            Debug.Log(responseNode.ToString());

            JSONNode data = responseNode["data"];

            SessionData.Instance.userId = data["userId"].Value;

            EnterTournament();
        }
    }

    void EnterTournament()
    {
        JSONArray membersArray = new JSONArray();

        JSONNode member = new JSONClass();
        member["userId"] = SessionData.Instance.userId;
        member["name"] = SessionData.Instance.firstName;
        member["lastName"] = SessionData.Instance.lastName;
        member["email"] = SessionData.Instance.userName;
        member["birthDate"] = "2017-02-01T08:00:00.000Z";

        membersArray.Add(member);

        JSONClass team = new JSONClass();
        team["name"] = SessionData.Instance.firstName + " " + SessionData.Instance.lastName;
        team["ownerId"] = SessionData.Instance.userId;
        team["members"] = membersArray;
        team["uniforms"] = new JSONArray();
        team["teamOfficials"] = new JSONArray();

        Debug.Log(team.ToString());

        mCurrentState = State.Loading;
        loadingMessage = "Entering Tournament...";
        TournamaticClient.Instance.EnterDivision(SessionData.Instance.access_token, SessionData.Instance.tournamentId, SessionData.Instance.drawId, team.ToString(), enterResult);
    }

    void enterResult(JSONNode responseNode)
    {
        if (responseNode["status"].Value != "ok")
        {
            mCurrentState = State.Error;
            errorMessage = "Failed to load tournament info." + "\n" + responseNode.ToString();
            return;
        }

        //Debug.Log(responseNode.ToString());

        SessionData.Instance.teamId = responseNode["data"]["teamId"].AsInt;

        mCurrentState = State.Loading;
        loadingMessage = "Loading Game...";
        //UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(1);
    }

    void OnLevelWasLoaded(int level)
    {
        if (level == 1)
            Hide();
    }
}
