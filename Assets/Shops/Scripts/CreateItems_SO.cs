using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


[CreateAssetMenu(fileName = "Item_SO", menuName = "Scriptable Objects/Create New Items SO", order = 1)]
public class CreateItems_SO : ScriptableObject
{
    public Sprite itemImg; // I tried using the Image type, but apparently, I can't drag and drop Images, only Sprites
    public string itemName;
    public int itemCost;
}
