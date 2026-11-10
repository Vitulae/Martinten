
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Setininging")]
    [SerializeField] private float moveSpeed = 3f; //predkosc poruszania sie

    [Header("Inputto")]
    [SerializeField] private InputActionAsset playerControls;

    private Rigidbody rb;
    private InputAction moveAction;

    private Vector2 moveInput; //wektor odpowiadajacy za ruch na przestrzeni XZ, ruch na osi Y jest zablokowany

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (playerControls == null)
        {
            Debug.LogError("Player Controls asset is missing!");
            enabled = false;
            return;
        }

        moveAction = playerControls.FindActionMap("Player", true).FindAction("Move", true);
    }

    private void OnEnable()
    {
        moveAction?.Enable();
    }

    private void OnDisable()
    {
        moveAction?.Disable();

        moveInput = Vector2.zero;

        if (rb != null)
            rb.linearVelocity = Vector3.zero;
    }

    private void Update()
    {
        if (moveAction == null)
            return;

        moveInput = moveAction.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Vector3 direction = new Vector3(
            moveInput.x,
            0f,
            moveInput.y
        );

        direction = Vector3.ClampMagnitude(direction, 1f); //zwalniamy i wygladzamy ruch
        rb.linearVelocity = direction * moveSpeed;
    }
}
