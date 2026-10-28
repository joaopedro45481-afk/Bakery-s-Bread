using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SaveController : MonoBehaviour
{
    private string saveLocation;
    private InventoryController inventoryController;
    private HotbarController hotbarController;
    
    void Start()
{
    saveLocation = Path.Combine(Application.persistentDataPath, "saveDataBakery's.json");
    inventoryController = FindObjectOfType<InventoryController>();
    hotbarController = FindObjectOfType<HotbarController>();

    LoadGame();
}

    public void SaveGame()
{
    PlayerMove player = GameObject.FindGameObjectWithTag("Player")
        .GetComponent<PlayerMove>();

    SaveData saveData = new SaveData
    {
        playerPosition = player.transform.position,
        moedasDoPlayer = player.moedas,
        inventorySaveData = inventoryController.GetInventoryItems(),
        hotbarSaveData = hotbarController.GetHotbarItems()
    };

    File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData));
}

  public void LoadGame()
{
    if (File.Exists(saveLocation))
    {
        PlayerMove player = GameObject.FindGameObjectWithTag("Player")
            .GetComponent<PlayerMove>();

        SaveData saveData = JsonUtility.FromJson<SaveData>(
            File.ReadAllText(saveLocation));

        player.transform.position = saveData.playerPosition;
        player.moedas = saveData.moedasDoPlayer;

        inventoryController.SetInventoryItems(saveData.inventorySaveData);
        hotbarController.SetHotbarItems(saveData.hotbarSaveData);
    }
    else
    {
        SaveGame();
    }
}
}
