Shader "Demo RP/Default" 
{
    Properties
    {
    }
    
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        
        Pass 
        {
            Name "Demo RP Default"
			Tags{ "LightMode" = "DemoRPForward" }
            
            HLSLPROGRAM

            #pragma vertex VS
            #pragma fragment PS

            #include "DemoRPDefaultForwardPass.hlsl"
            
            ENDHLSL
        }   
    }
}
