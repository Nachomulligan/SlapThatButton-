using UnityEngine;
using UnityEngine.InputSystem;
public class TrackballInputStrategy : IInputStrategy
{
    private readonly MovementConfig config;
    private readonly bool invertHorizontal;
    private readonly bool invertVertical;
    private readonly float sensitivity;
    private Mouse mouse;

    public TrackballInputStrategy(MovementConfig config, bool invertH = false, bool invertV = true, float sens = 1f)
    {
        this.config = config;
        this.invertHorizontal = invertH;
        this.invertVertical = invertV;
        this.sensitivity = sens;
        mouse = Mouse.current;
    }

    public Vector2 GetMovementInput()
    {
        if (mouse == null) return Vector2.zero;

        // Obtener el delta del mouse (movimiento del trackball)
        Vector2 mouseDelta = mouse.delta.ReadValue() * sensitivity;

        // Aplicar inversión de ejes según configuración local (override de config)
        if (invertVertical)
            mouseDelta.y = -mouseDelta.y;

        if (invertHorizontal)
            mouseDelta.x = -mouseDelta.x;

        // Aplicar dead zone
        if (mouseDelta.magnitude < config.deadZone)
            return Vector2.zero;

        // Normalizar si excede el máximo
        if (mouseDelta.magnitude > 1f)
            mouseDelta.Normalize();

        return mouseDelta;
    }

    public bool GetPrimaryAction()
    {
        return mouse != null && mouse.leftButton.wasPressedThisFrame;
    }

    public bool GetSecondaryAction()
    {
        return mouse != null && mouse.rightButton.wasPressedThisFrame;
    }
}