using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class AnalogInputDropdownUI : MonoBehaviour
{
    public enum LabJackInputEnum
    {
        Disabled,
        AIN0,
        AIN1,
        AIN2,
        AIN3,
        AIN4,
        AIN5,
        AIN6,
        AIN7,
        AIN8,
        AIN9,
        AIN10,
        AIN11,
        AIN12,
        AIN13,
        AIN14,  
        AIN15
    }

    public AnalogStim analogStim;

    public TMP_Dropdown pitch;
    public TMP_Dropdown roll;
    public TMP_Dropdown yaw;
    public TMP_Dropdown ap;
    public TMP_Dropdown ml;
    public TMP_Dropdown ud;
    public TMP_Dropdown scale;

    private void Start()
    {
        SetDropdownOptions(pitch);        
        SetDropdownOptions(roll);        
        SetDropdownOptions(yaw);        
        SetDropdownOptions(ap);        
        SetDropdownOptions(ml);        
        SetDropdownOptions(ud);        
        SetDropdownOptions(scale);
    }


    void FixedUpdate()
    {
        analogStim.pitchEnabled = pitch.value > 0;
        analogStim.rollEnabled = roll.value > 0;
        analogStim.yawEnabled = yaw.value > 0;
        analogStim.apEnabled = ap.value > 0;
        analogStim.mlEnabled = ml.value > 0;
        analogStim.udEnabled = ud.value > 0;
        analogStim.scaleEnabled = scale.value > 0;

        analogStim.pitchInput = ((LabJackInputEnum)pitch.value).ToString(); 
        analogStim.rollInput = ((LabJackInputEnum)roll.value).ToString(); 
        analogStim.yawInput = ((LabJackInputEnum)yaw.value).ToString(); 
        analogStim.apInput = ((LabJackInputEnum)ap.value).ToString(); 
        analogStim.mlInput = ((LabJackInputEnum)ml.value).ToString(); 
        analogStim.udInput = ((LabJackInputEnum)ud.value).ToString(); 
        analogStim.scaleInput = ((LabJackInputEnum)scale.value).ToString();
    }

    private void SetDropdownOptions(TMP_Dropdown dropdown)
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(Enum.GetNames(typeof(LabJackInputEnum)).ToList());
    }
}
