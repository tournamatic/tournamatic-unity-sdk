
using UnityEngine;
using SimpleJSON;

public class RegistrationWindow : WindowBase<RegistrationWindow>
{
    string firstName = "";
    string lastName = "";
    string userName = "";
    string password = "";

    bool registrationSuccess;

    Color linkColor = new Color(0.05f, 0.89f, 0.67f, 1);

    enum State
    {
        Normal,
        Loading,
        RegisterCompleted,
        Error
    }
    State mCurrentState;

    void Awake()
    {
        Instance = this;

        // Set window properties.
        wndWidth = 800;
        wndHeight = 650;
        windowTitle = "Registration";
    }

    // Use this for initialization
    protected override void Start()
    {
        userName = PlayerPrefs.GetString("user_name", "");
        firstName = PlayerPrefs.GetString("first_name", "");
        lastName = PlayerPrefs.GetString("last_name", "");
        password = PlayerPrefs.GetString("password", "");

        base.Start();
    }

    protected override void OnGUI()
    {
        GUI.skin.textField.fontSize = 28;
        GUI.skin.textField.alignment = TextAnchor.MiddleLeft;
        GUI.skin.window.fontSize = 18;
        GUI.skin.label.fontSize = 20;

        if (mCurrentState == State.RegisterCompleted)
        {
            windowFun = successWnd;
        }
        else if (mCurrentState == State.Normal)
        {
            windowFun = registrationWindow;
        }
        else
        {
            windowFun = messageWindow;
        }
        
        base.OnGUI();
    }

    void messageWindow(int id)
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
    }

    void registrationWindow(int id)
    {
        GUILayout.Space(35);
        GUILayout.BeginVertical();

        GUILayout.Label("First Name:");
        firstName = GUILayout.TextField(firstName, 100, GUILayout.Height(50));

        GUILayout.Label("Last Name:");
        lastName = GUILayout.TextField(lastName, 100, GUILayout.Height(50));

        GUILayout.Label("Email:");
        userName = GUILayout.TextField(userName, 100, GUILayout.Height(50));

        GUILayout.Label("Password:");
        password = GUILayout.PasswordField(password, "*"[0], 100, GUILayout.Height(50));

        GUILayout.Space(20);
        if (GUILayout.Button("Register", GUILayout.Height(60)))
        {
            doRegister();
        }
        GUILayout.Space(15);
        if (GUILayout.Button("Back", GUILayout.Height(60)))
        {
            goBack();
        }

        TermsOfService();

        GUILayout.EndVertical();
    }

    void TermsOfService()
    {
        TextAnchor origTextAnchor = GUI.skin.label.alignment;
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        GUILayout.Label("By signing up, I agree to Tournamatic.com's ");

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        Color origColor = GUI.color;
        GUI.color = linkColor;
        if (GUILayout.Button("Terms of Service", GUI.skin.label))
        {
            Application.OpenURL("https://tournamatic.com/#!/terms");
        }
        GUI.color = origColor;
        GUILayout.Label(" and ");
        GUI.color = linkColor;
        if (GUILayout.Button("Privacy Policy", GUI.skin.label))
        {
            Application.OpenURL("https://tournamatic.com/#!/privacy");
        }
        GUI.color = origColor;
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUI.skin.label.alignment = origTextAnchor;
    }

    void goBack()
    {
        this.Hide();
        SigninWindow.Instance.Show();
    }

    void doRegister()
    {
        PlayerPrefs.SetString("user_name", userName);
        PlayerPrefs.SetString("first_name", firstName);
        PlayerPrefs.SetString("last_name", lastName);
        PlayerPrefs.SetString("password", password);

        mCurrentState = State.Loading;
        loadingMessage = "Registering...";
        TournamaticClient.Instance.Register(userName, firstName, lastName, password, onRegisterResult);
    }

    void onRegisterResult(JSONNode responseNode)
    {
        if (responseNode["status"] == "error")
        {
            errorMessage = "Registration failed!";
            mCurrentState = State.Error;
        }
        else
        {
            mCurrentState = State.RegisterCompleted;
            Debug.Log("Registration success.");
        }
    }

    void successWnd(int id)
    {
        GUILayout.FlexibleSpace();

        int origSize = GUI.skin.label.fontSize;
        GUI.skin.label.fontSize = 30;

        TextAnchor origTextAnchor = GUI.skin.label.alignment;
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;

        GUILayout.Label("Registration Completed");
        GUILayout.Label("Please check your email and then try login.", GUILayout.Height(60));

        GUI.skin.label.fontSize = origSize;
        GUI.skin.label.alignment = origTextAnchor;

        GUILayout.Space(30);
        if (GUILayout.Button("Back", GUILayout.Height(60)))
        {
            goBack();
        }

        GUILayout.FlexibleSpace();
    }
}
