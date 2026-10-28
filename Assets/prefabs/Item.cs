using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
public class Item : MonoBehaviour
{
    public int ID;
    public string Name;
    public int Price;

    public virtual void UseItem()
    {
        Debug.Log("Using Item" + Name);
    }
    public virtual void PickUp()
    {
        Sprite itemIcon = GetComponent<Image>().sprite;
        if(ItemPickupUIController.Instance != null)
        {
            ItemPickupUIController.Instance.ShowItemPickUp(Name, itemIcon);
        }
    }
}
