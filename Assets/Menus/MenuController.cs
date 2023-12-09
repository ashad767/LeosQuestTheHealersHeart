using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject player;


    public GameObject mainCamera;
    public GameObject CutsceneCameras;

    private Player playerScript;


    private bool inCutscene;

    private void Start()
    {
        inCutscene = false;
        playerScript = player.GetComponent<Player>();
    }

    void Update()
    {

    }


    public void disablePlayer()
    {
        player.SetActive(false);
    }

    public void disablePlayerForCutscene()
    {
        inCutscene = true;
        disablePlayer();

        mainCamera.SetActive(false);
        CutsceneCameras.SetActive(true);

    }

    public void enablePlayerForCutscene()
    {
        inCutscene = false;
        enablePlayer();

        mainCamera.SetActive(true);
        CutsceneCameras.SetActive(false);

    }
    public void enablePlayer()
    {
        inCutscene = false;
        player.SetActive(true);
    }
}
