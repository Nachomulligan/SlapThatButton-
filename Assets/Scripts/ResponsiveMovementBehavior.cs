using UnityEngine;

public class ResponsiveMovementBehavior : IMovementBehavior
{
    private MovementConfig config;
    private Vector2 currentVelocity;

    public void Configure(MovementConfig config)
    {
        this.config = config;
    }

    public void Move(Vector2 input, Transform transform, Rigidbody2D rb)
    {
        if (config == null) return;

        Vector2 targetVelocity = input * config.maxSpeed;

        currentVelocity = Vector2.Lerp(
            currentVelocity,
            targetVelocity,
            Time.fixedDeltaTime * config.acceleration
        );

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