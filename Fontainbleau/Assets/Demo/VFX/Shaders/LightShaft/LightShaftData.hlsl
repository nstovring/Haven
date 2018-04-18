
//-------------------------------------------------------------------------------------
// Fill SurfaceData/Builtin data function
//-------------------------------------------------------------------------------------

/* plundered from BuiltinData.cs.hlsl at some point, probably not up to date anymore...

		struct BuiltinData
		{
			float opacity;
			float3 bakeDiffuseLighting;
			float3 emissiveColor;
			float emissiveIntensity;
			float2 velocity;
			float2 distortion;
			float distortionBlur;
			float depthOffset;
		};

		struct FragInputs
		{
			// Contain value return by SV_POSITION (That is name positionCS in PackedVarying).
			// xy: unormalized screen position (offset by 0.5), z: device depth, w: depth in view space
			// Note: SV_POSITION is the result of the clip space position provide to the vertex shaders that is transform by the viewport
			float4 unPositionSS; // In case depth offset is use, positionWS.w is equal to depth offset
			float3 positionWS;
			float2 texCoord0;
			float2 texCoord1;
			float2 texCoord2;
			float2 texCoord3;
			float3 tangentToWorld[3];
			float4 color; // vertex color

			// For two sided lighting
			bool isFrontFace;
		};

*/

void GetSurfaceAndBuiltinData(FragInputs input, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData)
{
    ZERO_INITIALIZE(BuiltinData, builtinData);

	// Translation, scale
	input.texCoord0.xy = (input.texCoord0.xy * _AlphaMaskST.xy) + _AlphaMaskST.zw;
	input.texCoord0.xy += fmod(_Time.y * (_AlphaMaskScrollSpeed.xy), 1.0f );

	float alpha = SAMPLE_TEXTURE2D(_AlphaMask, sampler_AlphaMask, input.texCoord0).a * _LightShaftColor.a;

#ifdef _ALPHATEST_ON
    clip(alpha - _AlphaCutoff);
#endif

#ifdef _SOFTPARTICLES_ON

	float sampledDepth = LinearEyeDepth(SAMPLE_TEXTURE2D(_MainDepthTexture, sampler_MainDepthTexture, posInput.positionNDC.xy).r, _ZBufferParams);
	alpha *= saturate( (sampledDepth - input.positionSS.w) * _SoftParticleInvDistance);
#endif

#ifdef _CAMERAFADING_ON
	alpha *= saturate((input.positionSS.w - _CameraFadeNearDistance) / max(0.01f, _CameraFadeFarDistance - _CameraFadeNearDistance));
#endif

	float3 cameraVector = GetWorldSpaceNormalizeViewDir(input.positionWS);
	float3 normal = input.worldToTangent[2];
	float falloff = pow(abs(dot(cameraVector, normal)), _FalloffPower);
#ifdef _FALLOFF_GRADIENT_ON
	falloff = SAMPLE_TEXTURE2D(_FalloffGradient, sampler_FalloffGradient, float2(falloff,0)).r;
#endif
	alpha *= falloff;

	/////////////////////////////////////////
	// Surface Data
	surfaceData.color = _LightShaftColor.rgb;

	/////////////////////////////////////////
    // Builtin Data

    builtinData.opacity = alpha;
}
