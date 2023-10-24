using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour, I_Interactable
{
    public bool canInteract = false;

    public List<InventorySlot> inventory;

    public List<SO_Item> equippedItems;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool AddItem(SO_Item item)
    {
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

    void RemoveItem(SO_Item item)
    {
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

    void EquipItem(SO_Item item)
    {
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
        foreach(SO_Item item in equippedItems)
        {
            if(item.equipSlot == equipSlot)
            {
                equippedItems.Remove(item);
            }
        }
    }

    public void Interact()
    {
        if (canInteract)
        {
            Debug.Log(name + "Inventory:");
            foreach (InventorySlot slot in inventory)
            {
                Debug.Log(slot.item.name);
            }
        }
    }

}
