using UnityEngine;

public class Ground : MonoBehaviour
{
    private bool touched = false;
    public AudioRoundRobin audioCfg;

    public void Touch()
    {
        touched = true;
        OnTouch();
        TouchedPhysics();
        TouchedAppearance();
        TouchedSound();
    }

    private void TouchedPhysics()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
        }
    }

    private void TouchedSound()
    {
        if (audioCfg != null)
        {
            audioCfg.Play();
        }
    }

    private void TouchedAppearance()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        Color prevColor = mr.material.GetColor("GroundAlbedo");
        mr.material.SetColor("GroundAlbedo", prevColor * 0.30f);
    }

    public bool Touched()
    {
        return touched;
    }

    protected virtual void OnTouch()
    {

    }
}
