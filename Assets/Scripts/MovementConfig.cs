using UnityEngine;

[CreateAssetMenu(fileName = "MovementConfig", menuName = "Movement/Configuration")]
public class MovementConfig : ScriptableObject
{
    [Header("Movement Settings")]
    [Range(1f, 20f)]
    public float maxSpeed = 8f;

    [Range(0.1f, 10f)]
    public float acceleration = 5f;

    [Range(0.1f, 10f)]
    public float deceleration = 6f;

    [Header("Floaty Feel")]
    [Range(0f, 1f)]
    public float smoothing = 0.85f;

    [Range(0f, 2f)]
    public float driftFactor = 0.95f;

    [Header("Input Settings")]
    public bool invertVerticalAxis = true;
    public bool invertHorizontalAxis = false;

    [Range(0.01f, 1f)]
    public float deadZone = 0.1f;

    [Range(0.5f, 2f)]
    public float sensitivity = 1f;
}