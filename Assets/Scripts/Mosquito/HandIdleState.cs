using UnityEngine;

public class HandIdleState : IHandState
{
    private HandController handController;

    public HandIdleState(HandController controller)
    {
        handController = controller;
    }

    public void Enter()
    {
    }

    public void Exit()
    {
    }

    public void HandleSlap()
    {
        handController.ChangeState(handController.SlapState);
    }
}