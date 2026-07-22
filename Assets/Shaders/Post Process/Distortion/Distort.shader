Shader "Hidden/Matt-Rhu/Distort"
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
            #include "Assets/Shaders/InvLerp+Remap.hlsl"

            #pragma multi_compile __ DISTORT

			TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
            float4 _Distortion_Amount;
            float _EdgeSmooth;

            //Unity's default distortion function, stripped down
            float2 Distort(float2 uv)
            {
				uv = (uv - 0.5) * _Distortion_Amount.z + 0.5;
                float2 ruv = (uv - 0.5);
                float ru = length(float2(ruv));
            	
                if (_Distortion_Amount.w > 0.0)
                {
                    float wu = ru * _Distortion_Amount.x;
                    ru = tan(wu) * (1.0 / (ru * _Distortion_Amount.y));
                    uv = uv + ruv * (ru - 1.0);
                }
                else
                {
                    ru = (1.0 / ru) * _Distortion_Amount.x * atan(ru * _Distortion_Amount.y);
                    uv = uv + ruv * (ru - 1.0);
                }

                return uv;
            }
                        
			float4 Frag(VaryingsDefault i) : SV_Target
			{
				float2 uv = 0;
				#if DISTORT
					uv = Distort(i.texcoord);
				#else
					uv = i.texcoord;
				#endif
				
				float2 smoothing = abs((uv - 0.5) * 2);
				smoothing = RemapClamped(1 - _EdgeSmooth, 1, 1, 0, smoothing);

				//none of these two is ideal, but it doesn't show that much
				const float smooth = smoothing.x * smoothing.y;
				//const float smooth = min(smoothing.x, smoothing.y);
				
				float4 col = lerp(0, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv), smooth * smooth);
				
				return col;
			}
			ENDHLSL
		}
    }
}