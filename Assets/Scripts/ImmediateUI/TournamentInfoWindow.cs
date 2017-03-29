using UnityEngine;

using SimpleJSON;

public class TournamentInfoWindow : WindowBase<TournamentInfoWindow>
{

    JSONNode tournamentInfo;

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
        wndHeight = 500;
        windowTitle = "Tournament info";
        windowFun = tournamentInfotWindow;
    }

    public override void Show()
    {
        base.Show();

        mCurrentState = State.Loading;
        loadingMessage = "Loading tournament info...";
        TournamaticClient.Instance.GetTournament(SessionData.Instance.tournamentId, tournamentInfoResult);
    }


    protected override void OnGUI()
    {
        GUI.skin.button.fontSize = 25;
        GUI.skin.label.fontSize = 20;
        GUI.skin.textField.fontSize = 25;
        GUI.skin.box.fontSize = 20;
        GUI.skin.box.wordWrap = true;
        GUI.skin.box.alignment = TextAnchor.MiddleLeft;

        base.OnGUI();
    }

    void tournamentInfotWindow(int id)
    {
        if (mCurrentState == State.Loading)
        {
            GUI.skin.label.fontSize = 25;
            GUILayout.Label(loadingMessage, GUILayout.Height(100));
            GUI.skin.label.fontSize = 15;
        }
        else if (mCurrentState == State.Error)
        {
            GUILayout.FlexibleSpace();
            GUI.skin.label.fontSize = 20;
            GUILayout.Label(errorMessage, GUILayout.Height(70));
            GUI.skin.label.fontSize = 15;

            GUILayout.Space(20);
            if (GUILayout.Button("Retry", GUILayout.Height(60)))
            {
                Show();
            }

            GUILayout.FlexibleSpace();
        }
        else if (mCurrentState == State.Normal)
        {
            showTournamentInfo();
        }
    }

    void showTournamentInfo()
    {
        if (tournamentInfo == null)
            return;

        GUILayout.Space(20);

        GUILayout.BeginHorizontal();
        GUI.skin.label.fontStyle = FontStyle.Bold;
        GUILayout.Label("Title:", GUILayout.Width(wndWidth / 2));
        GUI.skin.label.fontStyle = FontStyle.Normal;
        GUILayout.Box(tournamentInfo["title"].Value);
        GUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();

        GUI.skin.label.fontStyle = FontStyle.Bold;
        GUILayout.Label("Short Description:");
        GUI.skin.label.fontStyle = FontStyle.Normal;
        GUILayout.Box(tournamentInfo["shortDescription"].Value);

        GUILayout.FlexibleSpace();

        GUI.skin.label.fontStyle = FontStyle.Bold;
        GUILayout.Label("Long Description:");
        GUI.skin.label.fontStyle = FontStyle.Normal;
        GUILayout.Box(tournamentInfo["description"].Value, GUILayout.Width(wndWidth));

        GUILayout.FlexibleSpace();
        /*
        GUILayout.BeginHorizontal();
        GUI.skin.label.fontStyle = FontStyle.Bold;
        GUILayout.Label("Start Date:", GUILayout.Width(wndWidth / 2));
        GUI.skin.label.fontStyle = FontStyle.Normal;
        GUILayout.Box(tournamentInfo["start"].Value);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUI.skin.label.fontStyle = FontStyle.Bold;
        GUILayout.Label("End Date:", GUILayout.Width(wndWidth / 2));
        GUI.skin.label.fontStyle = FontStyle.Normal;
        GUILayout.Box(tournamentInfo["end"].Value);
        GUILayout.EndHorizontal();
        */
        GUILayout.BeginHorizontal();
        GUI.skin.label.fontStyle = FontStyle.Bold;
        GUILayout.Label("Location Date:", GUILayout.Width(wndWidth / 2));
        GUI.skin.label.fontStyle = FontStyle.Normal;
        GUILayout.Box(tournamentInfo["location"].Value);
        GUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Enter", GUILayout.Height(60)))
        {
            Hide();
            SigninWindow.Instance.Show();
        }

        GUILayout.Space(15);

        if (GUILayout.Button("Back", GUILayout.Height(60)))
        {
            Hide();
            TournamentSelectWindow.Instance.Show();
        }

    }

    void tournamentInfoResult(JSONNode responseNode)
    {
        if (responseNode["status"].Value != "ok")
        {
            errorMessage = "Failed to load tournament info." + "\n" + responseNode.ToString();
            mCurrentState = State.Error;
            return;
        }

        tournamentInfo = responseNode["data"];
        tournamentInfo["description"].Value = tournamentInfo["description"].Value.Replace("<p>", "").Replace("</p>", "").Replace("<br>", "");
        SessionData.Instance.drawId = tournamentInfo["draws"][0]["drawId"].AsInt;   // First draw.

        mCurrentState = State.Normal;
        Debug.Log(tournamentInfo.ToString());
    }

}
