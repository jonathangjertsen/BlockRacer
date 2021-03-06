﻿using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    public event Action OnStart;
    public event Action OnWin;
    public event Action OnDie;
    public event Action OnJump;
    public event Action OnStop;

    [Header("References")]
    public Rigidbody rb;

    [Header("Forward speed")]
    [Tooltip("Target forward velocity for the ball. The ball will speed up or slow down until this speed is reached.")]
    public float targetSpeed;
    [Tooltip("How much forward velocity to add when interacting with boost panels")]
    public float boostImpact;
    [Tooltip("Exponential factor when decelerating")]
    public float deceleration;
    [Tooltip("Exponential factor when accelerating")]
    public float acceleration;

    [Header("Steering")]
    [Tooltip("How much the velocity changes when moving from side to side")]
    public float sidewardForce;
    [Tooltip("How much the friction to apply when breaking")]
    public float sideBreakForce;

    [Header("Jumping")]
    [Tooltip("How much the vertical velocity changes when moving from side to side")]
    public float jumpForce;
    [Tooltip("Fall multiplier based on https://www.youtube.com/watch?v=7KiK0Aqtmzc")]
    public float fallMultiplier;
    [Tooltip("Released jump multiplier based on https://www.youtube.com/watch?v=7KiK0Aqtmzc")]
    public float releasedJumpMultiplier;
    [Tooltip("Mim number of physics frames that must pass between jumps")]
    public int minFramesBetweenJump;
    [Tooltip("Jump velocity will be proportional to (speed/originalTargetSpeed)^speedRatioToJumpForceExponent")]
    public float speedRatioToJumpForceExponent;

    [Header("Smooth rolling")]
    public float groundLockThresholdNeg;
    public float groundLockThresholdPos;
    public float groundHeight;
    [Tooltip("Wile E Coyote term")]
    public int groundCheckMaxCounter;
    [Tooltip("How close the ball must be to a block in order to interact with it")]
    public float maxGroundDistanceForInteraction;

    [Header("Death barrier")]
    [Tooltip("If the ball falls below this plane, we die")]
    public float deathBarrierHeight;
    [Tooltip("If the ball falls below this plane, the camera will start following the ball each frame")]
    public float cameraReleaseHeight;

    [Header("Score calculation")]
    [Tooltip("Score due to speed is speedToScoreFactor * speed ^ speedToScoreExp")]
    public float speedToScoreFactor;
    [Tooltip("Score due to speed is speedToScoreFactor * speed ^ speedToScoreExp")]
    public float speedToScoreExp;
    [Tooltip("How much score to add per coin collected")]
    public float scorePerCoin;

    private float score = 0;
    private bool midAir = false;
    private bool jumpHeld = false;
    private float speed;
    private bool coinCollected = false;
    private int groundCheckCounter = 0;
    private bool steeringAllowed = true;
    private bool jumpedThisFrame = false;
    private float startingTargetSpeed;
    private int framesSinceLastJump;
    private bool properlyStarted = false;

    private static Player instance;

    public static Player Find()
    {
        if (!instance)
        {
            instance = FindObjectOfType<Player>();
        }
        return instance;
    }

    void Start()
    {
        speed = startingTargetSpeed = targetSpeed;
        framesSinceLastJump = 0;
        score = 0;
        properlyStarted = false;
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
        if (!properlyStarted)
        {
            properlyStarted = true;
            OnStart?.Invoke();
        }

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

    public bool JumpAllowed()
    {
        return !midAir && !jumpedThisFrame && (framesSinceLastJump >= minFramesBetweenJump);
    }

    public void Jump()
    {
        if (JumpAllowed())
        {
            UnconditionalJump();
        }
        jumpHeld = true;
    }

    public void ReleaseJump()
    {
        jumpHeld = false;
    }

    void UnconditionalJump()
    {
        OnJump?.Invoke();
        rb.velocity = new Vector3(
            rb.velocity.x,
            jumpForce * Time.fixedDeltaTime * Mathf.Pow(
                speed / startingTargetSpeed,
                speedRatioToJumpForceExponent
            ),
            rb.velocity.z
        );
        midAir = true;
        jumpedThisFrame = true;
        framesSinceLastJump = 0;
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
        OnStop?.Invoke();
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

    public void Move(float x, float z)
    {
        transform.position = new Vector3(x, transform.position.y, z);
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

        float independentSpeedChangeTerm = (speed - targetSpeed) * Time.fixedDeltaTime;

        if (speed < targetSpeed)
        {
            speed -= independentSpeedChangeTerm * acceleration;
        }
        else
        {
            speed -= independentSpeedChangeTerm * deceleration;
        }
    }

    void AdjustVerticalSpeed()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.velocity.y > 0 && !jumpHeld)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (releasedJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    public void Steer(float factor)
    {
        if (!steeringAllowed)
        {
            return;
        }

        rb.AddForce(
            factor * sidewardForce * Time.deltaTime,
            0,
            0,
            ForceMode.VelocityChange
        );
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

    public int GetScore()
    {
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

    void UpdateJumpLimiting()
    {
        framesSinceLastJump++;
    }

    void FixedUpdate()
    {
        AdjustForwardSpeed();
        AdjustVerticalSpeed();
        EnsureSmoothRolling();
        UpdateJumpLimiting();
        CheckGround();
        CheckDeathBarrier();
    }

    private void Update()
    {
        jumpedThisFrame = false;
    }
}
