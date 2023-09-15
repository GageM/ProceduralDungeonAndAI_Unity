using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnMenuController : MonoBehaviour
{
    [SerializeField, Tooltip("The Spawn Menu Canvas")]
    GameObject spawnMenu;
    Character playerCharacter;

    // Start is called before the first frame update
    void Start()
    {
        spawnMenu.SetActive(false);
        playerCharacter = GetComponent<Character>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
             spawnMenu.SetActive(!spawnMenu.activeInHierarchy);
            if (spawnMenu.activeInHierarchy)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                Time.timeScale = 0;
                playerCharacter.disableInput = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Time.timeScale = 1;
                playerCharacter.disableInput = false;
            }
        }
    }
}
