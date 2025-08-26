using UnityEngine;

public class WalkPauseMovement : IMovementStrategy
{
    private readonly float speed;
    private readonly float walkDuration;
    private readonly float pauseDuration;

    private float timer;
    private bool isWalking = true;

    public WalkPauseMovement(float speed, float walkDuration, float pauseDuration)
    {
        this.speed = speed;
        this.walkDuration = walkDuration;
        this.pauseDuration = pauseDuration;
        Reset();
    }

    public void Move(InsectController insect, Rigidbody2D rb)
    {
        timer += Time.deltaTime;

        if (isWalking)
        {
            rb.linearVelocity = Vector2.right * speed;

            if (timer >= walkDuration)
            {
                SwitchToPause();
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;

            if (timer >= pauseDuration)
            {
                SwitchToWalk();
            }
        }
    }

    public void Reset()
    {
        timer = 0f;
        isWalking = true;
    }

    private void SwitchToPause()
    {
        isWalking = false;
        timer = 0f;
    }

    private void SwitchToWalk()
    {
        isWalking = true;
        timer = 0f;
    }
}