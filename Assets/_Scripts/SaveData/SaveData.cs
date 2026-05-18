using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public Vector3 playPosition;
    public List<InventorySaveData> inventorySaveData;
    public List<RequirementProgress> requirementProgressData;
    public List<string> handinRequirementIDs;
    public float volume;
    public bool isMuted;
}
