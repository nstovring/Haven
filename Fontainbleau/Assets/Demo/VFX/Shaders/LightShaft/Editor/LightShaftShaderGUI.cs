using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor.Experimental.Rendering.HDPipeline
{
    public class LightShaftShaderGUI : ShaderGUI
    {
        protected static class Styles
        {
            public static string optionText = "Options";
            public static string surfaceTypeText = "Surface Type";
            public static string blendModeText = "Blend Mode";

            public static GUIContent alphaCutoffEnableText = new GUIContent("Alpha Cutoff Enable", "Threshold for alpha cutoff");
            public static GUIContent alphaCutoffText = new GUIContent("Alpha Cutoff", "Threshold for alpha cutoff");
            public static GUIContent doubleSidedModeText = new GUIContent("Double Sided", "This will render the two face of the objects (disable backface culling)");
            public static GUIContent cullModeText = new GUIContent("Culling Mode");
            public static readonly string[] cullModeNames = new string[3] { "Two Sided", "Back Face", "Front Face" };


            public static readonly string[] blendModeNames = Enum.GetNames(typeof(BlendMode));
            public static readonly string[] falloffModeNames = Enum.GetNames(typeof(FalloffMode));

            public static string InputsOptionsText = "Inputs options";

            public static string InputsHeader = "LightShaft Settings";
            public static string FalloffHeader = "Falloff Settings";
            public static string FadingHeader = "Fading Options";

            public static string InputsMapText = "";

            public static GUIContent softFadingLabel = new GUIContent("Soft Fading Distance", "In world units, depth distance to first opaque pixel behind from which the particle will fade. Zero disables the feature");
            public static GUIContent cameraFadingLabel = new GUIContent("CameraFade Distance (Min/Max)", "In world units, min and max depth distance which the current pixel will fade.");

            public static GUIContent colorText = new GUIContent("LightShaft Color");
            public static GUIContent alphaMaskText = new GUIContent("Alpha Mask texture");
            public static GUIContent falloffText = new GUIContent("Falloff Attenuation Power");
            public static GUIContent falloffTextureText = new GUIContent("Falloff Gradient Texture");
        }

        public enum BlendMode
        {
            Lerp,
            Add,
            SoftAdd,
            Multiply,
            Premultiply
        }

        public enum FalloffMode
        {
            Exponent,
            GradientTexture
        }


        void BlendModePopup()
        {
            EditorGUI.showMixedValue = blendMode.hasMixedValue;
            var mode = (BlendMode)blendMode.floatValue;

            EditorGUI.BeginChangeCheck();
            mode = (BlendMode)EditorGUILayout.Popup(Styles.blendModeText, (int)mode, Styles.blendModeNames);
            if (EditorGUI.EndChangeCheck())
            {
                m_MaterialEditor.RegisterPropertyChangeUndo("Blend Mode");
                blendMode.floatValue = (float)mode;
            }

            EditorGUI.showMixedValue = false;
        }

        protected void ShaderOptionsGUI()
        {
            EditorGUI.indentLevel++;
            GUILayout.Label(Styles.optionText, EditorStyles.boldLabel);

            surfaceType.floatValue = 1.0f; // Transparent

            BlendModePopup();

            m_MaterialEditor.ShaderProperty(alphaCutoffEnable, Styles.alphaCutoffEnableText.text);
            if (alphaCutoffEnable.floatValue == 1.0)
            {
                m_MaterialEditor.ShaderProperty(alphaCutoff, Styles.alphaCutoffText.text);
            }

            CullMode cull = (CullMode)this.cullMode.floatValue;

            EditorGUI.BeginChangeCheck();
            cull = (CullMode)EditorGUILayout.Popup(Styles.cullModeText.text, (int)cull, Styles.cullModeNames);
            if(EditorGUI.EndChangeCheck())
            {
                this.cullMode.floatValue = (float)cull;
            }


            EditorGUI.indentLevel--;
        }

        public void FindCommonOptionProperties(MaterialProperty[] props)
        {
            surfaceType = FindProperty(kSurfaceType, props);
            blendMode = FindProperty(kBlendMode, props);
            cullMode = FindProperty(kCullMode, props);
            alphaCutoff = FindProperty(kAlphaCutoff, props);
            alphaCutoffEnable = FindProperty(kAlphaCutoffEnabled, props);
        }

		protected void SetupCommonOptionsKeywords(Material material)
		{
            // Note: keywords must be based on Material value not on MaterialProperty due to multi-edit & material animation
            // (MaterialProperty value might come from renderer material property block)

            bool alphaTestEnable = material.GetFloat(kAlphaCutoffEnabled) == 1.0;
            BlendMode blendMode = (BlendMode)material.GetFloat(kBlendMode);

            material.SetOverrideTag("RenderType", "Transparent");
            material.SetInt("_ZWrite", 0);
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

            switch (blendMode)
            {
                case BlendMode.Lerp:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    break;

                case BlendMode.Add:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    break;

                case BlendMode.SoftAdd:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusDstColor);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    break;

                case BlendMode.Multiply:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.DstColor);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    break;

                case BlendMode.Premultiply:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    break;
            }

            SetKeyword(material, "_ALPHATEST_ON", alphaTestEnable);

            SetupEmissionGIFlags(material);
		}

        protected void SetKeyword(Material m, string keyword, bool state)
        {
            if (state)
                m.EnableKeyword(keyword);
            else
                m.DisableKeyword(keyword);
        }

        public void ShaderPropertiesGUI(Material material)
        {
            // Use default labelWidth
            EditorGUIUtility.labelWidth = 0f;

            // Detect any changes to the material
            EditorGUI.BeginChangeCheck();
            {
                ShaderOptionsGUI();
                EditorGUILayout.Space();
                ShaderInputGUI();
            }

            if (EditorGUI.EndChangeCheck())
            {
                foreach (var obj in m_MaterialEditor.targets)
                    SetupMaterialKeywords((Material)obj);
            }
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            FindCommonOptionProperties(props); // MaterialProperties can be animated so we do not cache them but fetch them every event to ensure animated values are updated correctly
            FindMaterialProperties(props);

            m_MaterialEditor = materialEditor;
            Material material = materialEditor.target as Material;
            ShaderPropertiesGUI(material);
        }

        protected virtual void SetupEmissionGIFlags(Material material)
        {
            // Setup lightmap emissive flags
            MaterialGlobalIlluminationFlags flags = material.globalIlluminationFlags;
            if ((flags & (MaterialGlobalIlluminationFlags.BakedEmissive | MaterialGlobalIlluminationFlags.RealtimeEmissive)) != 0)
            {
                if (ShouldEmissionBeEnabled(material))
                    flags &= ~MaterialGlobalIlluminationFlags.EmissiveIsBlack;
                else
                    flags |= MaterialGlobalIlluminationFlags.EmissiveIsBlack;

                material.globalIlluminationFlags = flags;
            }
        }

        protected MaterialEditor m_MaterialEditor;

        MaterialProperty surfaceType = null;
        protected const string kSurfaceType = "_SurfaceType";
        MaterialProperty blendMode = null;
        protected const string kBlendMode = "_BlendMode";
        MaterialProperty cullMode = null;
        protected const string kCullMode = "_CullMode";

        MaterialProperty alphaCutoff = null;
        protected const string kAlphaCutoff = "_AlphaCutoff";
        MaterialProperty alphaCutoffEnable = null;
        protected const string kAlphaCutoffEnabled = "_AlphaCutoffEnable";



        MaterialProperty alphaMask = null;
        protected const string kAlphaMask = "_AlphaMask";
        MaterialProperty alphaMaskST = null;
        protected const string kAlphaMaskST = "_AlphaMaskST";
        MaterialProperty alphaMaskScrollSpeed = null;
        protected const string kAlphaMaskScrollSpeed = "_AlphaMaskScrollSpeed";


        MaterialProperty lightShaftColor = null;
        protected const string kLightShaftColor = "_LightShaftColor";


        MaterialProperty falloffGradient = null;
        protected const string kFalloffGradient = "_FalloffGradient";
        MaterialProperty falloffPower = null;
        protected const string kFalloffPower = "_FalloffPower";
        MaterialProperty falloffMode = null;
        protected const string kFalloffMode = "_falloffMode";

        MaterialProperty softParticleInvDistance = null;
        protected const string kSoftParticleInvDistance = "_SoftParticleInvDistance";
        MaterialProperty cameraFadeNearDistance = null;
        protected const string kCameraFadeNearDistance = "_CameraFadeNearDistance";
        MaterialProperty cameraFadeFarDistance = null;
        protected const string kCameraFadeFarDistance = "_CameraFadeFarDistance";

        protected void FindMaterialProperties(MaterialProperty[] props)
        {
            alphaMask = FindProperty(kAlphaMask, props);
            alphaMaskST = FindProperty(kAlphaMaskST, props);
            alphaMaskScrollSpeed = FindProperty(kAlphaMaskScrollSpeed, props);

            lightShaftColor = FindProperty(kLightShaftColor, props);

            falloffMode = FindProperty(kFalloffMode, props);
            falloffGradient = FindProperty(kFalloffGradient, props);
            falloffPower = FindProperty(kFalloffPower, props);

            softParticleInvDistance = FindProperty(kSoftParticleInvDistance, props);
            cameraFadeNearDistance = FindProperty(kCameraFadeNearDistance, props);
            cameraFadeFarDistance = FindProperty(kCameraFadeFarDistance, props);
        }

        protected void ShaderInputGUI()
        {
            EditorGUI.indentLevel++;
            {
                GUILayout.Label(Styles.InputsHeader, EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                lightShaftColor.colorValue = EditorGUILayout.ColorField(new GUIContent("Light Shaft Color"), lightShaftColor.colorValue, true, true, true);


                //m_MaterialEditor.ColorProperty(lightShaftColor, Styles.colorText.text);
                m_MaterialEditor.TexturePropertySingleLine(Styles.alphaMaskText, alphaMask);

                EditorGUI.indentLevel++;
                {
                    // Scale / Translate
                    EditorGUI.BeginChangeCheck();
                    Vector4 st = alphaMaskST.vectorValue;
                
                    Vector2 scale = new Vector2(st.x, st.y);
                    Vector2 trans = new Vector2(st.z, st.w);
                    scale = EditorGUILayout.Vector2Field("Scale", scale);
                    trans = EditorGUILayout.Vector2Field("Translation", trans);

                    if(EditorGUI.EndChangeCheck())
                    {
                        alphaMaskST.vectorValue = new Vector4(scale.x, scale.y, trans.x, trans.y);
                    }

                    // Scrolling
                    EditorGUI.BeginChangeCheck();
                    Vector4 scroll4 = alphaMaskScrollSpeed.vectorValue;
                    Vector2 scroll = new Vector2(scroll4.x, scroll4.y);
                    scroll = EditorGUILayout.Vector2Field("Scrolling", scroll);

                    if(EditorGUI.EndChangeCheck())
                    {
                        alphaMaskScrollSpeed.vectorValue = new Vector4(scroll.x, scroll.y, 0, 0);
                    }
                }
                EditorGUI.indentLevel--;

                EditorGUI.indentLevel--;

                // Falloff
                GUILayout.Label(Styles.FalloffHeader, EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                {
                EditorGUI.BeginChangeCheck();
                int mode = EditorGUILayout.Popup("Falloff Mode",(int)falloffMode.floatValue, Styles.falloffModeNames);
                if(EditorGUI.EndChangeCheck())
                {
                    falloffMode.floatValue = (float)mode;
                }

                if(mode == (int)FalloffMode.GradientTexture)
                {
                    m_MaterialEditor.TexturePropertySingleLine(Styles.falloffTextureText, falloffGradient);
                }

                m_MaterialEditor.FloatProperty(falloffPower, Styles.falloffText.text);
                }
                EditorGUI.indentLevel--;




                GUILayout.Label(Styles.FadingHeader, EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                // Soft Particles
                EditorGUI.BeginChangeCheck();

                float invDist = softParticleInvDistance.floatValue;
                float dist = invDist < 0? 0.0f : 1.0f / invDist;
                dist = Mathf.Max(0.0f, EditorGUILayout.FloatField(Styles.softFadingLabel, dist));

                if(EditorGUI.EndChangeCheck())
                {
                    if (dist == 0) // Disable Feature
                    {
                        softParticleInvDistance.floatValue = -1.0f;
                    }
                    else
                        softParticleInvDistance.floatValue = 1.0f / dist;
                }

                // Canera Fading
                Vector2 distances = new Vector2(cameraFadeNearDistance.floatValue, cameraFadeFarDistance.floatValue);
                EditorGUI.BeginChangeCheck();

                distances = EditorGUILayout.Vector2Field(Styles.cameraFadingLabel, distances);

                if(EditorGUI.EndChangeCheck())
                {
                    if(distances.x < 0.0f)
                    {
                        distances.x = 0.0f;
                    }

                    if(distances.y < 0.0f)
                    {
                        distances.y = 0.0f;
                    }
                    cameraFadeNearDistance.floatValue = distances.x;
                    cameraFadeFarDistance.floatValue = distances.y;
                }

                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
        }

        protected void SetupMaterialKeywords(Material material)
        {
            SetupCommonOptionsKeywords(material);

            // Falloff gradient
            FalloffMode falloffMode = (FalloffMode)material.GetFloat(kFalloffMode);
            SetKeyword(material, "_FALLOFF_GRADIENT_ON", falloffMode == FalloffMode.GradientTexture);

            // Soft Particles
            bool bSoftParticlesEnabled = material.GetFloat(kSoftParticleInvDistance) >= 0.0f;
            SetKeyword(material, "_SOFTPARTICLES_ON", bSoftParticlesEnabled);
            bool bCameraFadeEnabled = !(material.GetFloat(kCameraFadeNearDistance) == 0.0f && material.GetFloat(kCameraFadeFarDistance) == 0.0f);
            SetKeyword(material, "_CAMERAFADING_ON", bCameraFadeEnabled);
        }

        protected bool ShouldEmissionBeEnabled(Material mat)
        {
            return false;
        }

        protected static string[] reservedProperties = new string[] { kSurfaceType, kBlendMode, kAlphaCutoff, kAlphaCutoffEnabled };
    }

}
