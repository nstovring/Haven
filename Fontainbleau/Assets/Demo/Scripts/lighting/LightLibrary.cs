using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Object = UnityEngine.Object;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.HDPipeline;

namespace LightingTools
{
    [System.Serializable]
    public enum LightmapPresetBakeType
    {
        //Simplify serialization
        Baked = 0,
        Mixed = 1,
        Realtime = 2
    }

    public enum ShadowQuality
    {
        FromQualitySettings = 0,
        Low = 1,
        Medium = 2,
        High = 3,
        VeryHigh = 4
    }

    [System.Serializable]
    public enum LightShape
    {
        Point = 0,
        Spot = 1,
        Directional = 2,
        Rectangle = 3,
        Sphere = 4,
        Line = 5,
        Disc = 6,
        Frustum = 7
    }

    [System.Serializable]
    public class CascadeParameters
    {
        [Range(1, 4)]
        public int count = 4;
        [Range(0, 1)]
        public float split0 = 0.05f;
        [Range(0, 1)]
        public float split1 = 0.1f;
        [Range(0, 1)]
        public float split2 = 0.2f;
    }

    [System.Serializable]
    public class LightParameters
    {
        public LightType type = LightType.Point;
        public LightmapPresetBakeType mode = LightmapPresetBakeType.Mixed;
        public float range = 8;
        public bool useColorTemperature;
        public float colorTemperature = 6500;
        public Color colorFilter = Color.white;
        public float intensity = 1;
        public float indirectIntensity = 1;
        [Range(0f, 180f)]
        public float lightAngle = 45;
        public LightShadows shadowsType = LightShadows.Soft;
        public ShadowQuality shadowQuality = ShadowQuality.High;
        [Range(0.01f, 10f)]
        public float ShadowNearClip = 0.5f;
        [Range(0f, 2f)]
        public float shadowBias = 0.005f;
        [Range(0f, 0.5f)]
        public float shadowNormalBias = 0.001f;
        public CascadeParameters cascadeparams;
        public Texture lightCookie;
        public float cookieSize = 5;
        //HDRenderPipelineParameters
#if UNITY_2017 || UNITY_2018
        public LightShape lightShape = LightShape.Point;
        [Range(0f, 180f)]
        public float innerAngle = 40;
        public float lightRadius = 0.05f;
        public float lightLength = 0.05f;
        public float lightWidth = 0.05f;
        public float fadeDistance = 50;
        public float shadowFadeDistance = 50;
        public bool affectDiffuse = true;
        public bool affectSpecular = true;
#endif
        public bool shadows;
        public bool advancedEnabled;
    }

    [System.Serializable]
    public class LightTargetParameters
    {
        public string name = "Light01";
        [Range(-180f, 180f)]
        public float Yaw = 0.1f;
        [Range(90f, -90f)]
        public float Pitch = 0.1f;
        [Range(-180f, 180f)]
        public float Roll = 0.1f;
        public bool linkToCameraRotation = false;
        public float distance = 2f;
        public Vector3 offset;
        public bool drawGizmo = false;
    }

    [System.Serializable]
    public class LightSourceMeshParameters
    {
        public GameObject lightSourceObject;
        public ShadowCastingMode meshShadowMode;
        public bool showObjectInHierarchy;
    }

    [System.Serializable]
    public class LightSourceMaterialsParameters
    {
        public bool linkEmissiveIntensityWithLight = true;
        public bool linkEmissiveColorWithLight = true;
        public float emissiveMultiplier = 1f;
    }

    [System.Serializable]
    public class LightSourceAnimationParameters
    {
        public bool enabledFromStart = true;
        public bool enableFunctionalAnimation = false;
        public bool enableSwithOnAnimation = false;
        public bool enableSwithOffAnimation = false;
        public bool enableBreakAnimation = false;

        public LightAnimationParameters functionalAnimationParameters;
        public LightAnimationParameters switchOnAnimationParameters;
        public LightAnimationParameters switchOffAnimationParameters;
        public LightAnimationParameters breakAnimationParameters;
    }

    [System.Serializable]
    public class LightAnimationParameters
    {
        public AnimationMode animationMode;
        public AnimationCurve animationCurve;
        public AnimationClip animationClip;
        public NoiseAnimationParameters noiseParameters;
        public float animationLength = 1;
    }

    [System.Serializable]
    public class NoiseAnimationParameters
    {
        public float frequency = 5;
        public float minimumValue = 0;
        public float maximumValue = 1;
        [Range(0.0f, 1.0f)]
        public float jumpFrequency = 0;
    }

    public enum AnimationMode { Curve, Noise, AnimationClip }


    public static class LightingUtilities
    {

        public static void ApplyLightParameters(Light light, HDAdditionalLightData additionalLight, AdditionalShadowData shadowData, LightParameters lightparameters)
        {
            if (additionalLight == null || shadowData == null)
            {
                Debug.Log("HD render pipeline components not found on " + light.gameObject.name);
                //InitLightParameters(light, additionalLight, shadowData, lightparameters);
                return;
            }
            switch (lightparameters.lightShape)
            {
                case LightShape.Directional: light.type = LightType.Directional; additionalLight.lightTypeExtent = LightTypeExtent.Punctual; break;
                case LightShape.Point: light.type = LightType.Point; additionalLight.lightTypeExtent = LightTypeExtent.Punctual; break;
                case LightShape.Spot: light.type = LightType.Spot; additionalLight.lightTypeExtent = LightTypeExtent.Punctual; break;
                case LightShape.Rectangle: light.type = LightType.Point; additionalLight.lightTypeExtent = LightTypeExtent.Rectangle; break;
               // case LightShape.Frustum: light.type = LightType.Directional; additionalLight.lightTypeExtent = LightTypeExtent.Projector; break;
                //TODO : connect correct light shape when implemented
                case LightShape.Sphere: light.type = LightType.Point; additionalLight.lightTypeExtent = LightTypeExtent.Punctual; break;
                case LightShape.Line: light.type = LightType.Point; additionalLight.lightTypeExtent = LightTypeExtent.Line; break;
                case LightShape.Disc: light.type = LightType.Point; additionalLight.lightTypeExtent = LightTypeExtent.Punctual; break;
            }

#if UNITY_EDITOR
            switch (lightparameters.mode)
            {
                case LightmapPresetBakeType.Realtime: light.lightmapBakeType = LightmapBakeType.Realtime; break;
                case LightmapPresetBakeType.Baked: light.lightmapBakeType = LightmapBakeType.Baked; break;
                case LightmapPresetBakeType.Mixed: light.lightmapBakeType = LightmapBakeType.Mixed; break;
            }
#endif
            if (lightparameters.shadows == true)
            {
                lightparameters.shadowsType = (LightShadows)Mathf.Clamp((int)lightparameters.shadowsType, 1, 2);
            }
            if (lightparameters.shadows == false)
            {
                lightparameters.shadowsType = LightShadows.None;
            }
            light.shadows = lightparameters.shadowsType;
            light.intensity = lightparameters.intensity;
            light.color = lightparameters.colorFilter;
            light.range = lightparameters.range;
            light.shadowBias = lightparameters.shadowBias;
            light.shadowNormalBias = lightparameters.shadowNormalBias;
            light.spotAngle = lightparameters.lightAngle;
            additionalLight.m_InnerSpotPercent = lightparameters.innerAngle / lightparameters.lightAngle * 100;
            light.shadowNearPlane = lightparameters.ShadowNearClip;
            switch (lightparameters.shadowQuality)
            {
                case LightingTools.ShadowQuality.VeryHigh: shadowData.shadowResolution = 2048; break;
                case LightingTools.ShadowQuality.High: shadowData.shadowResolution = 1024; break;
                case LightingTools.ShadowQuality.Medium: shadowData.shadowResolution = 512; break;
                case LightingTools.ShadowQuality.Low: shadowData.shadowResolution = 256; break;
            }
            light.cookie = lightparameters.lightCookie;

            if (lightparameters.lightShape == LightShape.Rectangle || lightparameters.lightShape == LightShape.Frustum)
            {
                additionalLight.shapeHeight = lightparameters.lightLength;
                additionalLight.shapeWidth = lightparameters.lightWidth;
            }
            else
            {
                additionalLight.shapeHeight = lightparameters.lightRadius;
                additionalLight.shapeWidth = lightparameters.lightRadius;
            }
            additionalLight.fadeDistance = lightparameters.fadeDistance;
            shadowData.shadowFadeDistance = lightparameters.shadowFadeDistance;
            additionalLight.affectDiffuse = lightparameters.affectDiffuse;
            additionalLight.affectSpecular = lightparameters.affectSpecular;

            //TO-DO : check API changes$
            
            var shadowCascadeRatios = new float[3];
            shadowCascadeRatios[0] = lightparameters.cascadeparams.split0;
            shadowCascadeRatios[1] = lightparameters.cascadeparams.split1;
            shadowCascadeRatios[2] = lightparameters.cascadeparams.split2;
            var shadowCascadeBorders = new float[3];
            shadowCascadeBorders[0] = shadowCascadeBorders[1] = shadowCascadeBorders[2] = 0.2f;
            shadowData.SetShadowCascades(4, shadowCascadeRatios, shadowCascadeBorders);
        }

//        public static void InitLightParameters(Light light, HDAdditionalLightData lightData, AdditionalShadowData shadowData, LightParameters lightparameters)
//        {
//            //if (light == null || lightData == null || shadowData == null)
//            //    return;
//
//            lightparameters.type = light.type;
//
//            if (lightData.lightTypeExtent == LightTypeExtent.Punctual)
//            {
//                switch (light.type)
//                {
//                    case LightType.Directional: lightparameters.lightShape = LightShape.Directional; break;
//                    case LightType.Spot: lightparameters.lightShape = LightShape.Spot; break;
//                    case LightType.Point: lightparameters.lightShape = LightShape.Point; break;
//                }
//            }
//            else if (lightData.lightTypeExtent == LightTypeExtent.Rectangle)
//            {
//                lightparameters.lightShape = LightShape.Rectangle;
//            }
//       //     else if (lightData.lightTypeExtent == LightTypeExtent.Projector)
//        //    {
//         //       lightparameters.lightShape = LightShape.Frustum;
//          //  }
//
//#if UNITY_EDITOR
//            switch (light.lightmapBakeType)
//            {
//                case LightmapBakeType.Realtime: lightparameters.mode = LightmapPresetBakeType.Realtime; break;
//                case LightmapBakeType.Baked: lightparameters.mode = LightmapPresetBakeType.Baked; break;
//                case LightmapBakeType.Mixed: lightparameters.mode = LightmapPresetBakeType.Mixed; break;
//            }
//#endif
//
//            lightparameters.range = light.range;
//            lightparameters.colorFilter = light.color;
//            lightparameters.intensity = light.intensity;
//            lightparameters.lightAngle = light.spotAngle;
//            lightparameters.shadowsType = light.shadows;
//            if (shadowData != null)
//            {
//                switch (shadowData.shadowResolution)
//                {
//                    case 2048: lightparameters.shadowQuality = LightingTools.ShadowQuality.VeryHigh; break;
//                    case 1024: lightparameters.shadowQuality = LightingTools.ShadowQuality.High; break;
//                    case 512: lightparameters.shadowQuality = LightingTools.ShadowQuality.Medium; break;
//                    case 256: lightparameters.shadowQuality = LightingTools.ShadowQuality.Low; break;
//                }
//                float[] cascadeRatios = null;
//                float[] cascadeBorders = null;
//                shadowData.GetShadowCascades(out lightparameters.cascadeparams.count, out cascadeRatios, out cascadeBorders);
//                lightparameters.cascadeparams.split0 = cascadeRatios[0];
//                lightparameters.cascadeparams.split1 = cascadeRatios[1];
//                lightparameters.cascadeparams.split2 = cascadeRatios[2];
//
//                lightparameters.shadowFadeDistance = shadowData.shadowFadeDistance;
//            }
//
//            lightparameters.ShadowNearClip = light.shadowNearPlane;
//            lightparameters.shadowBias = light.shadowBias;
//            lightparameters.shadowNormalBias = light.shadowNormalBias;
//            lightparameters.lightCookie = light.cookie;
//            lightparameters.innerAngle = lightData.m_InnerSpotPercent * lightparameters.lightAngle / 100;
//            lightparameters.fadeDistance = lightData.fadeDistance;
//            lightparameters.affectDiffuse = lightData.affectDiffuse;
//            lightparameters.affectSpecular = lightData.affectSpecular;
//        }

    }

#if UNITY_EDITOR
    public static class EditorLightingUtilities
    {
        public static void AssignSerializedProperty(SerializedProperty sp, object source)
        {
            var valueType = source.GetType();
            if (valueType.IsEnum)
            {
                sp.enumValueIndex = (int)source;
            }
            else if (valueType == typeof(Color))
            {
                sp.colorValue = (Color)source;
            }
            else if (valueType == typeof(float))
            {
                sp.floatValue = (float)source;
            }
            else if (valueType == typeof(Vector3))
            {
                sp.vector3Value = (Vector3)source;
            }
            else if (valueType == typeof(bool))
            {
                sp.boolValue = (bool)source;
            }
            else if (valueType == typeof(string))
            {
                sp.stringValue = (string)source;
            }
            else if (typeof(int).IsAssignableFrom(valueType))
            {
                sp.intValue = (int)source;
            }
            else if (typeof(Object).IsAssignableFrom(valueType))
            {
                sp.objectReferenceValue = (Object)source;
            }
            else
            {
                Debug.LogError("Missing type  : " + valueType);
            }
        }

        public static void DrawSpotlightGizmo(Light spotlight, bool selected)
        {
            var flatRadiusAtRange = spotlight.range * Mathf.Tan(spotlight.spotAngle * Mathf.Deg2Rad * 0.5f);

            var vectorLineUp = Vector3.Normalize(spotlight.gameObject.transform.position + spotlight.gameObject.transform.forward * spotlight.range + spotlight.gameObject.transform.up * flatRadiusAtRange - spotlight.gameObject.transform.position);
            var vectorLineDown = Vector3.Normalize(spotlight.gameObject.transform.position + spotlight.gameObject.transform.forward * spotlight.range + spotlight.gameObject.transform.up * -flatRadiusAtRange - spotlight.gameObject.transform.position);
            var vectorLineRight = Vector3.Normalize(spotlight.gameObject.transform.position + spotlight.gameObject.transform.forward * spotlight.range + spotlight.gameObject.transform.right * flatRadiusAtRange - spotlight.gameObject.transform.position);
            var vectorLineLeft = Vector3.Normalize(spotlight.gameObject.transform.position + spotlight.gameObject.transform.forward * spotlight.range + spotlight.gameObject.transform.right * -flatRadiusAtRange - spotlight.gameObject.transform.position);

            var rangeDiscDistance = Mathf.Cos(Mathf.Deg2Rad * spotlight.spotAngle / 2) * spotlight.range;
            var rangeDiscRadius = spotlight.range * Mathf.Sin(spotlight.spotAngle * Mathf.Deg2Rad * 0.5f);
            var nearDiscDistance = Mathf.Cos(Mathf.Deg2Rad * spotlight.spotAngle / 2) * spotlight.shadowNearPlane;
            var nearDiscRadius = spotlight.shadowNearPlane * Mathf.Sin(spotlight.spotAngle * Mathf.Deg2Rad * 0.5f);

            
            //Draw Range disc
            Handles.Disc(spotlight.gameObject.transform.rotation, spotlight.gameObject.transform.position + spotlight.gameObject.transform.forward * rangeDiscDistance, spotlight.gameObject.transform.forward, rangeDiscRadius, false, 1);
            //Draw Lines

            Gizmos.DrawLine(spotlight.gameObject.transform.position, spotlight.gameObject.transform.position + vectorLineUp * spotlight.range);
            Gizmos.DrawLine(spotlight.gameObject.transform.position, spotlight.gameObject.transform.position + vectorLineDown * spotlight.range);
            Gizmos.DrawLine(spotlight.gameObject.transform.position, spotlight.gameObject.transform.position + vectorLineRight * spotlight.range);
            Gizmos.DrawLine(spotlight.gameObject.transform.position, spotlight.gameObject.transform.position + vectorLineLeft * spotlight.range);

            if(selected)
            {
                //Draw Range Arcs
                Handles.DrawWireArc(spotlight.gameObject.transform.position, spotlight.gameObject.transform.right, vectorLineUp, spotlight.spotAngle, spotlight.range);
                Handles.DrawWireArc(spotlight.gameObject.transform.position, spotlight.gameObject.transform.up, vectorLineLeft, spotlight.spotAngle, spotlight.range);
                //Draw Near Plane Disc
                if (spotlight.shadows != LightShadows.None) Handles.Disc(spotlight.gameObject.transform.rotation, spotlight.gameObject.transform.position + spotlight.gameObject.transform.forward * nearDiscDistance, spotlight.gameObject.transform.forward, nearDiscRadius, false, 1);

                //Inner Cone
                var additionalLightData = spotlight.GetComponent<HDAdditionalLightData>();
                DrawInnerCone(spotlight,additionalLightData);
            }
        }

        public static void DrawInnerCone(Light spotlight, HDAdditionalLightData additionalLightData)
        {
            if (additionalLightData == null) return;

            var flatRadiusAtRange = spotlight.range * Mathf.Tan(spotlight.spotAngle * additionalLightData.m_InnerSpotPercent * 0.01f * Mathf.Deg2Rad * 0.5f);

            var vectorLineUp = Vector3.Normalize(spotlight.gameObject.transform.position + spotlight.gameObject.transform.forward * spotlight.range + spotlight.gameObject.transform.up * flatRadiusAtRange - spotlight.gameObject.transform.position);
            var vectorLineDown = Vector3.Normalize(spotlight.gameObject.transform.position + spotlight.gameObject.transform.forward * spotlight.range + spotlight.gameObject.transform.up * -flatRadiusAtRange - spotlight.gameObject.transform.position);
            var vectorLineRight = Vector3.Normalize(spotlight.gameObject.transform.position + spotlight.gameObject.transform.forward * spotlight.range + spotlight.gameObject.transform.right * flatRadiusAtRange - spotlight.gameObject.transform.position);
            var vectorLineLeft = Vector3.Normalize(spotlight.gameObject.transform.position + spotlight.gameObject.transform.forward * spotlight.range + spotlight.gameObject.transform.right * -flatRadiusAtRange - spotlight.gameObject.transform.position);

            //Draw Lines

            Gizmos.DrawLine(spotlight.gameObject.transform.position, spotlight.gameObject.transform.position + vectorLineUp * spotlight.range);
            Gizmos.DrawLine(spotlight.gameObject.transform.position, spotlight.gameObject.transform.position + vectorLineDown * spotlight.range);
            Gizmos.DrawLine(spotlight.gameObject.transform.position, spotlight.gameObject.transform.position + vectorLineRight * spotlight.range);
            Gizmos.DrawLine(spotlight.gameObject.transform.position, spotlight.gameObject.transform.position + vectorLineLeft * spotlight.range);

            var innerAngle = spotlight.spotAngle * additionalLightData.GetInnerSpotPercent01();
            if (innerAngle > 0)
            {
                var innerDiscDistance = Mathf.Cos(Mathf.Deg2Rad * innerAngle * 0.5f) * spotlight.range;
                var innerDiscRadius = spotlight.range * Mathf.Sin(innerAngle * Mathf.Deg2Rad * 0.5f);
                //Draw Range disc
                Handles.Disc(spotlight.gameObject.transform.rotation, spotlight.gameObject.transform.position + spotlight.gameObject.transform.forward * innerDiscDistance, spotlight.gameObject.transform.forward, innerDiscRadius, false, 1);
            }
        }

        public static void DrawArealightGizmo(Light arealight)
        {
            
            var RectangleSize = new Vector3(arealight.areaSize.x, arealight.areaSize.y, 0);
            Gizmos.matrix = arealight.transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, RectangleSize);
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.DrawWireSphere(arealight.transform.position, arealight.range);
        }

        public static void DrawPointlightGizmo(Light pointlight, bool selected)
        {
            if (pointlight.shadows != LightShadows.None && selected) Gizmos.DrawWireSphere(pointlight.transform.position, pointlight.shadowNearPlane);
            Gizmos.DrawWireSphere(pointlight.transform.position, pointlight.range);
        }

        public static void DrawSpherelightGizmo(Light spherelight)
        {
            var additionalLightData = spherelight.GetComponent<HDAdditionalLightData>();
            if (additionalLightData == null) return;
            Gizmos.DrawSphere(spherelight.transform.position, additionalLightData.shapeHeight);
            if (spherelight.shadows != LightShadows.None) Gizmos.DrawWireSphere(spherelight.transform.position, spherelight.shadowNearPlane);
            Gizmos.DrawWireSphere(spherelight.transform.position, spherelight.range);
        }

        public static void DrawFrustumlightGizmo(Light frustumlight)
        {
            var additionalLightData = frustumlight.GetComponent<HDAdditionalLightData>();
            if (additionalLightData == null) return;
            var frustumSize = new Vector3(additionalLightData.shapeHeight / frustumlight.gameObject.transform.localScale.x, additionalLightData.shapeWidth / frustumlight.gameObject.transform.localScale.y, frustumlight.range - frustumlight.shadowNearPlane / frustumlight.gameObject.transform.localScale.z);
            Gizmos.matrix = frustumlight.transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.forward * (frustumSize.z * 0.5f + frustumlight.shadowNearPlane), frustumSize);
            Gizmos.matrix = Matrix4x4.identity;
        }

        public static void DrawDirectionalLightGizmo(Light directionalLight)
        {
            var gizmoSize = 0.2f;
            Handles.Disc(directionalLight.transform.rotation, directionalLight.transform.position, directionalLight.gameObject.transform.forward, gizmoSize, false, 1);
            Gizmos.DrawLine(directionalLight.transform.position, directionalLight.transform.position + directionalLight.transform.forward);
            Gizmos.DrawLine(directionalLight.transform.position + directionalLight.transform.up * gizmoSize, directionalLight.transform.position + directionalLight.transform.up * gizmoSize + directionalLight.transform.forward);
            Gizmos.DrawLine(directionalLight.transform.position + directionalLight.transform.up * -gizmoSize, directionalLight.transform.position + directionalLight.transform.up * -gizmoSize + directionalLight.transform.forward);
            Gizmos.DrawLine(directionalLight.transform.position + directionalLight.transform.right * gizmoSize, directionalLight.transform.position + directionalLight.transform.right * gizmoSize + directionalLight.transform.forward);
            Gizmos.DrawLine(directionalLight.transform.position + directionalLight.transform.right * -gizmoSize, directionalLight.transform.position + directionalLight.transform.right * -gizmoSize + directionalLight.transform.forward);
        }

        public static void DrawCross(Transform m_transform)
        {
            var gizmoSize = 0.25f;
            Gizmos.DrawLine(m_transform.position, m_transform.position + m_transform.TransformVector(m_transform.root.forward * gizmoSize / m_transform.localScale.z));
            Gizmos.DrawLine(m_transform.position, m_transform.position + m_transform.TransformVector(m_transform.root.forward * -gizmoSize / m_transform.localScale.z));
            Gizmos.DrawLine(m_transform.position, m_transform.position + m_transform.TransformVector(m_transform.root.up * gizmoSize / m_transform.localScale.y));
            Gizmos.DrawLine(m_transform.position, m_transform.position + m_transform.TransformVector(m_transform.root.up * -gizmoSize / m_transform.localScale.y));
            Gizmos.DrawLine(m_transform.position, m_transform.position + m_transform.TransformVector(m_transform.root.right * gizmoSize / m_transform.localScale.x));
            Gizmos.DrawLine(m_transform.position, m_transform.position + m_transform.TransformVector(m_transform.root.right * -gizmoSize / m_transform.localScale.x));
        }

        public static bool DrawHeader(string title, bool activeField)
        {
            var backgroundRect = GUILayoutUtility.GetRect(1f, 17f);

            var labelRect = backgroundRect;
            labelRect.xMin += 16f;
            labelRect.xMax -= 20f;

            var toggleRect = backgroundRect;
            toggleRect.y += 2f;
            toggleRect.width = 13f;
            toggleRect.height = 13f;

            // Background rect should be full-width
            backgroundRect.xMin = 0f;
            backgroundRect.width += 4f;

            // Background
            float backgroundTint = EditorGUIUtility.isProSkin ? 0.1f : 1f;
            EditorGUI.DrawRect(backgroundRect, new Color(backgroundTint, backgroundTint, backgroundTint, 0.2f));

            // Title
            using (new EditorGUI.DisabledScope(!activeField))
                EditorGUI.LabelField(labelRect, title, EditorStyles.boldLabel);

            // Active checkbox
            activeField = GUI.Toggle(toggleRect, activeField, GUIContent.none, new GUIStyle("ShurikenCheckMark"));

            var e = Event.current;
            if (e.type == EventType.MouseDown && backgroundRect.Contains(e.mousePosition) && e.button == 0)
            {
                activeField = !activeField;
                e.Use();
            }

            EditorGUILayout.Space();

            return activeField;
        }

        public static void DrawHeader(string title)
        {
            var backgroundRect = GUILayoutUtility.GetRect(1f, 17f);

            var labelRect = backgroundRect;
            labelRect.xMin += 16f;
            labelRect.xMax -= 20f;

            var foldoutRect = backgroundRect;
            foldoutRect.y += 1f;
            foldoutRect.width = 13f;
            foldoutRect.height = 13f;

            // Background rect should be full-width
            backgroundRect.xMin = 0f;
            backgroundRect.width += 4f;

            // Background
            float backgroundTint = EditorGUIUtility.isProSkin ? 0.1f : 1f;
            EditorGUI.DrawRect(backgroundRect, new Color(backgroundTint, backgroundTint, backgroundTint, 0.2f));

            // Title
            EditorGUI.LabelField(labelRect, title, EditorStyles.boldLabel);
            EditorGUILayout.Space();
        }

        public static void DrawSplitter()
        {
            EditorGUILayout.Space();
            var rect = GUILayoutUtility.GetRect(1f, 1f);

            // Splitter rect should be full-width
            rect.xMin = 0f;
            rect.width += 4f;

            if (Event.current.type != EventType.Repaint)
                return;

            EditorGUI.DrawRect(rect, !EditorGUIUtility.isProSkin
                ? new Color(0.6f, 0.6f, 0.6f, 1.333f)
                : new Color(0.12f, 0.12f, 0.12f, 1.333f));
        }

        public static bool DrawHeaderFoldout(string title, bool state)
        {
            var backgroundRect = GUILayoutUtility.GetRect(1f, 17f);

            var labelRect = backgroundRect;
            labelRect.xMin += 16f;
            labelRect.xMax -= 20f;

            var foldoutRect = backgroundRect;
            foldoutRect.y += 1f;
            foldoutRect.width = 13f;
            foldoutRect.height = 13f;

            // Background rect should be full-width
            backgroundRect.xMin = 0f;
            backgroundRect.width += 4f;

            // Background
            float backgroundTint = EditorGUIUtility.isProSkin ? 0.1f : 1f;
            EditorGUI.DrawRect(backgroundRect, new Color(backgroundTint, backgroundTint, backgroundTint, 0.2f));

            // Title
            EditorGUI.LabelField(labelRect, title, EditorStyles.boldLabel);

            // Active checkbox
            state = GUI.Toggle(foldoutRect, state, GUIContent.none, EditorStyles.foldout);

            var e = Event.current;
            if (e.type == EventType.MouseDown && backgroundRect.Contains(e.mousePosition) && e.button == 0)
            {
                state = !state;
                e.Use();
            }

            return state;
        }
    }
#endif
}
