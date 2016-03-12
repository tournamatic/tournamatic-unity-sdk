using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;

public class WebRequestTest : MonoBehaviour {

    StreamWriter outputFile;
    JSONNode jsonRoot;
    Vector2 scrollPosition;

    Rect mWindowRect;

    int wndWidth = 400;
    int wndHeight = 200;

    // Use this for initialization
    void Start()
    {
        mWindowRect = new Rect((Screen.width - wndWidth) / 2, (Screen.height - wndHeight) / 2, wndWidth, wndHeight);
    
        scrollPosition = new Vector2(150, 100);
        
        //outputFile = new StreamWriter("Tournamatic.txt");

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");

        string url = "https://demo.tournamatic.com/api/categories/3/tournaments";
        
        // Post a request to an URL with our custom headers
        WWW www = new WWW(url, null, headers);

        StartCoroutine(GetData(www));
    }

    IEnumerator GetData(WWW www)
    {
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
            Debug.Log(www.error);

        //Debug.Log(www.text);

        jsonRoot = JSONNode.Parse(www.text);
        //outputFile.WriteLine(www.text);
        //outputFile.Close();
       
    }

    void OnGUI()
    {
        mWindowRect = GUILayout.Window(0, mWindowRect, tournamentWnd, "Current Tournaments");
    }

    void tournamentWnd(int id)
    {
        GUILayout.Space(10);

        int origSize = GUI.skin.label.fontSize;
        GUI.skin.label.fontSize = 15;

        TextAnchor origTextAnchor = GUI.skin.label.alignment;
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;


        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(wndWidth-10), GUILayout.Height(wndHeight-10));

        if (jsonRoot != null)
        {
            foreach (JSONNode item in jsonRoot.AsArray)
            {
                GUILayout.Button(item["title"]);
            }
        }

        GUILayout.EndScrollView();

        GUI.skin.label.fontSize = origSize;
        GUI.skin.label.alignment = origTextAnchor;

        GUI.DragWindow();
    }

}
