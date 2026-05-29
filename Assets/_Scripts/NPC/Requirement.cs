using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Requirement;


[CreateAssetMenu(menuName = "Requirement")]
public class Requirement : ScriptableObject
{
    public string requirementID;
    public string requirementName;
    public string description;
    public List<RequirementObjective> objectives;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(requirementID))
        {
            requirementID = requirementName + Guid.NewGuid().ToString();
        }
    }
    
}
[System.Serializable]
public class RequirementObjective
{
    public string objectiveID;
    public string description;
    public ObjectiveType type;
    public int requiredAmount;
    public int currentAmount;

    public bool IsCompleted => currentAmount >= requiredAmount;
}

public enum ObjectiveType { CollectItem, DefeatEnemy, ReachLocation, TalkNPC, Custom }

[System.Serializable]
public class RequirementProgress
{
    public Requirement requirement;
    public List<RequirementObjective> objectives;
    public RequirementProgress(Requirement requirement)
    {
        this.requirement = requirement;
        objectives = new List<RequirementObjective>();

        foreach (var obj in requirement.objectives)
        {
            objectives.Add(new RequirementObjective
            {
                objectiveID = obj.objectiveID,
                description = obj.description,
                type = obj.type,
                requiredAmount = obj.requiredAmount,
                currentAmount = 0
            });
        }
    }

    public bool IsCompleted => objectives.TrueForAll(o => o.IsCompleted);

    public string RequirementID => requirement.requirementID;
}
