
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactionRadius = 1.5f; //promien zasiegu interakcji
    [SerializeField] private Transform interactionPoint;

    [Header("Layers")]
    [SerializeField] private LayerMask interactableLayers; //layer z obiektami do interakcji (w inspektorze zmienic na Interactable)
    [SerializeField] private LayerMask obstacleLayers; //layer z reszta obiektow ze srodowiska (zmienic na Environment)

    [Header("Input")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("UI")]
    [SerializeField] private TMP_Text interactionPrompt; //prompt do interakcji

    private InputAction interactAction; //akcja interakcja hehe z InputMapy

    private IInteractable currentInteractable; //obecnie aktywny objekt do interakcji

    private void Awake()
    {
        if (playerControls == null)
        {
            Debug.LogError("Player Controls asset missing!"); //standardowo sprawdzenie czy jest input
            enabled = false;
            return;
        }
        InputActionMap playerMap = playerControls.FindActionMap("Player", true);
        interactAction = playerMap.FindAction("Interact", true);
    }

    private void OnEnable()
    {
        interactAction?.Enable();
    }

    private void OnDisable()
    {
        interactAction?.Disable();
        currentInteractable = null;
        if (interactionPrompt != null)
            interactionPrompt.enabled = false;
    }

    private void Update()
    {
        FindClosestInteractable();
        if (currentInteractable != null &&
            interactAction.WasPressedThisFrame())
        {
            currentInteractable.Interact(gameObject);
        }
    }

    private void FindClosestInteractable()
    {
        currentInteractable = null;
        Vector3 origin = interactionPoint != null ? interactionPoint.position : transform.position;
        Collider[] hits = Physics.OverlapSphere(
            origin,
            interactionRadius,
            interactableLayers,
            QueryTriggerInteraction.Collide
        );
        float nearestDistance = float.PositiveInfinity;
        foreach (Collider hit in hits)
        {
            IInteractable candidate =
                hit.GetComponentInParent<IInteractable>();

            if (candidate == null || !candidate.CanInteract)
                continue;

            Vector3 closestPoint = hit.ClosestPoint(origin);

            float distance =
                (closestPoint - origin).sqrMagnitude;

            if (distance >= nearestDistance)
                continue;

            if (!HasLineOfSight(origin, closestPoint))
                continue;

            nearestDistance = distance;
            currentInteractable = candidate;
        }

        UpdatePrompt();
    }

    private bool HasLineOfSight(Vector3 origin, Vector3 targetPoint)
    {
        Vector3 direction = targetPoint - origin;
        float distance = direction.magnitude;
        if (distance < 0.01f)
            return true;

        return !Physics.Raycast(
            origin,
            direction.normalized,
            distance,
            obstacleLayers,
            QueryTriggerInteraction.Ignore
        );
    }

    private void UpdatePrompt()
    {
        if (interactionPrompt == null)
            return;
        bool showPrompt = currentInteractable != null;
        interactionPrompt.enabled = showPrompt;
        if (showPrompt)
        {
            interactionPrompt.text =
                currentInteractable.InteractionPrompt;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 origin = interactionPoint != null
            ? interactionPoint.position
            : transform.position;

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            origin,
            interactionRadius
        );
    }
}
