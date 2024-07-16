using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnalogInputToggleUI : MonoBehaviour
{
    public AnalogStim analogStim;

    public Toggle pitchToggle;
    public Toggle rollToggle;
    public Toggle yawToggle;
    public Toggle apToggle;
    public Toggle mlToggle;
    public Toggle udToggle;
    public Toggle scaleToggle;


    void Update()
    {
        analogStim.pitchEnabled = pitchToggle.isOn;
        analogStim.rollEnabled = rollToggle.isOn;
        analogStim.yawEnabled = yawToggle.isOn;

        analogStim.apEnabled = apToggle.isOn;
        analogStim.mlEnabled = mlToggle.isOn;
        analogStim.udEnabled = udToggle.isOn;

        analogStim.scaleEnabled = scaleToggle.isOn;
    }
}
