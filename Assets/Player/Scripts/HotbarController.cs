using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class HotbarController : MonoBehaviour
{
    public GameObject hotbarPanel;
    public GameObject slotPrefab;
    public int slotCount = 10;
    private ItemDictionary itemDictionary;
    private Key[] hotbarKeys;
    public void Awake()
    {
        itemDictionary = FindObjectOfType<ItemDictionary>();

        hotbarKeys = new Key[slotCount];
        for(int i = 0; i < slotCount; i++)
        {
            hotbarKeys[i] = i < 9 ? (Key)((int)Key.Digit1 + i): Key.Digit0;
        }
    }

    // Update is called once per frame
    void Update()
    {
     for(int i = 0; i < slotCount; i++)
        {
            if(Keyboard.current[hotbarKeys[i]].wasPressedThisFrame)//seleciona o item
            {
                //usa o item
                UseItemInSlot(i);
            }
        }   
    }

    void UseItemInSlot(int index)
    {
        Slot slot = hotbarPanel.transform.GetChild(index).GetComponent<Slot>();
        if(slot.currentItem != null)
        {
            Item item = slot.currentItem.GetComponent<Item>();
            item.UseItem();
        }
    }
    
    public List<InventorySaveData> GetHotbarItems()
    {
        List<InventorySaveData> hotbarData = new List<InventorySaveData>();
        foreach(Transform slotTransform in hotbarPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if(slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                hotbarData.Add(new InventorySaveData{itemID = item.ID, slotIndex = slotTransform.GetSiblingIndex()});
            }
        }
        return hotbarData;
    }

     public void SetHotbarItems(List<InventorySaveData> inventorySaveData)
{
    // Remove apenas os itens dos slots, NÃO os slots.
    foreach (Transform slotTransform in hotbarPanel.transform)
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
            Slot slot = hotbarPanel.transform
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
}
