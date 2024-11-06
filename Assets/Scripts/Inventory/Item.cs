using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item/Create New Item")]
public class Item : ScriptableObject
{
    public int id;
    public string itemName;
    public int value;
    public Sprite icon;
    public ItemType itemType;

    public static explicit operator GameObject(Item v)
    {
        throw new NotImplementedException();
    }

    public enum ItemType
    {
        Health_Pack,
        Cog_Wheel
    }
}
