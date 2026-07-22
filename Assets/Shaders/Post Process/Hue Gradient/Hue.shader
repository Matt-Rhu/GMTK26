Shader "Hidden/Matt-Rhu/Hue Gradient"
{
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment Frag

            #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
            #include "Assets/Shaders/Utilities/ColourUtilities.hlsl"
            
            TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);

            float _Intensity;
            float _Scale;
            float _Offset;
            float _Pow;
            
            float4 Frag (VaryingsDefault i) : SV_Target
            {
                float3 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
                float3 defaultCol = col;
                col = RGBtoHSV(col);

                float uvY = i.texcoord.y * 2 - 1;
                uvY = pow(abs(uvY), _Pow) * FastSign(uvY);
                col.x += uvY * _Scale + _Offset;

                col = HSVtoRGB(col);
                return float4(lerp(defaultCol, col, _Intensity), 1);
            }
            ENDHLSL
        }
    }   
}
