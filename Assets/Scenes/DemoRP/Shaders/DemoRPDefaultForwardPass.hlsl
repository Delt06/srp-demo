#ifndef DEMO_RP_DEFAULT_FORWARD_PASS_HLSL
#define DEMO_RP_DEFAULT_FORWARD_PASS_HLSL

// Common includes boilerplate to access common SRP functions and access Unity-specific uniforms (e.g. matrices) 
#include "../ShaderLibrary/Common.hlsl"
#include "../ShaderLibrary/Lighting.hlsl"

struct VertexAttributes
{
    float3 positionOS : POSITION;
    float3 normalOS : NORMAL;
};

struct V2F
{
    float4 positionCS : SV_POSITION;
    float3 normalWS : NORMAL;
};

// Put the shader properties in the UnityPerMaterial cbuffer for SRP batcher compatibility
CBUFFER_START(UnityPerMaterial)
float4 _MainColor;
CBUFFER_END

V2F VS(const in VertexAttributes IN)
{
    V2F OUT;

    const float3 positionWS = TransformObjectToWorld(IN.positionOS);
    OUT.positionCS = TransformWorldToHClip(positionWS);
    OUT.normalWS = TransformWorldToObjectNormal(IN.normalOS);

    return OUT;
    
}

float4 PS(in V2F IN) : SV_Target
{
    const float3 normalWS = normalize(IN.normalWS);
    const float3 albedo = _MainColor.rgb;
    const Light light = GetDirectionalLight();
    const float nDotL = saturate(dot(normalWS, light.direction));
    return float4(albedo * light.color * nDotL, 1.0f);
}

#endif