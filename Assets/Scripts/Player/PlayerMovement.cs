
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Setininging")]
    [SerializeField] private float walkSpeed = 5f; //predkosc poruszania sie
    [SerializeField] private float sprintSpeed = 8f;

    [Header("Inputto")]
    [SerializeField] private InputActionAsset playerControls;

    private Rigidbody rb;
    private InputAction moveAction;
    private InputAction sprintAction;

    private Vector2 moveInput; //wektor odpowiadajacy za ruch na przestrzeni XZ, ruch na osi Y jest zablokowany
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
        moveAction = playerControls.FindActionMap("Player", true).FindAction("Move", true);
        sprintAction = playerControls.FindActionMap("Player", true).FindAction("Move", true);
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

        direction = Vector3.ClampMagnitude(direction, 1f); //zwalniamy i wygladzamy ruch
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
        rb.linearVelocity = direction * currentSpeed;
    }
}
