using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIInventoryMenu : MonoBehaviour
{
    GameObject parent;
    Inventory inventory;
    List<GameObject> inventorySlots;

    [SerializeField] Transform content;

    [SerializeField] GameObject invSlotPrefab;

    void Awake()
    {
        parent = transform.parent.gameObject;
        inventory = parent.GetComponent<Inventory>();
        inventorySlots = new();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InventoryOpened()
    {
        // Refresh Inventory When Opened

        foreach(GameObject slot in inventorySlots)
        {
            Destroy(slot);
        }
        inventorySlots.Clear();

        PopulateInventoryUI();
    }

    void PopulateInventoryUI()
    {
        foreach(InventorySlot slot in inventory.inventory)
        {
            AddInventoryItem(slot);
        }
    }

    void AddInventoryItem(InventorySlot slot)
    {
        GameObject invSlot = Instantiate(invSlotPrefab, content);
        inventorySlots.Add(invSlot);
        foreach(var text in invSlot.GetComponentsInChildren<TextMeshProUGUI>())     
        {
            if (text.name == "Text-ItemName") text.text = slot.item.name;
            if (text.name == "Text-ItemType") text.text = slot.item.type.ToString();
            if (text.name == "Text-ItemCount") text.text = slot.count.ToString();
            if (text.name == "Text-ItemValue") text.text = slot.item.value.ToString();
        }
    }
}
