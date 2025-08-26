using UnityEngine;

public class MovingState : IControllerState
{
    public void Enter(PlayerController controller)
    {
        Debug.Log("Entering Moving State");
    }

    public void Execute(PlayerController controller)
    {
        Vector2 input = controller.InputStrategy.GetMovementInput();

        if (input.magnitude < 0.01f)
        {
            controller.ChangeState(new IdleState());
            return;
        }

        controller.MovementBehavior.Move(input, controller.transform, controller.Rigidbody);
    }

    public void Exit(PlayerController controller)
    {
        Debug.Log("Exiting Moving State");
    }
}