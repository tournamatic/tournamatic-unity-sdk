using UnityEngine;
using System.Collections.Generic;

using SimpleJSON;

public class TournamentSelectWindow : WindowBase<TournamentSelectWindow>
{
    Vector2 scrollPosition;

    enum State
    {
        Normal,
        Loading,
        Error
    }
    State mCurrentState;

    const int TournamentCategory = 24;      // The dart category.

    List<Tournament> tournamentsList = new List<Tournament>();

    void Awake()
    {
        Instance = this;

        // Set window properties.
        wndWidth = 800;
        wndHeight = 400;
        windowTitle = "Tournaments";
        windowFun = tournamentsListWindow;
    }

    protected override void Start()
    {
        base.Start();

        Show();
    }

    public override void Show()
    {
        base.Show();

        tournamentsList.Clear();
        mCurrentState = State.Loading;
        loadingMessage = "Loading tournaments...";
        TournamaticClient.Instance.GetTournamentsInCategory(TournamentCategory, tournamentsListResult);
    }

    protected override void OnGUI()
    {
        GUI.skin.button.fontSize = 25;
        GUI.skin.label.fontSize = 15;
        GUI.skin.textField.fontSize = 25;

        base.OnGUI();
    }

    void tournamentsListWindow(int id)
    {
        if (mCurrentState == State.Loading)
        {
            GUI.skin.label.fontSize = 25;
            GUILayout.Label(loadingMessage, GUILayout.Height(100));
            GUI.skin.label.fontSize = 15;
            return;
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
            showTournaments();
        }
    }

    void showTournaments()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        for (int i = 0; i < tournamentsList.Count; ++i)
        {
            if (GUILayout.Button(tournamentsList[i].title, GUILayout.Height(80)))
            {
                Hide();
                SessionData.Instance.tournamentId = tournamentsList[i].tournamentId;
                TournamentInfoWindow.Instance.Show();
            }
            GUILayout.Space(20);
        }

        GUILayout.EndScrollView();
    }

    void tournamentsListResult(JSONNode responseNode)
    {
        if (responseNode["status"].Value != "ok")
        {
            mCurrentState = State.Error;
            errorMessage = "Failed to load tournaments." + "\n" + responseNode.ToString();
            return;
        }

        JSONArray tournamentsArray = responseNode["data"].AsArray;

        foreach (JSONNode tournamentObj in tournamentsArray)
        {
            Tournament tournament = new Tournament();
            tournament.tournamentId = tournamentObj["tournamentId"].AsInt;
            tournament.title = tournamentObj["title"].Value;

            tournamentsList.Add(tournament);
        }

        mCurrentState = State.Normal;
    }
}
