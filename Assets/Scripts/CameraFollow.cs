using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private Transform target; // Arrastra aquí tu player
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10); // Offset de la cámara
    [SerializeField] private float smoothSpeed = 0.125f; // Suavizado del seguimiento
    [SerializeField] private bool useSmoothing = true;

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        if (useSmoothing)
        {
            // Seguimiento suave
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
        else
        {
            // Seguimiento directo
            transform.position = desiredPosition;
        }

        // Mantener la rotación original de la cámara (estática)
        // No modificamos transform.rotation, por lo que la cámara nunca rota
    }
}