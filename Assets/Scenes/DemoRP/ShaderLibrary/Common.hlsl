#ifndef DEMO_RP_COMMON_HLSL
#define DEMO_RP_COMMON_HLSL

#include "UnityInput.hlsl"

float3 TransformObjectToWorld(float3 positionOs)
{
    return mul(unity_ObjectToWorld, float4(positionOs, 1.0f)).xyz;
}

float4 TransformWorldToHClip(float3 positionWs)
{
    return mul(unity_MatrixVP, float4(positionWs, 1.0f));
}

#endif
