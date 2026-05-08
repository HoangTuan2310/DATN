using UnityEngine;

public class InteractionDetector : MonoBehaviour
{
    private IInteractable interactableRange = null;
    public GameObject interactionIcon;

    [SerializeField] private PlayerInput playerInput;

    private void Awake() 
    { 
        playerInput = FindFirstObjectByType<PlayerInput>(); 
    }
    void Start()
    {
        interactionIcon.SetActive(false);
    }

    private void HandleInteract()
    {
        if (interactableRange == null) return;

        if (interactableRange.CanInteract())
        {
            interactableRange.Interact();
        }
    }

    private void OnEnable()
    {
        if (playerInput != null)
            playerInput.OnInteract.AddListener(HandleInteract);
    }

    private void OnDisable()
    {
        if (playerInput != null)
            playerInput.OnInteract.RemoveListener(HandleInteract);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
        {
            interactableRange = interactable;
            interactionIcon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable == interactableRange)
        {
            interactableRange = null;
            interactionIcon.SetActive(false);
        }
    }
}
