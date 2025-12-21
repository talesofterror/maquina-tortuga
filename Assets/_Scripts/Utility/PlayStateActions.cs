using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class PlayStateNotifier
{
    static PlayStateNotifier()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            Debug.Log("Exiting playmode."); // Perform cleanup or saving operations here
        }
    }
}
