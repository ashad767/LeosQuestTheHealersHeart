using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Item Image Sizes
// Mobility: 74 x 64 ; Increases player mobility
// Coin Multiplier: 102 x 64 ; Increases the value of coins
// Damage Multiplier: 119 x 64 ; Increases the damage inflicted on enemies
// Luck: 102 x 64 ; Increases the chance of finding treasure chests
// Magic: 120 x 64 ; Something about magic

[CreateAssetMenu(fileName = "Upgrade_SO", menuName = "Scriptable Objects/Create New Upgrade SO", order = 2)]
public class CreateUpgrades_SO : ScriptableObject
{
    public Sprite itemImg; // I tried using the Image type, but apparently, I can't drag and drop Images, only Sprites
    public float imgWidth;

    public string itemName;
    public string description;
}
