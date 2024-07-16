using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriggerManager : MonoBehaviour
{
    public Toggle triggerToggle;
    public bool triggerEnabled = false;

    private void Start()
    {
        var moveStims = FindObjectsOfType<MoveStim>();

        foreach (var moveStim in moveStims)
        {
            moveStim.useTrigger = triggerEnabled;
        }

        triggerToggle.SetIsOnWithoutNotify(triggerEnabled);
        triggerToggle.onValueChanged.AddListener(value =>
        {
            foreach (var moveStim in moveStims)
            {
                moveStim.useTrigger = value;
            }
        });
    }
}
