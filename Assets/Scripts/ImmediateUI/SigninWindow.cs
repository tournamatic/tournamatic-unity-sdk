
using UnityEngine;

using System.Collections.Generic;

public class SigninWindow : WindowBase<SigninWindow>
{
    public LoginWindow loginWindow;
    public RegistrationWindow registerWindow;

    void Awake()
    {
        Instance = this;

        // Set window properties.
        wndWidth = 800;
        wndHeight = 400;
        windowTitle = "Sign in";
        windowFun = mainWindow;
    }

    protected override void OnGUI()
    {
        GUI.skin.button.fontSize = 25;
        GUI.skin.label.fontSize = 15;
        GUI.skin.textField.fontSize = 25;

        base.OnGUI();
    }

    void mainWindow(int id)
    {
        GUILayout.Space(45);
        GUILayout.BeginVertical();

        int origFontSize = GUI.skin.button.fontSize;
        GUI.skin.button.fontSize = 25;

        if (GUILayout.Button("Login", GUILayout.Height(60)))
        {
            this.Hide();
            loginWindow.Show();
        }

        GUILayout.Space(15);
        if (GUILayout.Button("Register", GUILayout.Height(60)))
        {
            this.Hide();
            registerWindow.Show();
        }

        GUILayout.Space(15);
        if (GUILayout.Button("Back", GUILayout.Height(60)))
        {
            Hide();
            TournamentInfoWindow.Instance.Show();
        }

        GUI.skin.button.fontSize = origFontSize;

        GUILayout.EndVertical();
    }

}
