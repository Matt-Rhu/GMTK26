Shader "Hidden/Matt-Rhu/Levels"
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
            
            TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
            
            float _MinOld;
            float _MaxOld;
            float _MinNew;
            float _MaxNew;

            float InvLerp (float a, float b, float v)
            {
                return (v - a) / (b - a);
            }
            
            float Remap (float miO, float maO, float miN, float maN, float input)
            {
                const float t = InvLerp(miO, maO, input);
                return lerp(miN, maN, t);
            }
            
            float4 Frag (VaryingsDefault i) : SV_Target
            {
                float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);

                col.r = Remap(_MinOld, _MaxOld, _MinNew, _MaxNew, col.r);
                col.g = Remap(_MinOld, _MaxOld, _MinNew, _MaxNew, col.g);
                col.b = Remap(_MinOld, _MaxOld, _MinNew, _MaxNew, col.b);
                
                return col;
            }
            ENDHLSL
        }
    }
}
