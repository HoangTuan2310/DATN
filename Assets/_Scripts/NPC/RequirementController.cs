using Codice.CM.Client.Differences.Merge;
using System.Collections.Generic;
using UnityEngine;

public class RequirementController : MonoBehaviour
{
    public static RequirementController Instance { get; private set; }
    public List<Requirement.Progress> activateRequirement = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Accept(Requirement requirement)
    {
        if (IsRequirementActive(requirement.requirementID)) return;
        activateRequirement.Add(new Requirement.Progress(requirement));
    }

    public bool IsRequirementActive(string requirementID) => activateRequirement.Exists(r => r.RequirementID == requirementID);

    //public bool CheckInventory()
    //{
    //    Dictionary<int, int> itemCounts = InventoryController.Instance.GetItemCounts();

    //    foreach(Progress progress in activateRequirement)
    //    {
    //        foreach(Objective)
    //    }
    //}
}
