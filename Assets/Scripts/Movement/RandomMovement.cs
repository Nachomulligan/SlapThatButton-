using UnityEngine;

public class RandomMovement : IMovementStrategy
{
    private readonly float speed;
    private readonly float changeDirectionInterval;

    private float timer;
    private Vector2 currentDirection = Vector2.right;

    public RandomMovement(float speed, float changeDirectionInterval = 1f)
    {
        this.speed = speed;
        this.changeDirectionInterval = changeDirectionInterval;
        Reset();
    }

    public void Move(InsectController insect, Rigidbody2D rb)
    {
        timer += Time.deltaTime;

        if (timer >= changeDirectionInterval)
        {
            // Cambiar direcci?n aleatoriamente pero manteniendo tendencia hacia la derecha
            float randomY = Random.Range(-0.5f, 0.5f);
            currentDirection = new Vector2(1f, randomY).normalized;
            timer = 0f;
        }

        rb.linearVelocity = currentDirection * speed;
    }

    public void Reset()
    {
        timer = 0f;
        currentDirection = Vector2.right;
    }
}