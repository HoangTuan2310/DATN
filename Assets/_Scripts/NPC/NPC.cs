using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour, IInteractable
{
    public NPCDialogue dialogueData;
    private DialogueController dialogueUI;
    private int dialogueIndex;
    private bool isTyping, isDialogueActive;

    [SerializeField] private DialogueAudio dialogueAudio;

    private enum RequirementState { NotStarted, InProgress, Completed }
    private RequirementState requirementSate = RequirementState.NotStarted;

    private void Start()
    {
        dialogueUI = DialogueController.Instance;
    }

    public bool CanInteract()
    {
        Debug.Log(requirementSate);
        return true;
    }

    public void Interact()
    {
        if (dialogueData == null) return;

        if (isDialogueActive)
            NextLine();
        else
            StartDialogue();
    }


    void StartDialogue()
    {
        SyncRequirementState();

        if (requirementSate == RequirementState.NotStarted)
        {
            dialogueIndex = 0;
        }
        else if (requirementSate == RequirementState.InProgress)
        {
            dialogueIndex = dialogueData.requirementInProgressIndex;
        }
        else if (requirementSate == RequirementState.Completed)
        {
            dialogueIndex = dialogueData.requirementCompletedIndex;
        }
        
        isDialogueActive = true;
        dialogueUI.SetNPCInfo(dialogueData.npcName, dialogueData.npcPortrait);
        dialogueUI.ShowDialogueUI(true);

        DisplayCurrentLine();
    }

    private void SyncRequirementState()
    {
        if (dialogueData.requirement == null) return;

        string requirementID = dialogueData.requirement.requirementID;
        if(RequirementController.Instance.IsRequirementCompleted(requirementID) || RequirementController.Instance.IsRequirementHandedIn(requirementID))
        {
            requirementSate = RequirementState.Completed;
        }
        else if (RequirementController.Instance.IsRequirementActive(requirementID))
        {
            requirementSate = RequirementState.InProgress;
        }
        else
        {
            requirementSate = RequirementState.NotStarted;
        }
    }

    void NextLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueUI.SetDialogueText(dialogueData.dialogueLines[dialogueIndex]);
            isTyping = false;
            dialogueAudio?.StopTypingSound();

            // After skipping, show choices if this line has them
            foreach (DialogueChoice dialogueChoice in dialogueData.choices)
            {
                if (dialogueChoice.dialogueIndex == dialogueIndex)
                {
                    DisplayChoices(dialogueChoice);
                    return;
                }
            }
            return;
        }

        // If choices are currently showing, wait for the player to click one
        if (dialogueUI.choiceContainer.childCount > 0) return;

        dialogueUI.ClearChoices();

        if (dialogueData.endDialogueLines.Length > dialogueIndex && dialogueData.endDialogueLines[dialogueIndex])
        {
            EndDialogue();
            return;
        }

        if (++dialogueIndex < dialogueData.dialogueLines.Length)
        {
            DisplayCurrentLine();
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueUI.SetDialogueText("");

        dialogueAudio?.StartTypingSound();

        foreach (char letter in dialogueData.dialogueLines[dialogueIndex])
        {
            dialogueUI.SetDialogueText(dialogueUI.dialogueText.text += letter);
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        isTyping = false;

        dialogueAudio?.StopTypingSound();

        // After typing finishes, immediately show choices if this line has them
        foreach (DialogueChoice dialogueChoice in dialogueData.choices)
        {
            if (dialogueChoice.dialogueIndex == dialogueIndex)
            {
                DisplayChoices(dialogueChoice);
                yield break; // Don't auto-advance; wait for the player to pick
            }
        }

        if (dialogueData.autoProgressLines.Length > dialogueIndex && dialogueData.autoProgressLines[dialogueIndex])
        {
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);
            NextLine();
        }
    }

    void DisplayChoices(DialogueChoice choice)
    {
        for (int i = 0; i < choice.choices.Length; i++) 
        {
            int nextIndex = choice.nextDialogueIndexes[i];
            bool givesRequirement = choice.givesRequirement[i];
            dialogueUI.CreateChoiceButton(choice.choices[i], () => ChooseOption(nextIndex, givesRequirement));
        }
    }

    void ChooseOption(int nextIndex, bool givesRequirement)
    {
        if (givesRequirement)
        {
            RequirementController.Instance.Accept(dialogueData.requirement);
            requirementSate = RequirementState.InProgress;
        }
        dialogueIndex = nextIndex;
        dialogueUI.ClearChoices();
        DisplayCurrentLine();
    }

    void DisplayCurrentLine()
    {
        StopAllCoroutines();
        StartCoroutine(TypeLine());
    }

    public void EndDialogue()
    {
        if (requirementSate == RequirementState.Completed && !RequirementController.Instance.IsRequirementHandedIn(dialogueData.requirement.requirementID))
        {
            HandleRequirementCompletion(dialogueData.requirement);
        }
        StopAllCoroutines();
        dialogueAudio?.StopTypingSound();
        isDialogueActive = false;
        dialogueUI.SetDialogueText("");
        dialogueUI.ShowDialogueUI(false);
    }

    void HandleRequirementCompletion(Requirement requirement)
    {
        RequirementController.Instance.HandInRequirement(requirement.requirementID);
    }
}
