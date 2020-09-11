using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody rb;

    public float targetSpeed = 20f;
    public float sidewardForce = 100f;
    public float jumpForce = 100f;
    public string leftKey = "a";
    public string rightKey = "d";
    public string sideBreakKey = "s";
    public string jumpKey = "space";
    public string restartKey = "r";

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
    private GameControl gameControl = null;
    private bool steeringAllowed = true;

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

    void Jump()
    {
        if (!midAir)
        {
            UnconditionalJump();
        }
    }

    void UnconditionalJump()
    {
        rb.AddForce(0, jumpForce, 0, ForceMode.Impulse);
        midAir = true;
    }

    void Boost()
    {
        speed += boostImpact;
    }

    void CollectCoin()
    {
        coinCollected = true;
    }

    void Stop()
    {
        targetSpeed = 0f;
        steeringAllowed = false;
    }

    void Win()
    {
        GetGameControl().Win();
        Stop();
    }

    void CheckGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, new Vector3(0, -1, 0), out hit))
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

    GameControl GetGameControl()
    {
        if (gameControl == null)
        {
            gameControl = FindObjectOfType<GameControl>();
        }
        return gameControl;
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
        GetGameControl().GameOver();
        Stop();
    }

    public void Move(float x)
    {
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }

    void Restart()
    {
        GetGameControl().Restart();
        Stop();
    }

    void SideBreak()
    {
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

    void Steer(float factor)
    {
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

    void HandleInputs()
    {
        if (Input.GetKey(restartKey))
        {
            Restart();
        }

        if (steeringAllowed)
        {
            if (Input.GetKey(sideBreakKey))
            {
                SideBreak();
            }
            if (Input.GetKey(leftKey))
            {
                Steer(-1);
            }
            if (Input.GetKey(rightKey))
            {
                Steer(+1);
            }
            if (Input.GetKey(jumpKey))
            {
                Jump();
            }
        }
    }

    void FixedUpdate()
    {
        HandleInputs();
        AdjustForwardSpeed();
        EnsureSmoothRolling();
        CheckGround();
        CheckDeathBarrier();
    }
}
