using UnityEngine;

public class HandSlapState : IHandState
{
    private HandController handController;

    public HandSlapState(HandController controller)
    {
        handController = controller;
    }

    public void Enter()
    {
        handController.StartSlapAnimation();
    }

    public void Exit()
    {
    }

    public void HandleSlap()
    {
        if (!handController.IsSlapping)
        {
            handController.StartSlapAnimation();
        }
    }
}