#ifndef SHADERPASS
#error Undefine_SHADERPASS
#endif

#define ATTRIBUTES_NEED_TEXCOORD0
#define ATTRIBUTES_NEED_NORMAL
#define ATTRIBUTES_NEED_TANGENT


#define VARYINGS_NEED_TEXCOORD0
#define VARYINGS_NEED_TANGENT_TO_WORLD
#define VARYINGS_NEED_POSITION_WS

// This include will define the various Attributes/Varyings structure
#include "HDRP/ShaderPass/VaryingMesh.hlsl"
