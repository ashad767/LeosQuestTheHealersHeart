using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject TabMenu;
    public GameObject player;

    public Text playerInfo;
    public Text swordLvl;
    public Text bowLvl;
    public Text magicLvl;

    private Player playerScript;

    private void Start()
    {
        playerScript = player.GetComponent<Player>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            player.SetActive(!player.activeInHierarchy);
            TabMenu.SetActive(!TabMenu.activeInHierarchy);

            if (Time.timeScale != 1)
            {
                Time.timeScale = 1;
            }
            else
            {
                Time.timeScale = 0;
            }
        }

        playerInfo.text = "Available Points: " + playerScript.skillPoints.ToString() +"\nAvailable Coins: " + playerScript.coins.ToString();

        swordLvl.text = (playerScript.swordLevel + 1).ToString();
        bowLvl.text = (playerScript.bowLevel + 1).ToString();
        magicLvl.text = (playerScript.magicLevel + 1).ToString();
    }
}
