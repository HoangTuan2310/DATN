using System.IO;
using UnityEngine;

public class SaveController : MonoBehaviour
{
    private string saveLocation;
    private InventoryController inventoryController;
    void Start()
    {
        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");
        inventoryController = Object.FindFirstObjectByType< InventoryController>();
        LoadGame();
    }

    public void SaveGame()
    {
        SaveData saveData = new SaveData
        {
            playPosition = GameObject.FindGameObjectWithTag("Player").transform.position,
            inventorySaveData = inventoryController.GetInventoryItems(),
            requirementProgressData = RequirementController.Instance.activateRequirements,
            handinRequirementIDs = RequirementController.Instance.handinRequirementIDs
        };
        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData));
    }

    public void LoadGame()
    {
        if (File.Exists(saveLocation))
        {
            SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));
            GameObject.FindGameObjectWithTag("Player").transform.position = saveData.playPosition;
            inventoryController.SetInventoryItems(saveData.inventorySaveData);

            RequirementController.Instance.LoadRequirementProgress(saveData.requirementProgressData);
            RequirementController.Instance.handinRequirementIDs = saveData.handinRequirementIDs;
        }
        else
        {
            SaveGame();
        }
    }
}
