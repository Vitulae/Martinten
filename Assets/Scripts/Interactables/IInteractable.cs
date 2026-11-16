
using UnityEngine;

public interface IInteractable
{
    bool CanInteract { get; }
    string InteractionPrompt { get; }

    void Interact(GameObject interactor);
}
