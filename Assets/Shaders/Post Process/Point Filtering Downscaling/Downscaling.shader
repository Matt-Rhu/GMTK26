Shader "Hidden/Matt-Rhu/Downscaling Post Process"
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
            
			Texture2D _MainTex;
            SamplerState point_clamp_sampler;
                        
			float4 Frag(VaryingsDefault IN) : SV_Target
			{
				float4 color = SAMPLE_TEXTURE2D(_MainTex, point_clamp_sampler, IN.texcoord);
				
				return color;
			}
			ENDHLSL
		}
    }
}