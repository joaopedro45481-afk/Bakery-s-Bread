using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionSystem : MonoBehaviour
{
    private IInteractable currentInteractable;

    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            if(currentInteractable != null)
            {
                currentInteractable.Interact();
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        IInteractable interactable = collision.GetComponent<IInteractable>();

        if(interactable != null)
        {
            currentInteractable = interactable;
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        IInteractable interactable = collision.GetComponent<IInteractable>();

        if(interactable != null)
        {
            currentInteractable = null;
        }
    }
}