
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f; //predkosc zwyklego chodu
    [SerializeField] private float sprintSpeed = 8f; //predkosc sprintu

    [Header("Input")]
    [SerializeField] private InputActionAsset playerControls;

    private Rigidbody rb;
    private InputAction moveAction;
    private InputAction sprintAction;

    private Vector2 moveInput;
    private bool isSprinting;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (playerControls == null)
        {
            Debug.LogError("Player Controls missing!");
            enabled = false;
            return;
        }

        InputActionMap playerMap = playerControls.FindActionMap("Player", true);

        moveAction = playerMap.FindAction("Move", true);
        sprintAction = playerMap.FindAction("Sprint", true);
    }

    private void OnEnable()
    {
        moveAction?.Enable();
        sprintAction?.Enable();
    }

    private void OnDisable()
    {
        moveAction?.Disable();
        sprintAction?.Disable();

        moveInput = Vector2.zero;
        isSprinting = false;

        if (rb != null)
            rb.linearVelocity = Vector3.zero;
    }

    private void Update()
    {
        if (moveAction == null || sprintAction == null)
            return;

        moveInput = moveAction.ReadValue<Vector2>();
        isSprinting = sprintAction.IsPressed();
    }

    private void FixedUpdate()
    {
        Vector3 direction = new Vector3(
            moveInput.x,
            0f,
            moveInput.y
        );
        direction = Vector3.ClampMagnitude(direction, 1f);
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
        rb.linearVelocity = direction * currentSpeed;
    }
}
