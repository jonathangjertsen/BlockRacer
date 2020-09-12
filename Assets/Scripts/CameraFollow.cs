using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Player player;
    private Vector3 offset;


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

    public void Steer(float upDown)
    {
        if (upDown == 0f)
        {
            return;
        }

        transform.RotateAround(
            player.transform.position,
            transform.right,
            rotationSpeed[1] * upDown
        );
    }

    public void Reset()
    {
        transform.rotation = originalRotation;
    }

    void Update()
    {
        // Follow player's x and z but not y
        Vector3 newPosition = player.transform.position + offset;
        newPosition.y = transform.position.y;
        transform.position = newPosition;

        if (player.ShouldFollowYAxis())
        {
            Quaternion toRotation = Quaternion.LookRotation(player.transform.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, followSpeed * Time.deltaTime);
        }
    }

    public void Zoom(float zoom)
    {
        offset *= 1 + zoom * zoomSpeed;
    }
}
