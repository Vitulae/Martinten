
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Transform target; //cel nad ktorym ma podazac kamera, wiec raczej gracz, ale mozna to wykorzystac np do cutscenek
    [SerializeField] private float smoothTime = 0.15f;

    private Vector3 offset;
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        if (target == null)
        {
            Debug.LogError("Camera target is missing!");
            enabled = false;
            return;
        }

        // pamietanie poczatkowej odleglosci od gracza
        offset = transform.position - target.position;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 targetPosition = target.position + offset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );
    }
}
