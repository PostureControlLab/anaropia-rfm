using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ExperimentGui : MonoBehaviour
{
    [HideInInspector]
    public string roomStimuliSet;
    [HideInInspector]
    public string screenStimuliSet;
    [HideInInspector]
    public string subjectNumber;

    private GUIEx.DropdownState roomDropdownState = new GUIEx.DropdownState();
    private GUIEx.DropdownState screenDropdownState = new GUIEx.DropdownState();
    private string[] stimuli;

    private int windowId = GuiManager.GetWindowId();
    private Rect windowRect;

    // Start is called before the first frame update
    void Start()
    {
        var basePath = System.Environment.CurrentDirectory;
        Debug.Log($"Base path: {basePath}");

        // Query available stimuli folders
        var stimuliPath = Path.Combine(basePath, "stimuli");

        if (!Directory.Exists(stimuliPath))
        {
            Debug.LogError("Stimuli directory does not exist! Run the setup script.");
            Util.AdaptiveQuit(-1);
        }

        // Extract stimuli set names
        var stimuliPaths = Directory.GetDirectories(stimuliPath);
        if (stimuliPaths.Length == 0)
        {
            Debug.LogError($"No stimuli found at {stimuliPath}!");
        }
        stimuli = stimuliPaths.Select(p => Path.GetFileName(p)).ToArray();
        stimuli = stimuli.Prepend("__LabJack").Prepend("None").ToArray();
    }

    private void OnGUI()
    {
        if (GuiManager.DoRenderGui)
        {
            // Fake window behind elements so we don't have draw order problems with dropdown menu
            GUI.depth = 1;
            windowRect = new Rect(0, 0, 640, 80);
            windowRect = GUI.Window(windowId, windowRect, ExperimentWindow, "Experiment settings");
            GUI.depth = 0;

            GUI.Label(new Rect(10, 35, 100, 30), "Room stimuli: ");
            GUI.Label(new Rect(210, 35, 100, 30), "Screen stimuli: ");

            GUI.Label(new Rect(420, 35, 100, 30), "Subject number:");
            subjectNumber = GUI.TextField(new Rect(530, 35, 100, 30), subjectNumber);

            roomDropdownState = GUIEx.Dropdown(new Rect(100, 30, 100, 30), stimuli, roomDropdownState);
            screenDropdownState = GUIEx.Dropdown(new Rect(310, 30, 100, 30), stimuli, screenDropdownState);

            roomStimuliSet = roomDropdownState.Caption;
            screenStimuliSet = screenDropdownState.Caption;
        }
    }

    private void ExperimentWindow(int windowId)
    {
    }
}
