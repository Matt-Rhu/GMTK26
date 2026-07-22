Shader "Hidden/Matt-Rhu/Speed Blur Post Process"
{
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {        	
            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment Frag

            #pragma shader_feature DEBUG_MASK
            #pragma multi_compile __ DEPTH

			#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

			TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
			TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);       

			float _ScreenRatio;
            
            float _Scale;
            int _Samples;
            
            float _MinWeight;
            float _WeightPower;

            float _MaskRoundness;
            float _MaskSize;
            float _MaskSoftness;
            
            float2 _Offset;
            float _Rotate;

            float _DepthMult;
            float _DepthWeight;
                        
			float Mask(float2 uv)
			{
				float2 maskCoord = uv - 0.5;
				maskCoord.x *= lerp(1, _ScreenRatio, _MaskRoundness);
				float mask = length(maskCoord);
				
				mask += _MaskSize * -1;
				mask = smoothstep(0, _MaskSoftness, mask);

				return mask;
			}

			float2 Rotator(float2 uv, float amount)
			{
				//variant of ASE's Rotator node, modified to preserve aspect ratio during rotation instead of stretching the rotated texture
				float rotat = amount * PI * 2; //conversion from radians to number of rotations
				float cos3 = cos( rotat );
				float sin3 = sin( rotat );
				float2 rotator = mul( float2(uv.x * _ScreenRatio, uv.y) - float2( 0.5 * _ScreenRatio, 0.5 ), float2x2( cos3, -sin3, sin3, cos3 )) + float2( 0.5 * _ScreenRatio, 0.5 );
				rotator.x /= _ScreenRatio;

				return rotator;
			}
            
			float4 Frag(VaryingsDefault i) : SV_Target
			{
			    //return pow(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoord) * 10000, 2);	
								
				#if DEBUG_MASK
					return Mask(i.texcoord);
				#endif
				
				float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);

				bool test = (color.b > color.g);
								
				//do nothing if no offset
				if(_Scale == 0.5) return color;

				float4 sampleColor = 0;
				float sum = 0;
								
				for (int ind = 1; ind <= _Samples; ind++)
				{
					const float scale = 0.5 - (0.5 - _Scale) / _Samples * ind;
					float2 coord = (i.texcoord - 0.5) * 2 * scale + (0.5 - (0.5 - _Offset) / _Samples * ind);
					coord = Rotator(coord, 0 - (0 - _Rotate) / _Samples * ind);
					
					//WITHOUT offset bullshit
					//float2 coord = (i.texcoord - 0.5) * 2 * scale + 0.5;

					#if DEPTH
						//sample depth so distant things are less blurry
						float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, coord).r * _DepthMult;
						depth = (1 + depth * _DepthWeight) / (1 + _DepthWeight);
					#else
						float depth = 1;
					#endif
					
					const float weightParamter = (scale - _Scale) / (0.5 - _Scale);
					const float weight = lerp(_MinWeight, 1, pow(weightParamter, _WeightPower)) * Mask(i.texcoord) * saturate(depth);
					sum += weight;

					sampleColor += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, coord) * weight;
				}
				const float4 blurred = (color*2 + sampleColor) / (sum + 2);

				return blurred;
			}
			ENDHLSL
		}
    }
}