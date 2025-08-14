using UnityEngine;

public class ZigzagMovement : IMovementStrategy
{
    private readonly float speed;
    private readonly float amplitude;
    private readonly float frequency;

    private float timeElapsed;

    public ZigzagMovement(float speed, float amplitude = 0.2f, float frequency = 0.2f)
    {
        this.speed = speed;
        this.amplitude = amplitude;
        this.frequency = frequency;
        Reset();
    }

    public void Move(InsectController insect, Rigidbody2D rb)
    {
        timeElapsed += Time.deltaTime;
        float verticalSpeed = Mathf.Sin(timeElapsed * frequency) * amplitude;
        rb.linearVelocity = new Vector2(speed, verticalSpeed);
    }

    public void Reset()
    {
        timeElapsed = 0f;
    }
}