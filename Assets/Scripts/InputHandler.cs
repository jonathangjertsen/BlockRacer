using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public InputMapping ballLeft;
    public InputMapping ballRight;
    public InputMapping ballSideBreak;
    public InputMapping ballJump;

    public InputMapping cameraUp;
    public InputMapping cameraDown;
    public InputMapping cameraZoomIn;
    public InputMapping cameraZoomOut;
    public InputMapping cameraReset;

    public InputMapping levelRestart;
    public InputMapping levelPause;
    public InputMapping levelExit;
    public InputMapping levelNext;

    void Update()
    {
        Check(ballLeft);
        Check(ballRight);
        Check(ballSideBreak);
        Check(ballJump);

        Check(cameraUp);
        Check(cameraDown);
        Check(cameraZoomIn);
        Check(cameraZoomOut);
        Check(cameraReset);

        Check(levelRestart);
        Check(levelPause);
        Check(levelExit);
        Check(levelNext);
    }

    void Check(InputMapping mapping)
    {
        bool shouldInvoke = false;

        switch (mapping.variant)
        {
            case InputMapping.Variant.Hold:
                shouldInvoke = Input.GetKey(mapping.key);
                break;
            case InputMapping.Variant.Press:
                shouldInvoke = Input.GetKeyDown(mapping.key);
                break;
            case InputMapping.Variant.Release:
                shouldInvoke = Input.GetKeyUp(mapping.key);
                break;
        }

        if (shouldInvoke)
        {
            mapping.handler.Invoke();
        }
    }
}
