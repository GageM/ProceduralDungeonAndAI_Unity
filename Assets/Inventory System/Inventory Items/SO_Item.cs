using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Item", menuName = "Inventory/Item")]
public class SO_Item : ScriptableObject
{
    // The base class already contains a variable for the name

    public int maxStack;
    public ItemType type;
    public EquipSlot equipSlot;
    public int value;
}
