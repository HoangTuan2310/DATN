using System.Collections.Generic;
using UnityEngine;

public class RequirementController : MonoBehaviour
{
    public static RequirementController Instance { get; private set; }
    public List<RequirementProgress> activateRequirements = new();

    public List<string> handinRequirementIDs = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        InventoryController.Instance.OnInventoryChanged += CheckInventoryForRequirement;
    }

    public void Accept(Requirement requirement)
    {
        if (IsRequirementActive(requirement.requirementID)) return;
        activateRequirements.Add(new RequirementProgress(requirement));
        CheckInventoryForRequirement();
    }

    public bool IsRequirementActive(string requirementID) => activateRequirements.Exists(r => r.RequirementID == requirementID);

    public void CheckInventoryForRequirement()
    {
        Dictionary<int, int> itemCounts = InventoryController.Instance.GetItemCounts();

        foreach (RequirementProgress requirement in activateRequirements)
        {
            foreach (RequirementObjective requirementObjective in requirement.objectives)
            {
                if (requirementObjective.type != ObjectiveType.CollectItem) continue;
                if (!int.TryParse(requirementObjective.objectiveID, out int itemID)) continue;

                int newAmount = itemCounts.TryGetValue(itemID, out int count) ? Mathf.Min(count, requirementObjective.requiredAmount) : 0;

                if (requirementObjective.currentAmount != newAmount)
                {
                    requirementObjective.currentAmount = newAmount;
                }
            }
        }
    }

    public bool IsRequirementCompleted(string requirementID)
    {
        RequirementProgress requirement = activateRequirements.Find(r => r.RequirementID == requirementID);
        return requirement != null && requirement.objectives.TrueForAll(o => o.IsCompleted);
    }

    public void HandInRequirement(string requirementID)
    {
        if (!RemoveRequiredItemsFromInventory(requirementID))
        {
            return;
        }
        RequirementProgress requirement = activateRequirements.Find(r => r.RequirementID == requirementID);
        if (requirement != null)
        {
            handinRequirementIDs.Add(requirementID);
            activateRequirements.Remove(requirement);

        }
    }

    public bool IsRequirementHandedIn(string requirementID)
    {
        return handinRequirementIDs.Contains(requirementID);
    }

    public bool RemoveRequiredItemsFromInventory(string requirementID)
    {
        RequirementProgress requirement = activateRequirements.Find(r => r.RequirementID == requirementID);
        if (requirement == null) return false;

        Dictionary<int, int> requiredItems = new();

        foreach(RequirementObjective objective in requirement.objectives)
        {
            if (objective.type == ObjectiveType.CollectItem && int.TryParse(objective.objectiveID, out int itemID))
            {
                requiredItems[itemID] = objective.requiredAmount;
            }
        }

        Dictionary<int, int> itemCounts = InventoryController.Instance.GetItemCounts();
        foreach(var item in requiredItems)
        {
            if (itemCounts.GetValueOrDefault(item.Key) < item.Value)
            {
                return false;
            }
        }
        foreach(var itemRequirement in requiredItems)
        {
            InventoryController.Instance.RemoveItemsFromInventory(itemRequirement.Key, itemRequirement.Value);
        }
        return true;
    }

    public void LoadRequirementProgress(List<RequirementProgress> savedRequirement)
    {
        activateRequirements = savedRequirement ?? new();

        CheckInventoryForRequirement();
    }
}
