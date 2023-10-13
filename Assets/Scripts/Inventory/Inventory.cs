using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    public List<InventorySlot> inventory;

    public List<Item> equippedItems;

    // Start is called before the first frame update
    void Start()
    {
        AddItem((int)ItemID.IronSword);
        for(int i = 0; i < 5; i++) AddItem((int)ItemID.HealingPotion);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool AddItem(int itemID)
    {
        Item item = ItemListLoader.Instance.itemList.items[itemID];
        InventorySlot newItem = new InventorySlot();
        newItem.item = item;

        foreach(InventorySlot slot in inventory)
        {
            if(slot.item == item)
            {
                if(slot.count < item.maxStack)
                {
                    slot.count++;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        inventory.Add(newItem);
        return true;
    }

    void RemoveItem(int itemID)
    {
        Item item = ItemListLoader.Instance.itemList.items[itemID];
        foreach (InventorySlot slot in inventory)
        {
            if(slot.item == item)
            {
                if(slot.count > 1)
                {
                    slot.count--;
                }
                else
                {
                    inventory.Remove(slot);
                }
            }
        }
    }

    void EquipItem(int itemID)
    {
        Item item = ItemListLoader.Instance.itemList.items[itemID];
        foreach (InventorySlot slot in inventory)
        {
            if(slot.item == item)
            {
                UnequipItem(slot.item.equipSlot);
                equippedItems.Add(slot.item);
            }
        }
    }

    void UnequipItem(EquipSlot equipSlot)
    {
        foreach(Item item in equippedItems)
        {
            if(item.equipSlot == equipSlot)
            {
                equippedItems.Remove(item);
            }
        }
    }
}
