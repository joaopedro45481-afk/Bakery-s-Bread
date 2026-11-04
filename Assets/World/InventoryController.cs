using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class InventoryController : MonoBehaviour
{
    private ItemDictionary itemDictionary;

    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public int slotCount;
    public GameObject[] itensPrefabs;
    void Start()
    {
        itemDictionary = FindObjectOfType<ItemDictionary>();


        for(int i = 0; i < slotCount; i++)
        {
            Slot slot = Instantiate(slotPrefab, inventoryPanel.transform).GetComponent<Slot>();
            if(i < itensPrefabs.Length)
            {
                GameObject item = Instantiate(itensPrefabs[i], slot.transform);
                Debug.Log($"Instanciado: {item.name}");
                Debug.Log($"Pai: {item.transform.parent.name}");
                Debug.Log($"Filhos do slot: {slot.transform.childCount}");
                item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.currentItem = item;
            }
        }
    }

    public bool addItem(GameObject itemPrefab)
    {
        foreach(Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if(slot != null && slot.currentItem == null)
            {
                GameObject newItem = Instantiate(itemPrefab, slot.transform);
                newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.currentItem = newItem;
                return true;
            }
        }

        Debug.Log("Inventory is Full");
        return false;
    }


    public void SetInventoryItems(List<InventorySaveData> inventorySaveData)
{
    // Remove apenas os itens dos slots, NÃO os slots.
    foreach (Transform slotTransform in inventoryPanel.transform)
    {
        Slot slot = slotTransform.GetComponent<Slot>();

        if (slot.currentItem != null)
        {
            Destroy(slot.currentItem);
            slot.currentItem = null;
        }
    }

    // Coloca os itens salvos de volta nos slots corretos.
    foreach (InventorySaveData data in inventorySaveData)
    {
        if (data.slotIndex >= 0 && data.slotIndex < slotCount)
        {
            Slot slot = inventoryPanel.transform
                .GetChild(data.slotIndex)
                .GetComponent<Slot>();

            GameObject itemPrefab = itemDictionary.GetItemPrefab(data.itemID);

            if (itemPrefab != null)
            {
                GameObject item = Instantiate(itemPrefab, slot.transform);
                item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.currentItem = item;
            }
            else
            {
                Debug.LogError($"Prefab do item {data.itemID} não encontrado.");
            }
        }
    }
}

public bool RemoveItem(int itemID)
{
    foreach(Transform slotTransform in inventoryPanel.transform)
    {
        Slot slot = slotTransform.GetComponent<Slot>();

        if(slot.currentItem != null)
        {
            Item item = slot.currentItem.GetComponent<Item>();

            if(item.ID == itemID)
            {
                Destroy(slot.currentItem);
                slot.currentItem = null;

                return true;
            }
        }
    }

    return false;
}

    public List<InventorySaveData> GetInventoryItems()
    {
        List<InventorySaveData> InvData = new List<InventorySaveData>();
        foreach(Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if(slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                InvData.Add(new InventorySaveData{itemID = item.ID, slotIndex = slotTransform.GetSiblingIndex()});
            }
        }
        return InvData;
    }
    
        public bool HasItem(int itemID)
    {
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();

            if (slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                if (item.ID == itemID)
                    return true;
            }
        }
        return false;
    }
}
