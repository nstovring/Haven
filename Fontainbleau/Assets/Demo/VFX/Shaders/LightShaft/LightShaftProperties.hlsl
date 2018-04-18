float4 _LightShaftColor;
TEXTURE2D(_AlphaMask);
SAMPLER(sampler_AlphaMask);

float4 _AlphaMaskST;
float4 _AlphaMaskScrollSpeed;

#ifdef _FALLOFF_GRADIENT_ON
TEXTURE2D(_FalloffGradient);
SAMPLER(sampler_FalloffGradient);

#endif

float _FalloffPower;

float _AlphaCutoff;

// _SoftParticleInvDistance tells us if the feature is disabled (<0)
float _SoftParticleInvDistance;

// These two params tells us if the feature is disabled (both = 0)
float _CameraFadeNearDistance;
float _CameraFadeFarDistance;
