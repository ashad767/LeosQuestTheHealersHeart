using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneSwitch : MonoBehaviour

{
    public Player player;
    public Vector3 NextRoom;

    public GameObject CameraStuff;

    public string SceneToSwitchToo;

    public bool isSceneSwapper;
    public GameObject E_Key_Object;

    public bool canLeave = true;


    private void Update()
    {
        //Debug.Log(playerCutsceneSprite.transform.position);
    }
    public void CutSceneSceneSwitch()
    {
        player.transform.position = NextRoom;
        CameraStuff.transform.position = player.transform.position;
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && canLeave)
        {
            if (Input.GetKey(KeyCode.E))
            {
                PlayerPrefs.SetFloat("Coins", player.coins);
                PlayerPrefs.SetInt("Sword", player.swordLevel);
                PlayerPrefs.SetInt("Bow", player.bowLevel);
                PlayerPrefs.SetInt("Magic", player.magicLevel);

                SceneManager.LoadScene(SceneToSwitchToo);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isSceneSwapper)
        {
            if (collision.CompareTag("Player"))
            {

                player.transform.position = NextRoom;
            }
        }

        else if (collision.gameObject.CompareTag("Player") && canLeave)
        {   
            E_Key_Object.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            E_Key_Object.SetActive(false);
        }
    }
}
