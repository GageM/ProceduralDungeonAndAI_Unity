using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryUIController : MonoBehaviour
{
    bool isInventoryOpen = false;

    public RectTransform canvas;

    public RectTransform slotContainer;

    Inventory inventory;

    List<GameObject> slots;

    public GameObject SlotPrefab;


    // Start is called before the first frame update
    void Awake()
    {
        slots = new();
        inventory = GetComponentInParent<Inventory>();
        canvas.gameObject.SetActive(false);
        UpdateInventory();
        inventory.OnInventoryUpdated.AddListener(UpdateInventory);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;

        if (isInventoryOpen)
        {
            canvas.gameObject.SetActive(true);
        }
        else
        {
            canvas.gameObject.SetActive(false);
        }
    }

    void InitializeSlot(InventorySlot slot)
    {
        GameObject instance = Instantiate(SlotPrefab, slotContainer);
        slots.Add(instance);
        foreach(var child in instance.GetComponentsInChildren<TextMeshProUGUI>())
        {
            if (child.name == "Text_ItemName") child.text = slot.item.name;
            if (child.name == "Text_ItemCount") child.text = slot.count.ToString();
            if (child.name == "Text_ItemType") child.text = slot.item.type.ToString();
            if (child.name == "Text_ItemValue") child.text = slot.item.value.ToString();
        }
    }

    public void UpdateInventory()
    {
        foreach (GameObject slot in slots)
        {
            Destroy(slot);
        }
        slots.Clear();

        foreach (var slot in inventory.inventory)
        {
            InitializeSlot(slot);
        }
    }
}
