using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalogStim : MonoBehaviour
{
    public class AnalogStimRecord
    {
        public float pitch = 0.0f;
        public float roll = 0.0f;
        public float yaw = 0.0f;

        public float ap = 0.0f;
        public float ml = 0.0f;
        public float ud = 0.0f;

        public float scale = 1.0f;
    }

    public LabJackReader labJackReader;
    public string pitchInput;
    public string rollInput;
    public string yawInput;
    public string apInput;
    public string mlInput;
    public string udInput;
    public string scaleInput;

    [Space]

    public float pitchScale = 1;
    public float rollScale = 1;
    public float yawScale = 1;
    public float apScale = 1;
    public float mlScale = 1;
    public float udScale = 1;
    public float scaleScale = 1;

    [Space]

    public bool pitchEnabled = true;
    public bool rollEnabled = true;
    public bool yawEnabled = true;
    public bool apEnabled = true;
    public bool mlEnabled = true;
    public bool udEnabled = true;
    public bool scaleEnabled = true;


    public StimulusRecord ReadFromLabJack()
    {
        return new StimulusRecord
        {
            Pitch = (float)LabJackValueOrDefault(pitchInput, pitchEnabled, pitchScale, 0.0),
            Roll = (float)LabJackValueOrDefault(rollInput, rollEnabled, rollScale, 0.0),
            Yaw = (float)LabJackValueOrDefault(yawInput, yawEnabled, yawScale, 0.0),

            Trans_Ap = (float)LabJackValueOrDefault(apInput, apEnabled, apScale, 0.0),
            Trans_Ml = (float)LabJackValueOrDefault(mlInput, mlEnabled, mlScale, 0.0),
            Trans_Ud = (float)LabJackValueOrDefault(udInput, udEnabled, udScale, 0.0),

            Scale = (float)LabJackValueOrDefault(scaleInput, scaleEnabled, scaleScale, 1.0)
        };
    }

    private double LabJackValueOrDefault(string key, bool enabled = true, double scale = 1.0, double defaultValue = 0.0)
    {
        if (!enabled)
        {
            return defaultValue;
        }

        return labJackReader.GetNamedValue(key) * scale ?? defaultValue;
    }
}
