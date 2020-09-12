using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Player player;
    private Vector3 offset;
    public int zoomFrames;
    public int tiltFrames;
    public float smoothingFactorExponent;

    public Vector3 rotationSpeed;
    public float zoomSpeed;
    public float followSpeed;
    private Quaternion originalRotation;
    private bool endOfLevelReached = false;

    [SerializeField]
    private int zoomFramesLeft;
    [SerializeField]
    private int tiltFramesLeft;

    private void Start()
    {
        endOfLevelReached = false;
        player = Player.Find();
        player.OnStop += EndOfLevelZoom;

        offset = transform.position - player.transform.position;
        originalRotation = transform.rotation;

        zoomFramesLeft = zoomFrames;
        tiltFramesLeft = tiltFrames;
    }

    public void EndOfLevelZoom()
    {
        endOfLevelReached = true;
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

        if (endOfLevelReached)
        {
            DoEndOfLevelZoom();
        }
        else if (player.ShouldFollowYAxis())
        {
            Quaternion toRotation = Quaternion.LookRotation(player.transform.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, followSpeed * Time.deltaTime);
        }
    }

    private void DoEndOfLevelZoom()
    {
        if (zoomFramesLeft > 0)
        {
            zoomFramesLeft -= 1;
            Zoom((0.5f * zoomFramesLeft) / zoomFrames);
        }

        if (tiltFramesLeft > 0)
        {
            tiltFramesLeft -= 1;
            Steer((0.1f * tiltFramesLeft) / tiltFrames);
        }
    }

    public void Zoom(float zoom)
    {
        offset *= 1 + zoom * zoomSpeed;
    }
}
