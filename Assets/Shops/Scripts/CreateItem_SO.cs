using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item_SO", menuName = "Scriptable Objects/Create New Item SO", order = 1)]
public class CreateItem_SO : ScriptableObject
{
    public Sprite itemImg; // I tried using the Image type, but apparently, I can't drag and drop Images, only Sprites
    public string itemName;
    public string description;
    public int cost;
}
