#ifndef DEMO_RP_LIGHTING_HLSL
#define DEMO_RP_LIGHTING_HLSL

struct Light
{
    float3 color;
    float3 direction;
};

CBUFFER_START(_DemoRPLight)
    float3 _DirectionalLightColor;
    float3 _DirectionalLightDirection;
CBUFFER_END

Light GetDirectionalLight()
{
    Light light;
    light.color = _DirectionalLightColor;
    light.direction = _DirectionalLightDirection;
    return light;
}

#endif