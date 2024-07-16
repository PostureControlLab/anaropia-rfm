using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using CsvHelper;
using System.Linq;
using System.Data;
using CsvHelper.Configuration;
using System.Threading;
using System.Globalization;
using System.Text.RegularExpressions;

/// <summary>
/// Move object it is attached to according to stimulus file.
/// </summary>
public class MoveStim : MonoBehaviour
{
    public enum InputKeymap
    {
        Keypad,
        Alpha
    };
    public enum StimuliSet
    {
        Room,
        Screen
    }

    public AnalogStim analogStim;
    public string outputFolderName = "vr_expt_data";
    public string stimulusName = "STIMULUS_NAME";
    public InputKeymap selectedKeymap;
    public StimuliSet stimuliSet;
    public Transform rotationCenter;
    public float resetTime = 3.0f;

    [Header("Trigger")]
    public bool useTrigger;
    public string triggerInput;
    public float triggerThreshold;


    // Keymaps
    private Keymap keypadKeymap = new Keymap
    {
        stop = KeyCode.Keypad0,
        manualRecord = KeyCode.KeypadMultiply,
        stim1 = KeyCode.Keypad1,
        stim2 = KeyCode.Keypad2,
        stim3 = KeyCode.Keypad3,
        stim4 = KeyCode.Keypad4,
        stim5 = KeyCode.Keypad5,
        stim6 = KeyCode.Keypad6,
        stim7 = KeyCode.Keypad7,
        stim8 = KeyCode.Keypad8,
        stim9 = KeyCode.Keypad9
    };
    private Keymap alphaKeymap = new Keymap
    {
        stop = KeyCode.Alpha0,
        manualRecord = KeyCode.R,
        stim1 = KeyCode.Alpha1,
        stim2 = KeyCode.Alpha2,
        stim3 = KeyCode.Alpha3,
        stim4 = KeyCode.Alpha4,
        stim5 = KeyCode.Alpha5,
        stim6 = KeyCode.Alpha6,
        stim7 = KeyCode.Alpha7,
        stim8 = KeyCode.Alpha8,
        stim9 = KeyCode.Alpha9
    };
    private Keymap keymap;
    private ExperimentGui stimuliSelection;

    // Position and rotation tracking
    private Transform cameraTransform;
    private Transform shoulderTransform, hipTransform;
    private Transform leftHandTransform;
    private Transform rightHandTransform;
    private Transform leftFootTransform;
    private Transform rightFootTransform;
    private Vector3 localStartPosition;
    private Quaternion localStartRotation;

    // Stimulus
    private float stimulusNum; // used to fetch stimulus file and record stim values
    private List<StimulusRecord> stimuli;
    private string currentStimulusPath;

    // Movement variables
    private bool stimRunning = false;
    private bool isResetting = false;
    private float elapsedResetTime = 0.0f;
    private float startTime;
    private float elapsedTime = 0;
    private float stimulusLength = 0;
    private List<DataRecord> movementData = new List<DataRecord>();
    private string filePath;

    // Manual recording
    private float manualStartTime;
    private List<DataRecord> manualMovementData = new List<DataRecord>();
    private bool isManualRecording = false;
    private string manualRecordingFilePath;

    private int windowId = GuiManager.GetWindowId();
    private Rect windowRect;

    private string basePath;

    private IScaler scaler;

    void Start()
    {
        basePath = System.Environment.CurrentDirectory;

        // Set up keymap
        if (selectedKeymap == InputKeymap.Alpha)
        {
            keymap = alphaKeymap;
        }
        else if (selectedKeymap == InputKeymap.Keypad)
        {
            keymap = keypadKeymap;
        }

        stimuliSelection = FindObjectOfType<ExperimentGui>();

        transform.SetParent(rotationCenter.transform, worldPositionStays: true);

        // Start position of this stimulus object
        localStartPosition = rotationCenter.transform.localPosition;
        localStartRotation = rotationCenter.transform.localRotation;

        // Set camera transform
        cameraTransform = Camera.main.transform;

        // Find hip and shoulder trackers
        shoulderTransform = GameObject.FindWithTag("shoulder").transform;
        hipTransform = GameObject.FindWithTag("hip").transform;

        // Find hands
        leftHandTransform = GameObject.FindWithTag("left_hand").transform;
        rightHandTransform = GameObject.FindWithTag("right_hand").transform;

        // Find feet
        leftFootTransform = GameObject.FindWithTag("left_foot").transform;
        rightFootTransform = GameObject.FindWithTag("right_foot").transform;

        startTime = Time.realtimeSinceStartup;

        scaler = GetComponent<IScaler>();

        if (analogStim is null)
        {
            analogStim = GetComponent<AnalogStim>();
        }
    }

    //private void CheckExternalTrigger()
    //{
    //    if (experimentManager.TestModeEnabled && !doUseLabJackForTriggerTest)
    //    {
    //        if (testModeStartTrigger)
    //        {
    //            testModeStartTrigger = false;
    //            var started = TryStartTrial();

    //            Debug.Log($"Triggered. Started: {started}");
    //        }
    //        else if (testModeStopTrigger)
    //        {
    //            testModeStopTrigger = false;
    //            FinishTrial();

    //            Debug.Log($"Triggered. Stopped.");
    //        }
    //    }
    //    else
    //    {
    //        var currentTriggerValue = analogStim.labJackReader.GetNamedValueOrZero(triggerInput);

    //        // High voltage: start
    //        if (!stimRunning && currentTriggerValue >= triggerThreshold)
    //        {
    //            // Start recording
    //        }
    //        // Low voltage: stop
    //        else if (stimRunning && currentTriggerValue < triggerThreshold)
    //        {
    //            // Stop recording 
    //        }
    //    }
    //}

    void Update()
    {
        bool doSwitchManualRecordingState = false;

        var stimRecord = new StimulusRecord();

        // Time since start of stimulus
        elapsedTime = Time.realtimeSinceStartup - startTime;

        if (!isResetting)
        {
            if (useTrigger)
            {
                // Check trigger
                var currentTriggerValue = analogStim.labJackReader.GetNamedValueOrZero(triggerInput);

                // High voltage: start
                if (!isManualRecording && currentTriggerValue >= triggerThreshold)
                {
                    doSwitchManualRecordingState = true;
                }
                // Low voltage: stop
                else if (isManualRecording && currentTriggerValue < triggerThreshold)
                {
                    doSwitchManualRecordingState = true;
                }
            }

            /// Fetch stimulus values based on key pressed and record data,
            if (Input.GetKeyDown(keymap.stop))
            {
                Debug.Log($"({stimulusName}) Stop movement");
                stimulusNum = 0;
                BeginReset();
                WriteMovementData(movementData, filePath.Replace(".csv", "_INCOMPLETE.csv"));
                Debug.LogWarning($"({stimulusName}) Wrote INCOMPLETE movement data to: {filePath}");
                stimRunning = false;
            }
            if (Input.GetKeyDown(keymap.manualRecord))
            {
                doSwitchManualRecordingState = true;
            }

            if (!stimRunning)
            {
                if (Input.GetKeyDown(keymap.stim1))
                {
                    stimulusNum = 1;
                    stimRunning = false;
                }

                if (Input.GetKeyDown(keymap.stim2))
                {
                    stimulusNum = 2;
                    stimRunning = false;
                }

                if (Input.GetKeyDown(keymap.stim3))
                {
                    stimulusNum = 3;
                    stimRunning = false;
                }

                if (Input.GetKeyDown(keymap.stim4))
                {
                    stimulusNum = 4;
                    stimRunning = false;
                }

                if (Input.GetKeyDown(keymap.stim5))
                {
                    stimulusNum = 5;
                    stimRunning = false;
                }

                if (Input.GetKeyDown(keymap.stim6))
                {
                    stimulusNum = 6;
                    stimRunning = false;
                }

                if (Input.GetKeyDown(keymap.stim7))
                {
                    stimulusNum = 7;
                    stimRunning = false;
                }

                if (Input.GetKeyDown(keymap.stim8))
                {
                    stimulusNum = 8;
                    stimRunning = false;
                }

                if (Input.GetKeyDown(keymap.stim9))
                {
                    stimulusNum = 9;
                    stimRunning = false;
                }
            }
        }


        var stimuliDir = stimuliSet == StimuliSet.Room ? stimuliSelection.roomStimuliSet : stimuliSelection.screenStimuliSet;


        if (stimuliDir == "__LabJack")
        {
            stimRecord = analogStim.ReadFromLabJack();

            rotationCenter.transform.localEulerAngles = new Vector3(stimRecord.Pitch, stimRecord.Yaw, stimRecord.Roll);
            rotationCenter.transform.localPosition = localStartPosition + new Vector3(stimRecord.Trans_Ml, stimRecord.Trans_Ud, stimRecord.Trans_Ap);
            scaler?.SetScale(stimRecord.Scale);
        }
        else if (stimuliDir == "None")
        {
            rotationCenter.transform.localRotation = Quaternion.identity;
            rotationCenter.transform.localPosition = localStartPosition;
            rotationCenter.transform.localScale = Vector3.one;
        }
        // For key>0 create output file and record stim values from file, if file exists
        // else do nothing and app continues to run as normal
        // Stim file name expected (example): vr_stim_1.csv
        // The number at the end of the file corresponds to the number keys 1-9
        else if (stimuliDir != "__LabJack" && stimuliDir != "None" && stimulusNum > 0 && !doSwitchManualRecordingState)
        {
            /// move components and record data, if file exists
            /// else do nothing and app continues to run as normal
            if (stimRunning)
            {
                var index = (int)Math.Floor(elapsedTime * 1000);

                if (index < stimuli.Count)
                {
                    var roll = stimuli[index].Roll;
                    var pitch = stimuli[index].Pitch;
                    var yaw = stimuli[index].Yaw;

                    var x = stimuli[index].Trans_Ml;
                    var y = stimuli[index].Trans_Ud;
                    var z = stimuli[index].Trans_Ap;

                    rotationCenter.transform.localEulerAngles = new Vector3(pitch, yaw, roll);
                    rotationCenter.transform.localPosition = localStartPosition + new Vector3(x, y, z);
                    // BUG: Not working correctly
                    scaler?.SetScale(stimuli[index].Scale);

                    stimRecord = stimuli[index];

                    TrackMovement(movementData, elapsedTime, stimRecord);
                }
                else
                {
                    Debug.Log($"({stimulusName}) Stimulus finished. Resetting to 0.");
                    stimulusNum = 0;
                    stimRunning = false;
                    BeginReset();

                    var writtenPath = WriteMovementData(movementData, filePath);
                    Debug.Log($"({stimulusName}) Wrote movement data to: {writtenPath}");
                }
            }
            else
            {
                try
                {
                    stimRunning = true;
                    
                    ClearMovementData(movementData);

                    // read stim file and store time and value in separate lists
                    currentStimulusPath = Path.Combine(basePath, "stimuli", stimuliDir, "vr_stim_" + stimulusNum + ".csv");

                    var config = new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
                    {
                        PrepareHeaderForMatch = args => args.Header.ToLower(),
                        MissingFieldFound = null,
                        //HeaderValidated = null
                    };
                    using (var sr = new StreamReader(currentStimulusPath, System.Text.Encoding.UTF8))
                    using (var csv = new CsvReader(sr, config))
                    {
                        try
                        {
                            stimuli = csv.GetRecords<StimulusRecord>().ToList();
                        }
                        catch (HeaderValidationException e)
                        {
                            Debug.LogError(e.Message);
                            Util.AdaptiveQuit(-1);
                        }
                    }

                    stimulusLength = stimuli.Last().Time;

                    filePath = GetOutputFilePath();

                    Debug.Log($"({stimulusName}) Starting stimulus {stimulusNum} of set {stimuliDir}");

                    startTime = Time.realtimeSinceStartup;
                }
                catch (FileNotFoundException)
                {
                    Debug.LogError("File named vr_stim_" + stimulusNum + ".csv not found. Ensure stimulus file is present for the key pressed");
                    stimulusNum = 0;
                    stimRunning = false;
                }
            }
        }
        
        if (doSwitchManualRecordingState)
        {
            // Stop manual recording
            if (isManualRecording)
            {
                Debug.Log($"({stimulusName}) Stopping manual recording");
                var writtenPath = WriteMovementData(manualMovementData, manualRecordingFilePath);
                Debug.Log($"({stimulusName}) Wrote manually recorded movement data to: {writtenPath}");
                BeginReset();

                isManualRecording = false;
            }
            // Start manual recording
            else if (stimuliDir != "None")
            {
                Debug.Log($"({stimulusName}) Starting manual recording");
                manualRecordingFilePath = GetOutputFilePath(isManualRecording: true);
                manualStartTime = Time.realtimeSinceStartup;
                ClearMovementData(manualMovementData);

                isManualRecording = true;
            }
        }

        if (isManualRecording)
        {
            var manualElapsedTime = Time.realtimeSinceStartup - manualStartTime;
            TrackMovement(manualMovementData, manualElapsedTime, stimRecord);
        }
    }

    private void BeginReset()
    {
        StartCoroutine(Reset()); 
    }

    private IEnumerator Reset()
    {
        isResetting = true;
        elapsedResetTime = 0.0f;

        Vector3 startingPosition = rotationCenter.transform.localPosition;
        Quaternion startingRotation = rotationCenter.transform.rotation;

        Vector3 targetPosition = localStartPosition;
        Quaternion targetRotation = localStartRotation;

        scaler?.SetScale(1.0f);

        while (elapsedResetTime < resetTime)
        {
            elapsedResetTime += Time.deltaTime;
            rotationCenter.transform.localPosition = Vector3.Lerp(startingPosition, targetPosition, (elapsedResetTime / resetTime));
            rotationCenter.transform.localRotation = Quaternion.Slerp(startingRotation, targetRotation, (elapsedResetTime / resetTime));
            yield return new WaitForEndOfFrame();
        }

        isResetting = false;
    }

    // Record movement
    private void TrackMovement(List<DataRecord> data, float time, StimulusRecord stimulus)
    {
        data.Add(new DataRecord
        {
            time = time,

            // stimulus
            stim_pitch = stimulus.Pitch,
            stim_roll = stimulus.Roll,
            stim_yaw = stimulus.Yaw,
            stim_trans_ap = stimulus.Trans_Ap,
            stim_trans_ml = stimulus.Trans_Ml,
            stim_trans_ud = stimulus.Trans_Ud,
            stim_scale = stimulus.Scale,

            // head movement
            xpos = cameraTransform.position.x,
            ypos = cameraTransform.position.y,
            zpos = cameraTransform.position.z,
            xrot = cameraTransform.eulerAngles.x,
            yrot = cameraTransform.eulerAngles.y,
            zrot = cameraTransform.eulerAngles.z,

            // shoulder tracker
            shld_xpos = shoulderTransform.position.x,
            shld_ypos = shoulderTransform.position.y,
            shld_zpos = shoulderTransform.position.z,
            shld_xrot = shoulderTransform.eulerAngles.x,
            shld_yrot = shoulderTransform.eulerAngles.y,
            shld_zrot = shoulderTransform.eulerAngles.z,

            // hip tracker
            hip_xpos = hipTransform.position.x,
            hip_ypos = hipTransform.position.y,
            hip_zpos = hipTransform.position.z,
            hip_xrot = hipTransform.eulerAngles.x,
            hip_yrot = hipTransform.eulerAngles.y,
            hip_zrot = hipTransform.eulerAngles.z,

            // left hand
            lhand_xpos = leftHandTransform.position.x,
            lhand_ypos = leftHandTransform.position.y,
            lhand_zpos = leftHandTransform.position.z,
            lhand_xrot = leftHandTransform.eulerAngles.x,
            lhand_yrot = leftHandTransform.eulerAngles.y,
            lhand_zrot = leftHandTransform.eulerAngles.z,

            // right hand
            rhand_xpos = rightHandTransform.position.x,
            rhand_ypos = rightHandTransform.position.y,
            rhand_zpos = rightHandTransform.position.z,
            rhand_xrot = rightHandTransform.eulerAngles.x,
            rhand_yrot = rightHandTransform.eulerAngles.y,
            rhand_zrot = rightHandTransform.eulerAngles.z,

            // left foot
            lfoot_xpos = leftFootTransform.position.x,
            lfoot_ypos = leftFootTransform.position.y,
            lfoot_zpos = leftFootTransform.position.z,
            lfoot_xrot = leftFootTransform.eulerAngles.x,
            lfoot_yrot = leftFootTransform.eulerAngles.y,
            lfoot_zrot = leftFootTransform.eulerAngles.z,

            // right foot
            rfoot_xpos = rightFootTransform.position.x,
            rfoot_ypos = rightFootTransform.position.y,
            rfoot_zpos = rightFootTransform.position.z,
            rfoot_xrot = rightFootTransform.eulerAngles.x,
            rfoot_yrot = rightFootTransform.eulerAngles.y,
            rfoot_zrot = rightFootTransform.eulerAngles.z,

            // analog inputs
            analog0 = analogStim.labJackReader.GetNamedValueOrZero("AIN0"),
            analog1 = analogStim.labJackReader.GetNamedValueOrZero("AIN1"),
            analog2 = analogStim.labJackReader.GetNamedValueOrZero("AIN2"),
            analog3 = analogStim.labJackReader.GetNamedValueOrZero("AIN3"),
            analog4 = analogStim.labJackReader.GetNamedValueOrZero("AIN4"),
            analog5 = analogStim.labJackReader.GetNamedValueOrZero("AIN5"),
            analog6 = analogStim.labJackReader.GetNamedValueOrZero("AIN6"),
            analog7 = analogStim.labJackReader.GetNamedValueOrZero("AIN7"),
            analog8 = analogStim.labJackReader.GetNamedValueOrZero("AIN8"),
            analog9 = analogStim.labJackReader.GetNamedValueOrZero("AIN9"),
            analog10 = analogStim.labJackReader.GetNamedValueOrZero("AIN10"),
            analog11 = analogStim.labJackReader.GetNamedValueOrZero("AIN11"),
            analog12 = analogStim.labJackReader.GetNamedValueOrZero("AIN12"),
            analog13 = analogStim.labJackReader.GetNamedValueOrZero("AIN13"),
        });
    }

    // Data file column format
    // Time
    // Stim
    // Head translation x y z
    // Head rotation x y z
    // Shoulder translation x y z
    // Shoudler rotation x y z
    // Hip translation x y z
    // Hip rotation x y z
    private string WriteMovementData(List<DataRecord> data, string path)
    {
        // Failsafe, to prevent overwriting in case something goes wrong creating another trial file
        if (File.Exists(path))
        {
            path += "_FAILSAFE";
        }

        using (var writer = new StreamWriter(path))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture)) 
        {
            csv.WriteRecords(data);
        }

        return path;
    }

    // Clear movement data
    private void ClearMovementData(List<DataRecord> data)
    {
        data.Clear();
    }

    private string GetOutputFilePath(bool isManualRecording = false)
    {
        var stimuliDir = stimuliSet == StimuliSet.Room ? stimuliSelection.roomStimuliSet : stimuliSelection.screenStimuliSet;
        var currentStimulusPath = Path.Combine(basePath, "stimuli", stimuliDir, "vr_stim_" + stimulusNum + ".csv");
        var subject = $"s{stimuliSelection.subjectNumber}";
        var stimulusSetName = Path.GetFileName(Path.GetDirectoryName(currentStimulusPath));
        var subjectPath = Path.Combine(Environment.CurrentDirectory, outputFolderName, subject);
        Directory.CreateDirectory(subjectPath);

        var trial = $"t{CalculateTrialNumber(subjectPath)}";

        //var trial = $"t{Directory.GetFiles(subjectPath).Length + 1}";

        var stimulusNumberString = stimulusSetName == "__LabJack" ? "Analog" : stimulusNum.ToString();
        stimulusSetName = stimulusSetName.Replace("_", "");

        //var file = $"{stimulusName}_{stimulusSetName}_{stimulusNum}_{subject}_{trial}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.csv";
        var file = $"{stimulusName}_{stimulusSetName}_{stimulusNumberString}_{subject}_{trial}.csv";
        if (isManualRecording)
        {
            file = $"Manual_{file}";
        }
        var filePath = Path.Combine(subjectPath, file);

        return filePath;
    }

    private int CalculateTrialNumber(string path)
    {
        var nextTrialNumber = 1;

        if (!Directory.Exists(path))
        {
            return nextTrialNumber;
        }

        var regex = new Regex(@"_t(?<trial>\d+)");
        var filenames = Directory.GetFiles(path, $"*.csv").Select(Path.GetFileName);
        var matchingFiles = filenames.Where(f => regex.IsMatch(f)).Select(f => regex.Match(f)).ToList();

        var trialNumbers = matchingFiles.Select(f => int.Parse(f.Groups["trial"].Value)).ToList();

        if (trialNumbers.Count > 0)
        {
            nextTrialNumber = trialNumbers.Max() + 1;
        }

        return nextTrialNumber;
    }

    private void OnGUI()
    {
        if (GuiManager.DoRenderGui)
        {
            var xOffset = stimuliSet == StimuliSet.Room ? 0 : 401;

            windowRect = new Rect(xOffset, 81, 400, 300);
            windowRect = GUI.Window(windowId, windowRect, InfoWindow, $"{stimulusName} stim info");
        }
    }

    private void InfoWindow(int windowId)
    {
        if (stimRunning)
        {
            GUI.Label(new Rect(10, 20, 390, 50), $"Current stimulus: {currentStimulusPath}");
            GUI.Label(new Rect(10, 80, 390, 20), $"Remaining time: {Math.Floor(stimulusLength - elapsedTime)} sec");
            GUI.Label(new Rect(10, 110, 390, 150), $"Output file: {filePath}");
        }

        if (isManualRecording)
        {
            GUI.Label(new Rect(10, 160, 390, 50), $"Manual recording to {manualRecordingFilePath}. Press {keymap.manualRecord} to stop.");
        }

        if (isResetting)
        {
            GUI.Label(new Rect(10, 20, 390, 50), $"Resetting... ({Mathf.Floor(resetTime - elapsedResetTime)})");
        }
    }
}