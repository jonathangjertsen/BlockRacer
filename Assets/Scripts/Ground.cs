using UnityEngine;

public class Ground : MonoBehaviour
{
    public AudioRoundRobin audioCfg;

    private bool touched = false;

    public void Touch()
    {
        touched = true;
        TouchedPhysics();
        TouchedAppearance();
        TouchedSound();
        OnTouch();
    }

    private void TouchedPhysics()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
        }
    }

    private void TouchedAppearance()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        Color prevColor = mr.material.GetColor("GroundAlbedo");
        mr.material.SetColor("GroundAlbedo", prevColor * 0.30f);
    }

    private void TouchedSound()
    {
        audioCfg.Play();
    }

    public bool Touched()
    {
        return touched;
    }

    protected virtual void OnTouch()
    {

    }
}
