using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUi : MonoBehaviour
{
    public Button loadLivingRoomButton;
    public Button loadLivingRoomRectScreenButton;
    public Button loadLabButton;
    public Button loadLabRectScreenButton;


    private void Start()
    {
        loadLivingRoomButton.onClick.AddListener(() =>
        {
            ScreenManagerPersistent.UseRectangularScreen = false;
            SceneManager.LoadScene("LivingRoom");
        });
        loadLabButton.onClick.AddListener(() =>
        {
            ScreenManagerPersistent.UseRectangularScreen = false;
            SceneManager.LoadScene("Lab");
        });

        loadLivingRoomRectScreenButton.onClick.AddListener(() =>
        {
            ScreenManagerPersistent.UseRectangularScreen = true;
            SceneManager.LoadScene("LivingRoom");
        });
        loadLabRectScreenButton.onClick.AddListener(() =>
        {
            ScreenManagerPersistent.UseRectangularScreen = true;
            SceneManager.LoadScene("Lab");
        });
    }
}
