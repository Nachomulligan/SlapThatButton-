using UnityEngine;
public interface IInputStrategy
{
    Vector2 GetMovementInput();
    bool GetPrimaryAction();
    bool GetSecondaryAction();
}