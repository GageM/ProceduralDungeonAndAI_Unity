using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Item
{
    public string name;
    public int maxStack;
    public ItemType type;
    public EquipSlot equipSlot;
    public int value;
    public float damage;
    public float protection;
    public DamageType damageType;

    public void Print()
    {
        Debug.Log("Name: " + name + ", Type: " + type + ", Slot: " + equipSlot + ", Value: " + value + ", Damage: " + damage + ", Protection: " + protection + ", Damage Type: " + damageType);
    }

    public void Drop() { }
    public void PickUp() { }
}

[Serializable]
public class InventorySlot
{
    public Item item;
    public int count = 1;
}

[Serializable]
public class ItemList
{
    public Item[] items;
}

// This enum contains all the item types in the game
public enum ItemType
{
    WEAPON,
    ARMOR,
    POTION,
    SCROLL,
    TREASURE,
    MISC
}

// This enum contains all the slots items can be equipped in
public enum EquipSlot
{ 
    NO_EQUIP,
    HEAD,
    CHEST,
    FEET,
    HANDS,
    WEAPON
}

public enum DamageType
{
    DEFAULT,
    FIRE,
    ICE,
    DECAY,
    HOLY
}

// Update Along With ItemList.json For Easy Inventory Management
public enum ItemID
{
    TestItem,
    IronSword,
    HealingPotion,
    IronHelmet

}


