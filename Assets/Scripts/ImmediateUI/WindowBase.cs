using UnityEngine;
using System.Collections;

public class WindowBase<T> : MonoBehaviour
{
    public static T Instance;

    Rect mWindowRect;
    protected GUI.WindowFunction windowFun;

    const float nativeWidth = 1280f;
    const float nativeHeight = 720f;
    //const float nativeWidth = 1920f;
    //const float nativeHeight = 1080f;

    protected int wndWidth = 400;
    protected int wndHeight = 200;

    Vector3 uiScale;

    protected string windowTitle;
    protected string loadingMessage;
    protected string errorMessage;


    // Use this for initialization
    protected virtual void Start()
    {
        uiScale.x = Screen.width / nativeWidth;
        uiScale.y = Screen.height / nativeHeight;
        uiScale.z = 1;

        mWindowRect = new Rect((nativeWidth - wndWidth) / 2, (nativeHeight - wndHeight) / 2, wndWidth, wndHeight);
    }

    void OnDestry()
    {
        Instance = default(T);
    }

    protected virtual void OnGUI()
    {
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, uiScale);

        mWindowRect = GUILayout.Window(0, mWindowRect, windowFun, windowTitle);
    }

    public virtual void Show()
    {
        enabled = true;
    }

    public virtual void Hide()
    {
        enabled = false;
    }
}
