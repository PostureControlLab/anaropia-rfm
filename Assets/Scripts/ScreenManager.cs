using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public GameObject screen;
    public GameObject rectScreen;

    public AnalogInputDropdownUI sreenAnalogDropdownUi;

    private void Start()
    {
        screen.SetActive(false);
        rectScreen.SetActive(false);

        if (ScreenManagerPersistent.UseRectangularScreen)
        {
            rectScreen.SetActive(true);
            sreenAnalogDropdownUi.analogStim = rectScreen.GetComponent<AnalogStim>();
        }
        else
        {
            screen.SetActive(true);
            sreenAnalogDropdownUi.analogStim = screen.GetComponent<AnalogStim>();
        }
    }
}

public static class ScreenManagerPersistent
{
    public static bool UseRectangularScreen { get; set; }
}
