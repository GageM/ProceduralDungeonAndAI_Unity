using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This Singleton Class Contains All The Game's Items
[ExecuteInEditMode]
public class ItemListLoader : MonoBehaviour
{
    TextAsset itemFile;


    [SerializeField, Tooltip("A List Of Every Item In The Game")]
    public ItemList itemList;

    public static ItemListLoader Instance { get; private set; }

    private void OnEnable()
    {
        Instance = this;

        itemFile = Resources.Load<TextAsset>("ItemList");

        itemList = JsonUtility.FromJson<ItemList>(itemFile.text);
    }

    private void Awake()
    {

    }
}