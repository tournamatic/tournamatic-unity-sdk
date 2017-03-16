using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using SimpleJSON;


public class UpdateScoreWindow : WindowBase<UpdateScoreWindow>
{
    Vector2 scrollPosition;

    GUIStyle style1 = new GUIStyle();
    GUIStyle style2 = new GUIStyle();

    enum State
    {
        GettingRounds,
        GettingMatches,
        ReadyToRecord,
        Recording,
        Recorded,
        GettingScoreboard,
        ScoreBoard,
        Error
    }
    State mCurrentState;

    int roundId;
    int poolId;
    JSONNode matchData;
    List<Score> mScores = new List<Score>();

    int mScore;

    void Awake()
    {
        Instance = this;

        // Set window properties.
        wndWidth = 800;
        wndHeight = 600;
        windowTitle = "Scores";
        windowFun = updateScoreWindow;
    }

    protected override void Start()
    {
        base.Start();

        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, new Color(0.8f, 0.8f, 0.8f, 0.3f));
        texture.Apply();
        style1.normal.background = texture;

        texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, new Color(0.1f, 0.1f, 0.1f, 0.3f));
        texture.Apply();
        style2.normal.background = texture;

        Hide();
        mCurrentState = State.GettingRounds;
        loadingMessage = "Getting Rounds...";
        TournamaticClient.Instance.GetRounds(SessionData.Instance.tournamentId, SessionData.Instance.drawId, roundsInfoResult);
    }

    public void RecordScore(int score)
    {
        mScore = score;

        JSONNode scoreObject = new JSONClass();
        scoreObject["teamId"] = matchData["teamId"].Value;
        scoreObject["matchId"] = matchData["matchId"].Value;
        scoreObject["matchTeamId"] = matchData["matchTeamId"].Value;
        scoreObject["placeholderName"] = matchData["placeholderName"].Value;
        scoreObject["finalScore"].AsInt = score;

        JSONArray scoreArray = new JSONArray();
        scoreArray.Add(scoreObject);
        
        TournamaticClient.Instance.RecordScore(SessionData.Instance.access_token, SessionData.Instance.tournamentId, SessionData.Instance.drawId, scoreArray.ToString(), recoredResult);

        StartCoroutine("DoShowMenu");
    }

    IEnumerator DoShowMenu()
    {
        yield return new WaitForSeconds(1);

        Show();
    }

    protected override void OnGUI()
    {
        GUI.skin.button.fontSize = 25;
        GUI.skin.label.fontSize = 15;
        GUI.skin.textField.fontSize = 25;
        
        base.OnGUI();
    }

    void updateScoreWindow(int id)
    {
        if (mCurrentState != State.ScoreBoard)
        {
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUI.skin.label.fontSize = 25;
            GUILayout.Label("Your Score: ", GUILayout.Height(100));
            GUILayout.Label(mScore.ToString(), GUILayout.Height(100));
            GUI.skin.label.fontSize = 15;
            GUILayout.EndHorizontal();
        }

        if (mCurrentState == State.GettingMatches || mCurrentState == State.GettingRounds || mCurrentState == State.GettingScoreboard)
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
                mCurrentState = State.GettingRounds;
                loadingMessage = "Getting Rounds...";
                TournamaticClient.Instance.GetRounds(SessionData.Instance.tournamentId, SessionData.Instance.drawId, roundsInfoResult);
            }

            GUILayout.FlexibleSpace();
        }
        else if (mCurrentState == State.Recorded)
        {
            GUILayout.Space(70);
            if (GUILayout.Button("View Scoreboard", GUILayout.Height(60)))
            {
                mCurrentState = State.GettingScoreboard;
                loadingMessage = "Loading Scoreboard...";
                TournamaticClient.Instance.GetLeaderboard(SessionData.Instance.tournamentId, SessionData.Instance.drawId, roundId, poolId, leaderboardResult);
            }
        }
        else if (mCurrentState == State.ScoreBoard)
        {
            drawScoreboard();
        }
    }

    void drawScoreboard()
    {
        GUI.skin.label.fontSize = 20;
        GUILayout.BeginHorizontal(GUI.skin.box);

        TextAnchor origTextAnchor = GUI.skin.label.alignment;
        GUI.skin.label.alignment = TextAnchor.MiddleLeft;

        GUILayout.Label("RANK", GUILayout.Width(80), GUILayout.Height(50));
        GUILayout.Label("NAME", GUILayout.Width(300), GUILayout.Height(50));
        GUILayout.Label("SCORE", GUILayout.Height(50));

        GUILayout.EndHorizontal();

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        for (int i = 0; i < mScores.Count; ++i)
        {
            GUILayout.BeginHorizontal((i % 2 == 0 ? style1 : style2));

            GUILayout.Label(mScores[i].position.ToString(), GUILayout.Width(50), GUILayout.Height(50));
            GUILayout.Label(mScores[i].teamName, GUILayout.Width(300), GUILayout.Height(50));
            GUILayout.Label(mScores[i].score.ToString(), GUILayout.Width(50), GUILayout.Height(50));

            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();

        GUI.skin.label.alignment = origTextAnchor;
        GUI.skin.label.fontSize = 15;
    }

    void roundsInfoResult(JSONNode responseNode)
    {
        if (responseNode["status"].Value != "ok")
        {
            errorMessage = responseNode.ToString();
            mCurrentState = State.Error;
            return;
        }

        Debug.Log(responseNode.ToString());

        roundId = responseNode["data"][0]["roundId"].AsInt;

        mCurrentState = State.GettingMatches;
        loadingMessage = "Getting Matches...";
        TournamaticClient.Instance.GetMatches(SessionData.Instance.tournamentId, SessionData.Instance.drawId, roundId, matchesResult);
    }

    void matchesResult(JSONNode responseNode)
    {
        if (responseNode["status"].Value != "ok")
        {
            errorMessage = responseNode.ToString();
            mCurrentState = State.Error;
            return;
        }

        Debug.Log(responseNode.ToString());

        string placeHolderName = SessionData.Instance.firstName + " " + SessionData.Instance.lastName;
        foreach (JSONNode team in responseNode["data"][0]["teams"].AsArray)
        {
            if (team["placeholderName"].Value == placeHolderName)
            {
                matchData = team;
                break;
            }
        }

        poolId = responseNode["data"][0]["poolId"].AsInt;
        mCurrentState = State.ReadyToRecord;
    }

    void recoredResult(JSONNode responseNode)
    {
        if (responseNode["status"].Value == "ok")
        {
            Debug.Log("Record Score is Success!");

            mCurrentState = State.Recorded;
        }
        else
        {
            mCurrentState = State.Error;
            errorMessage = "Recording Score failed!\n" + responseNode.ToString();
        }
    }

    void leaderboardResult(JSONNode responseNode)
    {
        if (responseNode["status"].Value != "ok")
        {
            errorMessage = responseNode.ToString();
            mCurrentState = State.Error;
            return;
        }

        JSONArray scoreData = responseNode["data"].AsArray;

        foreach (JSONNode scoreObj in scoreData)
        {
            Score score = new Score();
            score.fromJson(scoreObj);
            mScores.Add(score);
        }

        mCurrentState = State.ScoreBoard;
        //Debug.Log(responseNode.ToString());
    }
}
