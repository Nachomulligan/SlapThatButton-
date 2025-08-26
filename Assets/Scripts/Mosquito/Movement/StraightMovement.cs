using UnityEngine;
public class StraightMovement : IMovementStrategy
{
    private readonly float speed;

    public StraightMovement(float speed)
    {
        this.speed = speed;
    }

    public void Move(InsectController insect, Rigidbody2D rb)
    {
        rb.linearVelocity = Vector2.right * speed;
    }

    public void Reset()
    {
        // No hay estado que resetear en movimiento recto
    }
}