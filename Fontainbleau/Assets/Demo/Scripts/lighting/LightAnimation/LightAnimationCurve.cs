using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractLightAnimation : MonoBehaviour
{
    public abstract float getCurrentValue();

    public abstract void StartAnimation();
    public abstract void StopAnimation();
    public abstract void PauseAnimation();
    public enum AnimationMode { animateFromStart, animateFromTrigger, animateFromScript };
    public AnimationMode animationMode;
}


[RequireComponent(typeof(LightAnimationManager))]
public class LightAnimationCurve : AbstractLightAnimation
{
    public AnimationCurve intensityCurve;
    public float animationLength = 1;
    public bool loopAnimation;
	public bool enableLightFromStart = true;
    private float fade = 1;
    private bool animate = false;
    private float localTime = 0 ;
    private bool lightEnabled = true;

	void Start ()
    {
        if ( animationMode == AnimationMode.animateFromStart) { animate = true; }
        if (!enableLightFromStart) { lightEnabled = false; }
	}

    public override float getCurrentValue()
    {
        if (!lightEnabled) { fade = 0; }
        if (animate)
        {
            if (loopAnimation) { localTime = (localTime + Time.deltaTime) % animationLength; }
            if (!loopAnimation) { localTime += Time.deltaTime; }
            fade = intensityCurve.Evaluate(localTime / animationLength);
            if (localTime >= animationLength) { StopAnimate(); }
        }
        return fade;
    }

    void Animate()
    {
        animate = true;
    }

    void PauseAnimate()
    {
        animate = false;
    }

    void StopAnimate()
    {
        if (fade > 0 ) { lightEnabled = true; }
        PauseAnimate();
        localTime = 0;
    }

    //Public Events
    public override void StartAnimation()
    {
        Animate();
    }

    public override void StopAnimation()
    {
        StopAnimate();
    }

    public override void PauseAnimation()
    {
        PauseAnimate();
    }
}
