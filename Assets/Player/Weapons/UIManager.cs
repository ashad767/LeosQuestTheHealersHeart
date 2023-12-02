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

    [SerializeField] private Image[] borders;
    public void UpdatePlayerUI()
    {
        swordIcon.sprite = player.weapons[0, player.indexToWeaponLevel[0]].weaponImage;
        bowIcon.sprite = player.weapons[1, player.indexToWeaponLevel[1]].weaponImage;
        magicIcon.sprite = player.weapons[2, player.indexToWeaponLevel[2]].weaponImage;

        foreach (Image img in borders)
            img.enabled = false;
        borders[player.currentWeaponIndex].enabled = true;
    }

}
