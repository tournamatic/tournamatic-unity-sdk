
using System;
using SimpleJSON;

[Serializable]
public class Score
{
    public int drawId;
    public int roundId;
    public int poolId;
    public int teamId;
    public int position;
    public string teamName;
    public int played;
    public int wins;
    public int draws;
    public int losses;
    public int goalsScored;
    public int goalsAgainst;
    public int score;
    public int goalDifference;
    public int points;

    public void fromJson(JSONNode jsNode)
    {
        drawId      = jsNode["drawId"].AsInt;
        roundId     = jsNode["roundId"].AsInt;
        poolId      = jsNode["poolId"].AsInt;
        teamId      = jsNode["teamId"].AsInt;
        position    = jsNode["position"].AsInt;
        teamName    = jsNode["teamName"].Value;
        played      = jsNode["played"].AsInt;
        wins        = jsNode["wins"].AsInt;
        draws       = jsNode["draws"].AsInt;
        losses      = jsNode["losses"].AsInt;
        goalsScored = jsNode["goalsScored"].AsInt;
        goalsAgainst = jsNode["goalsAgainst"].AsInt;
        score       = jsNode["score"].AsInt;
        goalDifference = jsNode["goalDifference"].AsInt;
        points      = jsNode["points"].AsInt;
    }
}
