using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(BoxCollider))]
public class LightTrigger : MonoBehaviour {

    public Light targetLight;
    public bool autoDisable = true;

    private bool disabled = false;

    private void OnEnable()
    {
        gameObject.GetComponent<BoxCollider>().isTrigger = true;
    }

    void OnTriggerEnter (Collider other)
    {
        if (other.tag == "Player" && targetLight != null && !disabled)
        {
            foreach (var lightAnimator in targetLight.GetComponents<AbstractLightAnimation>())
            {
                Debug.Log(lightAnimator.animationMode);
                if (lightAnimator.animationMode == AbstractLightAnimation.AnimationMode.animateFromTrigger)
                {
                    lightAnimator.StartAnimation();
                }
            }
            if (autoDisable) { disabled = true; }
        }
            
	}
}
