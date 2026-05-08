using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(menuName = "Requirement")]
public class Requirement : ScriptableObject
{
    public string requirementID;
    public string requirementName;
    public string description;
    public List<Objectives> objectives;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(requirementID))
        {
            requirementID = requirementName + Guid.NewGuid().ToString();
        }
    }
    [System.Serializable]
    public class Objectives
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
    public class Progress
    {
        public Requirement requirement;
        public List<Objectives> objectives;
        public Progress(Requirement requirement)
        {
            this.requirement = requirement;
            objectives = new List<Objectives>();

            foreach (var obj in requirement.objectives)
            {
                objectives.Add(new Objectives
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
}
