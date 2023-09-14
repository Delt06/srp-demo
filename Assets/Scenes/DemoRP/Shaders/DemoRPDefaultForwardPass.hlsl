#ifndef DEMO_RP_DEFAULT_FORWARD_PASS_HLSL
#define DEMO_RP_DEFAULT_FORWARD_PASS_HLSL

#include "../ShaderLibrary/Common.hlsl"

struct VertexAttributes
{
    float3 positionOS : POSITION;
};

struct V2F
{
    float4 positionCS : SV_POSITION;
};

V2F VS(const in VertexAttributes IN)
{
    V2F OUT;

    const float3 positionWS =  TransformObjectToWorld(IN.positionOS);
    OUT.positionCS = TransformWorldToHClip(positionWS);

    return OUT;
    
}

float4 PS(in V2F IN) : SV_Target
{
    return 0;
}

#endif