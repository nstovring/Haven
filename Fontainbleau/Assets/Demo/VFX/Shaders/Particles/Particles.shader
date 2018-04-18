Shader "HDRenderPipeline/Particles"
{
    Properties
    {
        _ColorMap("ColorMap", 2D) = "white" {}
        _Brightness("Brightness", float) = 1.0

        _DistortionVectorMap("DistortionVectorMap", 2D) = "black" {}
        _DistortionIntensity("DistortionIntensity", float) = 1.0

        _SoftParticleInvDistance("_SoftParticleInvDistance", float) = 1.0

		_CameraFadeNearDistance("Camera Fade Near Distance", float) = 0.0
		_CameraFadeFarDistance("Camera Fade Far Distance", float) = 1.0

        [ToggleOff] _DistortionEnable("Enable Distortion", Float) = 0.0
        [ToggleOff] _DistortionOnly("Distortion Only", Float) = 0.0
        [ToggleOff] _DistortionDepthTest("Distortion Depth Test Enable", Float) = 0.0

        [ToggleOff]  _AlphaCutoffEnable("Alpha Cutoff Enable", Float) = 0.0
        _AlphaCutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

        // Blending state
        [HideInInspector] _SurfaceType("__surfacetype", Float) = 0.0
        [HideInInspector] _BlendMode("__blendmode", Float) = 0.0
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

    #pragma shader_feature _ALPHATEST_ON
    #pragma shader_feature _DISTORTION_ON
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

    #include "Assets/Demo/VFX/Shaders/Particles/ParticlesProperties.hlsl"

    // All our shaders use same name for entry point
    #pragma vertex Vert
    #pragma fragment Frag

    ENDHLSL

    SubShader
    {
        Tags { "RenderType"="Opaque" "PerformanceChecks"="False" }
        LOD 300
			/*
        Pass
        {
            Name "Debug"
            Tags { "LightMode" = "DebugViewMaterial" }

            Cull[_CullMode]

            HLSLPROGRAM

            #define SHADERPASS SHADERPASS_DEBUG_VIEW_MATERIAL
            #include "HDRenderPipeline/Material/Material.hlsl"
            #include "ShaderPass/ParticlesPass.hlsl"            
            #include "ParticlesData.hlsl"
            #include "HDRP/ShaderPass/ShaderPassDebugViewMaterial.hlsl"

            ENDHLSL
        }
		*/
			Pass
		{
			Name "Distortion" // Name is not used
			Tags{"LightMode" = "DistortionVectors"} // This will be only for transparent object based on the RenderQueue index

			Blend One One
			ZTest[_ZTestMode]
			ZWrite off
			Cull[_CullMode]

			HLSLPROGRAM

			#define SHADERPASS SHADERPASS_DISTORTION
			#include "HDRP/Material/Material.hlsl"
			#include "ShaderPass/ParticlesDistortionPass.hlsl"
			#include "ParticlesData.hlsl"
			#include "HDRP/ShaderPass/ShaderPassDistortion.hlsl"

			ENDHLSL
		}


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
            #include "ShaderPass/ParticlesPass.hlsl"
            #include "ParticlesData.hlsl"
			#include "HDRP/ShaderPass/ShaderPassForwardUnlit.hlsl"

            ENDHLSL
        }

        // Unlit opaque material need to be render with ForwardOnlyOpaque. Unlike Lit that can be both deferred and forward, 
        // unlit require to be forward only, that's why we need this pass. Unlit transparent will use regular Forward pass
        // (Code is exactly the same as "Forward", it simply allow our system to filter objects correctly)
        Pass
        {
            Name "ForwardUnlit"
            Tags { "LightMode" = "ForwardOnlyOpaque" }

            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]
            Cull [_CullMode]

            HLSLPROGRAM

            #define SHADERPASS SHADERPASS_FORWARD_UNLIT
			#include "HDRP/Material/Material.hlsl"
			#include "ShaderPass/ParticlesPass.hlsl"
			#include "ParticlesData.hlsl"
			#include "HDRP/ShaderPass/ShaderPassForwardUnlit.hlsl"

            ENDHLSL
        }
    }

    CustomEditor "Experimental.Rendering.HDPipeline.ParticlesShaderGUI"
}
