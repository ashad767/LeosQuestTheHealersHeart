using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]private Player player;
    [Space]
    [SerializeField] private Image swordIcon;
    [SerializeField] private Image bowIcon;
    [SerializeField] private Image magicIcon;

    [SerializeField] private Image healthBar;
    [SerializeField] private Image shieldBar;
    [SerializeField] private Image energyBar;

    [SerializeField] private Text Quests;
    private string currentQuests;



    [SerializeField] private Image[] borders;
    public void UpdatePlayerUI()
    {
        swordIcon.sprite = player.weapons[0, player.indexToWeaponLevel[0]].weaponImage;
        bowIcon.sprite = player.weapons[1, player.indexToWeaponLevel[1]].weaponImage;
        magicIcon.sprite = player.weapons[2, player.indexToWeaponLevel[2]].weaponImage;

        foreach (Image img in borders)
            img.enabled = false;
        borders[player.currentWeaponIndex].enabled = true;

        if(player.currentShield > 0)
        {
            shieldBar.gameObject.SetActive(true);
            shieldBar.rectTransform.sizeDelta = new Vector2(player.currentShield / player.MaxShield * 93, 46.5929f);
        }
        else
        {
            shieldBar.gameObject.SetActive(false);
        }
        
        healthBar.rectTransform.sizeDelta= new Vector2(player.GetHealth() / player.maxHealth * 327, 46.5929f);
        energyBar.rectTransform.sizeDelta = new Vector2(player.GetEnergy() / player.MaxEnergy * 327, 46.6839f);

        currentQuests = "";
        foreach(KeyValuePair<string, string> quest in player.quests)
        {
            currentQuests += quest.Value + "\n";
        }

        Quests.text = currentQuests;
    }

}
