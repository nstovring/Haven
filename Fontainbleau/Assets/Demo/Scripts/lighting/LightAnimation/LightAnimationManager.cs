using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.HDPipeline;

[RequireComponent(typeof(Light))]
public class LightAnimationManager : MonoBehaviour
{
    private float initialIntensity;

    void Start()
    {
        initialIntensity = gameObject.GetComponent<HDAdditionalLightData>().lightDimmer;
    }

    void Update()
    {
        var currentValue = 1.0f;
        foreach (var lightAnimator in gameObject.GetComponents<AbstractLightAnimation>())
        {
            currentValue *= lightAnimator.getCurrentValue();
        }
        gameObject.GetComponent<HDAdditionalLightData>().lightDimmer = currentValue * initialIntensity;
    }
}