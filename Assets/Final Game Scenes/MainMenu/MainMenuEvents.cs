using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuEvents : MonoBehaviour
{
    public GameObject main;
    public GameObject controls;

    public void ExitButtonClick()
    {
#if     UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ControlsButtonClick()
    {
        SceneManager.LoadScene("TrainingGrounds");
    }

    public void BackToMainFromControls()
    {
        controls.SetActive(false);
        main.SetActive(true);
    }

    public void StartGame()
    {
        PlayerPrefs.SetFloat("Coins", 0f);
        PlayerPrefs.SetInt("Sword", 0);
        PlayerPrefs.SetInt("Bow", 0);
        PlayerPrefs.SetInt("Magic", 0);

        SceneManager.LoadScene("StartScene - The House");
    }
}
