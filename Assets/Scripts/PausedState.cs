using UnityEngine;

public class PausedState : IControllerState
{
    public void Enter(PlayerController controller)
    {
        if (controller.Rigidbody != null)
            controller.Rigidbody.linearVelocity = Vector2.zero;

        Debug.Log("Game Paused");
    }

    public void Execute(PlayerController controller)
    {
    }

    public void Exit(PlayerController controller)
    {
        Debug.Log("Game Resumed");
    }
}