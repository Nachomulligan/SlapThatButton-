using UnityEngine;
public interface IMovementStrategy
{
    void Move(InsectController insect, Rigidbody2D rb);
    void Reset(); 
}