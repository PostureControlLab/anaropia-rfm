using UnityEngine;

public class GuiManager : MonoBehaviour
{
    public static bool DoRenderGui { get; set; } = true;

    private static int nextWindowId = 1024;


    public static int GetWindowId()
    {
        return nextWindowId++;
    }

    private void LateUpdate()
    {
        // Hide IMGUI when debug console is active
        if (IngameDebugConsole.DebugLogManager.Instance)
        {
            DoRenderGui = !IngameDebugConsole.DebugLogManager.Instance.IsLogWindowVisible;
        }
    }
}
