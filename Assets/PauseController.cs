using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    public GameObject player;
    public GameObject pauseMenu;

    private bool previousState = false;
    private bool pause = false;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if(pause)
            {
                Unpause();
            }
            else
            {
                pause = true;
                Time.timeScale = 0;

                previousState = player.activeInHierarchy;

                player.SetActive(false);
                pauseMenu.SetActive(true);
            }
        }
    }

    public void Unpause()
    {
        pause = false;
        Time.timeScale = 1;

        pauseMenu.SetActive(false);
        player.SetActive(previousState);
    }

    public void Quit()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Main Menu");
    }
}
