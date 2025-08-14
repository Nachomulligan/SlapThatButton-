using UnityEngine;
using static UnityEngine.UI.ScrollRect;

public static class MovementStrategyFactory
{
    public static IMovementStrategy CreateMovement(MovementType type, float speed, params float[] parameters)
    {
        return type switch
        {
            MovementType.Straight => new StraightMovement(speed),
            MovementType.WalkPause => new WalkPauseMovement(speed,
                parameters.Length > 0 ? parameters[0] : 1f,
                parameters.Length > 1 ? parameters[1] : 0.5f),
            MovementType.Zigzag => new ZigzagMovement(speed,
                parameters.Length > 0 ? parameters[0] : 2f,
                parameters.Length > 1 ? parameters[1] : 2f),
            MovementType.Random => new RandomMovement(speed,
                parameters.Length > 0 ? parameters[0] : 1f),
            _ => new StraightMovement(speed)
        };
    }
}