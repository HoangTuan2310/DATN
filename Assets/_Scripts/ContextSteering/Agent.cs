using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    private AgentAnimations agentAnimations;
    private AgentMover agentMover;
    private WeaponParent weaponParent;

    private Vector2 pointerInput, movementInput;

    public Vector2 MovementInput { get => movementInput; set => movementInput = value; }
    public Vector2 PointerInput  { get => pointerInput;  set => pointerInput  = value; }

    private void Awake()
    {
        agentAnimations = GetComponentInChildren<AgentAnimations>();
        weaponParent    = GetComponentInChildren<WeaponParent>();
        agentMover      = GetComponent<AgentMover>();
    }

    private void Update()
    {
        if (PauseController.IsGamePaused)
        {
            agentMover.MovementInput = Vector2.zero;
            agentAnimations.PlayAnimation(Vector2.zero);
            return;
        }

        agentMover.MovementInput     = MovementInput;
        weaponParent.PointerPosition = pointerInput;

        Vector2 lookDirection = pointerInput - (Vector2)transform.position;
        agentAnimations.RotateToPointer(lookDirection);
        agentAnimations.PlayAnimation(MovementInput);
    }

    public void PerformAttack()
    {
        weaponParent.Attack();
    }
}
