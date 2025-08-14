using UnityEngine;

public class ZigzagMovement : IMovementStrategy
{
    private readonly float speed;
    private readonly float amplitude;
    private readonly float frequency;

    private float timeElapsed;

    public ZigzagMovement(float speed, float amplitude = 2f, float frequency = 2f)
    {
        this.speed = speed;
        this.amplitude = amplitude;
        this.frequency = frequency;
        Reset();
    }

    public void Move(InsectController insect, Rigidbody2D rb)
    {
        timeElapsed += Time.deltaTime;

        float verticalOffset = Mathf.Sin(timeElapsed * frequency) * amplitude;
        Vector2 direction = new Vector2(1f, verticalOffset).normalized;

        rb.linearVelocity = direction * speed;
    }

    public void Reset()
    {
        timeElapsed = 0f;
    }
}