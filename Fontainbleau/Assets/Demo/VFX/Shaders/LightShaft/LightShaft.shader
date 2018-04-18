Shader "HDRenderPipeline/Lightshaft"
{
    Properties
    {
		_LightShaftColor("Color", Color) = (1.0,1.0,1.0,1.0)
        _AlphaMask("Alpha Mask (Grayscale)", 2D) = "white" {}
		_AlphaMaskST("Alpha Mask Scale & Translate", Vector) = (1.0,1.0,0.0,0.0)
		_AlphaMaskScrollSpeed("Alpha Mask Scroll Speed", Vector) = (0.0,0.0,0.0,0.0)

        _FalloffGradient("Falloff Gradient", 2D) = "white" {}
		_FalloffPower("Falloff Power", Float) = 3.0

        _SoftParticleInvDistance("_SoftParticleInvDistance", float) = 1.0

		_CameraFadeNearDistance("Camera Fade Near Distance", float) = 0.0
		_CameraFadeFarDistance("Camera Fade Far Distance", float) = 1.0

        [ToggleOff]  _AlphaCutoffEnable("Alpha Cutoff Enable", Float) = 0.0
        _AlphaCutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

        // Blending state
        [HideInInspector] _SurfaceType("__surfacetype", Float) = 0.0
        [HideInInspector] _BlendMode("__blendmode", Float) = 0.0
		[HideInInspector] _falloffMode("__falloffMode", Float) = 0.0
        [HideInInspector] _SrcBlend("__src", Float) = 1.0
        [HideInInspector] _DstBlend("__dst", Float) = 0.0
        [HideInInspector] _ZWrite("__zw", Float) = 1.0
        [HideInInspector] _CullMode("__cullmode", Float) = 2.0
        [HideInInspector] _ZTestMode("_ZTestMode", Int) = 8
    }

    HLSLINCLUDE

    #pragma target 4.5
    #pragma only_renderers d3d11 ps4 xboxone vulkan metal

    //-------------------------------------------------------------------------------------
    // Variant
    //-------------------------------------------------------------------------------------

    #pragma shader_feature _FALLOFF_GRADIENT_ON
    #pragma shader_feature _ALPHATEST_ON
    #pragma shader_feature _SOFTPARTICLES_ON
	#pragma shader_feature _CAMERAFADING_ON

    //-------------------------------------------------------------------------------------
    // Define
    //-------------------------------------------------------------------------------------

    #define UNITY_MATERIAL_UNLIT // Need to be define before including Material.hlsl

    //-------------------------------------------------------------------------------------
    // Include
    //-------------------------------------------------------------------------------------

    #include "CoreRP/ShaderLibrary/common.hlsl"
    #include "HDRP/ShaderConfig.cs.hlsl"
    #include "HDRP/ShaderVariables.hlsl"
    #include "HDRP/ShaderPass/FragInputs.hlsl"
    #include "HDRP/ShaderPass/ShaderPass.cs.hlsl"    

    //-------------------------------------------------------------------------------------
    // variable declaration
    //-------------------------------------------------------------------------------------

    #include "Assets/Demo/VFX/Shaders/LightShaft/LightShaftProperties.hlsl"

    // All our shaders use same name for entry point
    #pragma vertex Vert
    #pragma fragment Frag

    ENDHLSL

    SubShader
    {
        Tags { "RenderType"="Opaque" "PerformanceChecks"="False" }
        LOD 300

        /*Pass
        {
            Name "Debug"
            Tags { "LightMode" = "DebugViewMaterial" }

            Cull[_CullMode]

            HLSLPROGRAM

            #define SHADERPASS SHADERPASS_DEBUG_VIEW_MATERIAL
            #include "HDRenderPipeline/Material/Material.hlsl"
            #include "ShaderPass/LightShaftPass.hlsl"            
            #include "LightShaftData.hlsl"
            #include "HDRP/ShaderPass/ShaderPassDebugViewMaterial.hlsl"

            ENDHLSL
        }
			*/

		// Forward rendering for blended
        Pass
        {
            Name "ForwardUnlit"
            Tags { "LightMode" = "Forward" }

            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]
            Cull [_CullMode]

            HLSLPROGRAM

            #define SHADERPASS SHADERPASS_FORWARD_UNLIT
			#include "HDRP/Material/Material.hlsl"
            #include "ShaderPass/LightShaftPass.hlsl"
            #include "LightShaftData.hlsl"
			#include "HDRP/ShaderPass/ShaderPassForwardUnlit.hlsl"

            ENDHLSL
        }

    }

    CustomEditor "Experimental.Rendering.HDPipeline.LightShaftShaderGUI"
}
