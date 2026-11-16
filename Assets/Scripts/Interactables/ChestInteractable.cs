
using UnityEngine;

public class ChestInteractable : MonoBehaviour, IInteractable
{
    private bool isOpen = false;

    public bool CanInteract => !isOpen;

    public string InteractionPrompt => "E - Open chest";

    public void Interact(GameObject interactor)
    {
        if (!CanInteract)
            return;

        isOpen = true; //container zostaje otwarty

        Debug.Log(
            $"{name} opened by {interactor.name}!" //debug, kto dokonal interakcji
        );
    }
}
