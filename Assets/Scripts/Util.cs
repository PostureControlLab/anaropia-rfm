#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class Util
{
    public static void AdaptiveQuit(int exitCode)
    {
        if (Application.isEditor)
        {
#if UNITY_EDITOR
            Debug.Log($"Exit code: {exitCode}");
            EditorApplication.isPlaying = false;
#endif
        }
        else
        {
            Application.Quit(exitCode);
        }
    }
}

