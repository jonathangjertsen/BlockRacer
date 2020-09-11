using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Player player;
    private Vector3 offset;

    public string leftKey;
    public string rightKey;
    public string upKey;
    public string downKey;
    public string zoomInKey;
    public string zoomOutKey;
    public string resetKey;

    public Vector3 rotationSpeed;
    public float zoomSpeed;
    public float followSpeed;
    private Quaternion originalRotation;

    private void Start()
    {
        player = FindObjectOfType<Player>();

        offset = transform.position - player.transform.position;
        originalRotation = transform.rotation;
    }

    void Steer(float upDown, float side)
    {
        if (upDown == 0f && side == 0f)
        {
            return;
        }

        transform.RotateAround(
            player.transform.position,
            transform.right,
            rotationSpeed[1] * upDown
        );
        transform.RotateAround(
            player.transform.position,
            transform.up,
            rotationSpeed[2] * side
        );
    }

    void Reset()
    {
        transform.rotation = originalRotation;
    }

    void FixedUpdate()
    {
        // Follow player's x and z but not y
        Vector3 newPosition = player.transform.position + offset;
        newPosition.y = transform.position.y;
        transform.position = newPosition;

        if (player.ShouldFollowYAxis())
        {
            Quaternion toRotation = Quaternion.LookRotation(player.transform.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, followSpeed * Time.fixedDeltaTime);
        }

        // Rotation with arrow keys
        float upDown = 0f;
        float leftRight = 0f;
        if (leftKey != "" && Input.GetKey(leftKey))
        {
            leftRight -= 1f;
        }
        if (rightKey != "" && Input.GetKey(rightKey))
        {
            leftRight += 1f;
        }
        if (downKey != "" && Input.GetKey(downKey))
        {
            upDown -= 1f;
        }
        if (upKey != "" && Input.GetKey(upKey))
        {
            upDown += 1f;
        }
        Steer(upDown, leftRight);

        float zoom = 0;
        if (zoomInKey != "" && Input.GetKey(zoomInKey))
        {
            zoom -= 1f;
        }
        if (zoomOutKey != "" && Input.GetKey(zoomOutKey))
        {
            zoom += 1f;
        }
        if (zoom != 0f)
        {
            offset *= 1 + zoom * zoomSpeed;
        }

        if (Input.GetKey(resetKey))
        {
            Reset();
        }
    }
}
