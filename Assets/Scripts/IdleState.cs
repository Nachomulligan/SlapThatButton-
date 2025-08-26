using UnityEngine;

public class IdleState : IControllerState
{
    public void Enter(PlayerController controller)
    {

        Debug.Log("Entering Idle State");
    }

    public void Execute(PlayerController controller)
    {
        Vector2 input = controller.InputStrategy.GetMovementInput();

        if (input.magnitude > 0.01f)
        {
            controller.ChangeState(new MovingState());
        }

        controller.MovementBehavior.Move(Vector2.zero, controller.transform, controller.Rigidbody);
    }

    public void Exit(PlayerController controller)
    {
        Debug.Log("Exiting Idle State");
    }
}