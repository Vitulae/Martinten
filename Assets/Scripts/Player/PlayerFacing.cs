
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerFacing : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera playerCamera;

    [Header("Rotation Settings")]
    [SerializeField] private float groundHeight = 0f;
    [SerializeField] private float rotationSpeed = 720f;

    private Rigidbody rb;

    private Quaternion targetRotation;
    private bool hasTargetRotation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (playerCamera == null)
            playerCamera = Camera.main;

        if (playerCamera == null)
        {
            Debug.LogError("Camera missing!");
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        if (Mouse.current == null)
            return;

        Vector2 mousePosition =
            Mouse.current.position.ReadValue();

        Ray ray = playerCamera.ScreenPointToRay(mousePosition);

        Plane groundPlane = new Plane(
            Vector3.up,
            new Vector3(0f, groundHeight, 0f)
        );

        if (!groundPlane.Raycast(ray, out float distance))
            return;

        Vector3 mouseWorldPosition = ray.GetPoint(distance);

        Vector3 direction =
            mouseWorldPosition - rb.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        targetRotation = Quaternion.LookRotation(
            direction,
            Vector3.up
        );

        hasTargetRotation = true;
    }

    private void FixedUpdate()
    {
        if (!hasTargetRotation)
            return;

        Quaternion nextRotation = Quaternion.RotateTowards(
            rb.rotation,
            targetRotation,
            rotationSpeed * Time.fixedDeltaTime
        );

        rb.MoveRotation(nextRotation);
    }
}
