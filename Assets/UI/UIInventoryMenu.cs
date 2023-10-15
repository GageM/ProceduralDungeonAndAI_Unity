using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIInventoryMenu : MonoBehaviour
{
    GameObject parent;
    Inventory inventory;

    [SerializeField] Transform content;

    [SerializeField] GameObject invSlotPrefab;

    void Awake()
    {
        parent = transform.parent.gameObject;
        inventory = parent.GetComponent<Inventory>();
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

        foreach(RectTransform child in content)
        {
            Destroy(child.gameObject);
        }

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
        invSlot.GetComponentInChildren<TextMeshProUGUI>().text = slot.item.name;
    }
}
