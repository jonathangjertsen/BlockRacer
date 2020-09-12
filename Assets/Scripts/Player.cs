using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    public Rigidbody rb;
    public event Action OnWin;
    public event Action OnDie;

    public float targetSpeed = 20f;
    public float sidewardForce = 100f;
    public float jumpForce = 100f;

    public float groundLockThresholdNeg = 0.1f;
    public float groundLockThresholdPos = 0.1f;
    public float groundHeight = 2.0f;
    public float deathBarrierHeight = -3.0f;
    public float cameraReleaseHeight = -3.0f;

    public float boostImpact = 10f;
    public float deceleration = 1f;
    public float sideBreakForce = 0.1f;

    public float speedToScoreFactor = 0.01f;
    public float speedToScoreExp = 3.4f;
    public float scorePerCoin = 500f;
    public int groundCheckMaxCounter = 10;
    public float maxGroundDistanceForInteraction = 2f;

    private float score = 0;
    private bool midAir = false;
    private float speed = 0;
    private bool coinCollected = false;
    private int groundCheckCounter = 0;
    private bool steeringAllowed = true;
    private bool jumpedThisFrame = false;

    void Start()
    {
        speed = 0;
        score = 0;
    }

    private void EnsureSmoothRolling()
    {
        if (midAir)
        {
            return;
        }

        float height = transform.position.y;
        if ((height > groundHeight - groundLockThresholdNeg) && (height < groundHeight + groundLockThresholdPos))
        {
            transform.position = new Vector3(
                transform.position.x,
                groundHeight,
                transform.position.z
            );
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        midAir = false;
    }

    void InteractWithGround(Ground ground)
    {
        if (ground == null || ground.Touched())
        {
            return;
        }

        if (ground is JumpGround)
        {
            UnconditionalJump();
        }

        if (ground is SpeedGround)
        {
            Boost();
        }

        if (ground is CoinGround)
        {
            CollectCoin();
        }

        if (ground is GoalGround)
        {
            Win();
        }

        ground.Touch();
    }

    public void Jump()
    {
        if (!midAir && !jumpedThisFrame)
        {
            UnconditionalJump();
        }
    }

    void UnconditionalJump()
    {
        rb.velocity = new Vector3(
            rb.velocity.x,
            jumpForce * Time.fixedDeltaTime,
            rb.velocity.z
        );
        midAir = true;
        jumpedThisFrame = true;
    }

    void Boost()
    {
        speed += boostImpact;
    }

    void CollectCoin()
    {
        coinCollected = true;
    }

    public void Stop()
    {
        targetSpeed = 0f;
        steeringAllowed = false;
    }

    void Win()
    {
        Stop();
        OnWin?.Invoke();
    }

    void CheckGround()
    {
        if (Physics.Raycast(transform.position, new Vector3(0, -1, 0), out RaycastHit hit))
        {
            if (hit.distance < maxGroundDistanceForInteraction)
            {
                groundCheckCounter = 0;
                InteractWithGround(hit.collider.GetComponent<Ground>());
            }
            else
            {
                groundCheckCounter += 1;
            }
        }
        else
        {
            groundCheckCounter += 1;
        }

        if (groundCheckCounter > groundCheckMaxCounter)
        {
            midAir = true;
        }
    }

    void CheckDeathBarrier()
    {
        if (transform.position.y < deathBarrierHeight)
        {
            Die();
        }
    }

    void Die()
    {
        Stop();
        OnDie?.Invoke();
    }

    public void Move(float x)
    {
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }

    public void SideBreak()
    {
        if (!steeringAllowed)
        {
            return;
        }

        rb.velocity = new Vector3(
            rb.velocity.x * (1 - sideBreakForce),
            rb.velocity.y,
            rb.velocity.z
        );
    }

    void AdjustForwardSpeed()
    {
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, speed);
        speed -= (speed - targetSpeed) * deceleration;
    }

    public void Steer(float factor)
    {
        if (!steeringAllowed)
        {
            return;
        }

        rb.AddForce(factor * sidewardForce * Time.fixedDeltaTime, 0, 0, ForceMode.VelocityChange);
    }

    public int UpdateScore()
    {
        score += Mathf.Pow(speed, speedToScoreExp) * speedToScoreFactor;
        if (coinCollected)
        {
            score += scorePerCoin;
            coinCollected = false;
        }
        return (int)score;
    }

    public int GetSpeed()
    {
        return (int)speed;
    }

    public bool ShouldFollowYAxis()
    {
        return transform.position.y < cameraReleaseHeight;
    }

    void FixedUpdate()
    {
        AdjustForwardSpeed();
        EnsureSmoothRolling();
        CheckGround();
        CheckDeathBarrier();
    }

    private void Update()
    {
        jumpedThisFrame = false;
    }
}
