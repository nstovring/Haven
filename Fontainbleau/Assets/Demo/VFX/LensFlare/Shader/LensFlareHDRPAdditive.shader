Shader "HDRenderPipeline/LensFlare (HDRP Additive)"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
        _OccludedSizeScale("Occluded Size scale", Float) = 1.0
	}
	SubShader
	{
		Pass
		{
			Name "ForwardUnlit"
			Tags{ "LightMode" = "Forward"  "RenderQueue" = "Transparent" }

			Blend One One
			ZWrite Off
			Cull Off
			ZTest Always

			HLSLPROGRAM

			#pragma target 5.0
			#pragma vertex vert
			#pragma fragment frag


			#include "Core/ShaderLibrary/common.hlsl"
			#include "HDRenderPipeline/ShaderConfig.cs.hlsl"
			#include "HDRenderPipeline/ShaderVariables.hlsl"
			#include "LensFlareHDRPCommon.hlsl"

			float4 frag (v2f i) : SV_Target
			{
				float4 col = tex2D(_MainTex, i.uv);
				return col * i.color;
			}

			ENDHLSL
		}
	}
}
