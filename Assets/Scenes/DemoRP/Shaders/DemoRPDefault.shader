Shader "Demo RP/Default" 
{
    Properties
    {
        // Define a property with HLSL name "_MainColor", display name "Color", type Color, and default value of white
        _MainColor ("Color", Color) = (1, 1, 1, 1) 
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
        
        Pass 
        {
            Name "Demo RP Depth"
			Tags{ "LightMode" = "DemoRPDepthOnly" }
            
            HLSLPROGRAM

            #pragma vertex VS
            #pragma fragment PS

            #include "DemoRPDefaultDepthOnlyPass.hlsl"
            
            ENDHLSL
        }  
    }
}
