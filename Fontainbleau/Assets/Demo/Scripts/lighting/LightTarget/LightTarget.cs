using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;
using LightingTools;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.HDPipeline;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class LightTarget : MonoBehaviour
{
    public LightTargetParameters lightTargetParameters;
    public LightParameters lightParameters;
    [SerializeField]
    [HideInInspector]
    private GameObject targetedLight;
    [SerializeField]
    [HideInInspector]
    private GameObject LightParent;
    private bool showEntities = false;
    public bool advancedEnabled;

    private void OnEnable()
    {
        if (LightParent == null || LightParent.transform.parent != gameObject.transform) { CreateLightParent(); }
        if (targetedLight == null || LightParent.transform.parent != gameObject.transform) { CreateLight(); }
        //Enable if it has been disabled
        if (targetedLight != null) { targetedLight.GetComponent<Light>().enabled = true; }
        Update();
    }

    private void OnDisable()
    {
        targetedLight.GetComponent<Light>().enabled = false;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (LightParent != null) { SetLightParentTransform(); }
        if (lightParameters == null)
        {
            //var targetedLightSpot = targetedLight.GetComponent<Light>();
            //var targetedLightAdditionalData = targetedLight.GetComponent<HDAdditionalLightData>();
            //var targetedLightShadowData = targetedLight.GetComponent<AdditionalShadowData>();
            //LightingUtilities.InitLightParameters(targetedLightSpot, targetedLightAdditionalData, targetedLightShadowData, lightParameters);
        }
        if (targetedLight != null && lightTargetParameters != null)
        {
            SetLightTransform();
            SetLightSettings();
        }
        ApplyShowFlags(showEntities);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        //Draw Target
        EditorLightingUtilities.DrawCross(gameObject.transform);

        if (lightTargetParameters != null && lightTargetParameters.drawGizmo)
        {
            var targetedLightSpot = targetedLight.GetComponent<Light>();
            //var targetedLightAdditionalData = targetedLight.GetComponent<HDAdditionalLightData>();
            var gizmoColor = lightParameters.colorFilter;
            gizmoColor.a = 1f;
            Gizmos.color = gizmoColor;

            switch (lightParameters.lightShape)
            {
                case LightShape.Spot:
                    EditorLightingUtilities.DrawSpotlightGizmo(targetedLightSpot,true);
                    break;
                case LightShape.Point:
                    EditorLightingUtilities.DrawPointlightGizmo(targetedLightSpot,true);
                    break;
                case LightShape.Directional:
                    EditorLightingUtilities.DrawDirectionalLightGizmo(targetedLightSpot);
                    break;
                case LightShape.Rectangle:
                    EditorLightingUtilities.DrawArealightGizmo(targetedLightSpot);
                    break;
                case LightShape.Frustum:
                    EditorLightingUtilities.DrawFrustumlightGizmo(targetedLightSpot);
                    break;
                case LightShape.Sphere:
                    EditorLightingUtilities.DrawSpherelightGizmo(targetedLightSpot);
                    break;
                case LightShape.Line:
                    break;
                case LightShape.Disc:
                    break;
            }
        }
    }
#endif

    void CreateLightParent()
    {
        LightParent = new GameObject("LightParent");
        LightParent.transform.parent = gameObject.transform;
        LightParent.transform.position = Vector3.zero;
        LightParent.transform.rotation = Quaternion.identity;
    }

    void CreateLight()
    {
        targetedLight = new GameObject("TargetSpot");
        targetedLight.transform.parent = LightParent.transform;
        targetedLight.AddComponent<Light>();
        targetedLight.AddComponent<HDAdditionalLightData>();
        targetedLight.AddComponent<AdditionalShadowData>();
    }

    void SetLightTransform()
    {
        targetedLight.transform.localPosition = Vector3.forward * lightTargetParameters.distance;
        targetedLight.transform.localRotation = Quaternion.Euler(0, 180, 0);
        targetedLight.transform.localScale = gameObject.transform.localScale;
    }

    void SetLightParentTransform()
    {
        if (LightParent != null && lightTargetParameters != null && LightParent.transform.parent == gameObject.transform)
        {
            if (lightTargetParameters.linkToCameraRotation)
            {
                var cameraRotation = FindObjectOfType<Camera>().transform.rotation;
                LightParent.transform.rotation = Quaternion.Euler(new Vector3(lightTargetParameters.Pitch, lightTargetParameters.Yaw, lightTargetParameters.Roll)) * cameraRotation;
            }
            if (!lightTargetParameters.linkToCameraRotation)
            {
                LightParent.transform.rotation = Quaternion.Euler(new Vector3(lightTargetParameters.Pitch, lightTargetParameters.Yaw, lightTargetParameters.Roll));
            }
            LightParent.transform.localPosition = lightTargetParameters.offset;
        }
    }

    void SetLightSettings()
    {
        var targetedLightSpot = targetedLight.GetComponent<Light>();
        var targetedLightAdditionalData = targetedLight.GetComponent<HDAdditionalLightData>();
        var targetedLightShadowData = targetedLight.GetComponent<AdditionalShadowData>();
        LightingUtilities.ApplyLightParameters(targetedLightSpot, targetedLightAdditionalData, targetedLightShadowData, lightParameters);
    }

    private void OnDestroy()
    {
        DestroyImmediate(targetedLight);
        DestroyImmediate(LightParent);
    }

    void ApplyShowFlags(bool show)
    {
        if (targetedLight != null)
        {
            if (!show) { targetedLight.hideFlags = HideFlags.HideInHierarchy; }
            if (show)
            {
                targetedLight.hideFlags = HideFlags.None;
            }
        }
        if (LightParent != null)
        {
            if (!show) { LightParent.hideFlags = HideFlags.HideInHierarchy; }
            if (show)
            {
                LightParent.hideFlags = HideFlags.None;
            }
        }
    }

}
