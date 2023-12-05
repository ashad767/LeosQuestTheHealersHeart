using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Item Image Sizes
// Mobility: 74 x 64 
// CM: 102 x 64
// Damage: 119 x 64
// Luck: 102 x 64
// Ring/Magic: 120 x 64

[CreateAssetMenu(fileName = "Item_SO", menuName = "Scriptable Objects/Create New Item SO", order = 1)]
public class CreateItem_SO : ScriptableObject
{
    public Sprite itemImg; // I tried using the Image type, but apparently, I can't drag and drop Images, only Sprites
    public float imgWidth;

    public string itemName;
    public string description;
    public int pointerToNextUpgradeBar;
    public int cost;
}
