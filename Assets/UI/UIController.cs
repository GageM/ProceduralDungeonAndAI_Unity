using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIController : MonoBehaviour
{
    public Canvas gameUI;
    public Canvas inventoryUI;

    bool isInventoryOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleInventory(InputAction.CallbackContext context)
    {
        isInventoryOpen = !isInventoryOpen;

        gameUI.gameObject.SetActive(!isInventoryOpen);
        inventoryUI.gameObject.SetActive(isInventoryOpen);
    }
}
