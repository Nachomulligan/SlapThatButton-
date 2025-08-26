using UnityEngine;

public interface IMovementBehavior
{
    void Move(Vector2 input, Transform transform, Rigidbody2D rb);
    void Configure(MovementConfig config);
}