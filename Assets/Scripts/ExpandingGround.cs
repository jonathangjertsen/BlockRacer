using UnityEngine;

public class ExpandingGround : Ground
{
    public float expansionPerFrame;

    private void FixedUpdate()
    {
        transform.localScale += Vector3.one * expansionPerFrame;
    }

    protected override void OnTouch()
    {
        audioCfg.source.pitch = Mathf.Pow(2f / transform.localScale[0], 2.5f);
    }
}
