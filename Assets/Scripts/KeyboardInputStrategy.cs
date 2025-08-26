using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardInputStrategy : IInputStrategy
{
    private Keyboard keyboard;

    public KeyboardInputStrategy()
    {
        keyboard = Keyboard.current;
    }

    public Vector2 GetMovementInput()
    {
        if (keyboard == null) return Vector2.zero;

        Vector2 input = Vector2.zero;

        if (keyboard.wKey.isPressed) input.y = 1;
        if (keyboard.sKey.isPressed) input.y = -1;
        if (keyboard.aKey.isPressed) input.x = -1;
        if (keyboard.dKey.isPressed) input.x = 1;

        return input.normalized;
    }

    public bool GetPrimaryAction()
    {
        return keyboard != null && keyboard.spaceKey.wasPressedThisFrame;
    }

    public bool GetSecondaryAction()
    {
        return keyboard != null && keyboard.leftShiftKey.wasPressedThisFrame;
    }
}