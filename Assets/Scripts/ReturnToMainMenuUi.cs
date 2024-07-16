using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReturnToMainMenuUi : MonoBehaviour
{
    public Button returnToMainMenuButton;


    private void Start()
    {
        returnToMainMenuButton.onClick.AddListener(() =>
        {
            LoadMainMenu();
        });
    }

    public static void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
