#ifndef SHADERPASS
#error Undefine_SHADERPASS
#endif

#define ATTRIBUTES_NEED_TEXCOORD0
#define ATTRIBUTES_NEED_COLOR


#define VARYINGS_NEED_TEXCOORD0
#define VARYINGS_NEED_COLOR

// This include will define the various Attributes/Varyings structure
#include "HDRP/ShaderPass/VaryingMesh.hlsl"
