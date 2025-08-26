using UnityEngine;
 public interface IControllerState
{
    void Enter(PlayerController controller);
    void Execute(PlayerController controller);
    void Exit(PlayerController controller);
}