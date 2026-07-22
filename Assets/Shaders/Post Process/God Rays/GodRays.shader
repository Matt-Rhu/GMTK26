Shader "Hidden/Matt-Rhu/God Rays Post Process"
{	
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment Frag

            //Set to literally anything else but one for god rays to be able to come from other things than the skybox
            #define SKYBOX_ONLY 1
            
			#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
            
			TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
			TEXTURE2D_SAMPLER2D(_Mask, sampler_Mask);
            #if SKYBOX_ONLY == 1
				TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);       
			#endif
            
            float _Scale;
            int _Samples;
            float _Intensity;
            
            float _MinWeight;
            float _WeightPower;

            float _ThresholdDiv;
            float _ThresholdPow;
            float _ThresholdMult;
            
            float2 _Offset;
            
			float4 Frag(VaryingsDefault i) : SV_Target
			{				
				float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
				
				//do nothing if no offset
				if(_Scale == 0.5) return color;

				float4 sampleColor = 0;
				float sum = 0;
				
				for (int ind = 1; ind <= _Samples; ind++)
				{
					const float scale = 0.5 - (0.5 - _Scale) / _Samples * ind;
					const float2 coord = (i.texcoord - 0.5) * 2 * scale + (0.5 - (0.5 - _Offset) / _Samples * ind);

					//WITHOUT offset bullshit
					//float2 coord = (i.texcoord - 0.5) * 2 * scale + 0.5;

					#if SKYBOX_ONLY == 1
						float depth = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, coord).r;
						if (depth * 10000 == 0)
						{
							float4 colorg = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, coord);
							
							const float valueParameter =  saturate(pow(length(colorg.xyz / _ThresholdDiv), _ThresholdPow) * _ThresholdMult);
							
							const float weightParamter = (scale - _Scale) / (0.5 - _Scale);
							const float weight = lerp(_MinWeight, 1, pow(weightParamter, _WeightPower)) * SAMPLE_TEXTURE2D(_Mask, sampler_Mask, coord).r * valueParameter * _Intensity;
							sum += weight;

							sampleColor += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, coord) * weight;
						}
					#else
						float4 colorg = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, coord);
							
							const float valueParameter =  saturate(pow(length(colorg.xyz / _ThresholdDiv), _ThresholdPow) * _ThresholdMult);
							
							const float weightParamter = (scale - _Scale) / (0.5 - _Scale);
							const float weight = lerp(_MinWeight, 1, pow(weightParamter, _WeightPower)) * SAMPLE_TEXTURE2D(_Mask, sampler_Mask, coord).r * valueParameter * _Intensity;
							sum += weight;

							sampleColor += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, coord) * weight;
					#endif
				}
				
				const float4 blurred = (color*2 + sampleColor) / (sum + 2);

				return blurred;
			}
			ENDHLSL
		}
    }
}