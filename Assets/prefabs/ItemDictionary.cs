using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemDictionary : MonoBehaviour
{
    public List<Item> ItemPrefabs;
    private Dictionary<int, GameObject> itemDictionary;
    private void Awake()
    {
        itemDictionary = new Dictionary<int, GameObject>();

        for(int i = 0; i < ItemPrefabs.Count; i++)
        {
            if(ItemPrefabs[i] != null)
            {
                ItemPrefabs[i].ID = i + 1;
            }
        }

        foreach(Item item in ItemPrefabs)
        {
            itemDictionary[item.ID] = item.gameObject;
        }
    }

    public GameObject GetItemPrefab(int itemID)
    {
        itemDictionary.TryGetValue(itemID, out GameObject prefab);
        if(prefab == null)
        {
            Debug.LogWarning($"Item with Id {itemID} not found in the Dictionary");
        }
        return prefab;
    }
}
