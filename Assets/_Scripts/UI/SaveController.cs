using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveController : MonoBehaviour
{
    private string saveLocation;
    private InventoryController inventoryController;

    void Start()
    {
        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");
        inventoryController = FindFirstObjectByType<InventoryController>();

        if (File.Exists(saveLocation))
        {
            SaveData data = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));
            inventoryController.SetInventoryItems(data.inventorySaveData);
            RequirementController.Instance.LoadRequirementProgress(data.requirementProgressData);
            RequirementController.Instance.handinRequirementIDs = data.handinRequirementIDs;
            if (AudioController.Instance != null)
                AudioController.Instance.ApplyAudioSaveData(data.volume, data.isMuted);
        }
        else
        {
            inventoryController.SetInventoryItems(new List<InventorySaveData>());
            File.WriteAllText(saveLocation, JsonUtility.ToJson(new SaveData
            {
                inventorySaveData = new List<InventorySaveData>(),
                requirementProgressData = new List<RequirementProgress>(),
                handinRequirementIDs = new List<string>(),
                volume = 1f,
                isMuted = false
            }));
        }
    }

    public void SaveGame()
    {
        float volume = 1f;
        bool muted = false;
        if (AudioController.Instance != null)
            AudioController.Instance.GetAudioSaveData(out volume, out muted);

        File.WriteAllText(saveLocation, JsonUtility.ToJson(new SaveData
        {
            inventorySaveData = inventoryController.GetInventoryItems(),
            requirementProgressData = RequirementController.Instance.activateRequirements,
            handinRequirementIDs = RequirementController.Instance.handinRequirementIDs,
            volume = volume,
            isMuted = muted
        }));
    }

    public void LoadGame()
    {
        if (!File.Exists(saveLocation)) return;
        SaveData data = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));
        inventoryController.SetInventoryItems(data.inventorySaveData);
        RequirementController.Instance.LoadRequirementProgress(data.requirementProgressData);
        RequirementController.Instance.handinRequirementIDs = data.handinRequirementIDs;
        if (AudioController.Instance != null)
            AudioController.Instance.ApplyAudioSaveData(data.volume, data.isMuted);
    }
}
