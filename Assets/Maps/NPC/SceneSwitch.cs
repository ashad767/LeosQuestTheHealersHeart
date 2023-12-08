using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSwitch : MonoBehaviour

{
    public Player player;
    public GameObject playerCutsceneSprite;
    public Vector3 NextRoom;

    public void CutSceneSceneSwitch()
    {
        player.transform.position = NextRoom;
        playerCutsceneSprite.transform.position = NextRoom;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
        {

            player.transform.position = NextRoom;
        }
    }
}
