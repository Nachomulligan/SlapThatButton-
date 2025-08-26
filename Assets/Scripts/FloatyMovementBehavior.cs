using UnityEngine;

public class FloatyMovementBehavior : IMovementBehavior
{
    private MovementConfig config;
    private Vector2 currentVelocity;
    private Vector2 targetVelocity;
    private Vector2 velocitySmoothing;

    public void Configure(MovementConfig config)
    {
        this.config = config;
    }

    public void Move(Vector2 input, Transform transform, Rigidbody2D rb)
    {
        if (config == null) return;

        targetVelocity = input * config.maxSpeed;

        float accelerationRate = input.magnitude > 0.01f ?
            config.acceleration : config.deceleration;

        currentVelocity = Vector2.SmoothDamp(
            currentVelocity,
            targetVelocity,
            ref velocitySmoothing,
            config.smoothing / accelerationRate,
            config.maxSpeed
        );

        currentVelocity *= config.driftFactor;

        if (rb != null)
        {
            rb.linearVelocity = currentVelocity;
        }
        else
        {
            transform.position += (Vector3)currentVelocity * Time.fixedDeltaTime;
        }
    }
}