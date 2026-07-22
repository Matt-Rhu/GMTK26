Shader "Hidden/Matt-Rhu/CRT"
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
            #include "Assets/Shaders/Utilities/Lerps.hlsl"
            #include "Assets/Shaders/Utilities/ColourUtilities.hlsl"
            
			Texture2D _MainTex;
            SamplerState point_clamp_sampler;
            
            float _BlurAmount;
            float4 _BloomColour;
            
            int _LinesTiling;
            float _LinesOpacity;
            float _LinesSpeed;

            int _PointsTiling;
            float _PointsSize;
            float _PointsSoftness;
            float _PointsOpacity;
            float _PointsRatio;

            float _WavesTiling;
            float _WavesSpeed;
            float _WavesStrength;

            float _UnscaledTime;

            
			float Lines(float uvY)
			{
				uvY += _UnscaledTime * _LinesSpeed;
				uvY *= _LinesTiling * 2;

				return lerp(1, step(1, uvY % 2), _LinesOpacity);
			}
            
			float Mask(float2 uv)
			{
				uv.x *= _ScreenParams.x / _ScreenParams.y;
				uv.y *= _PointsRatio;
				float2 maskCoord = frac(uv * _PointsTiling) - 0.5;
				
				float mask = length(maskCoord);
							
				mask += _PointsSize * -1;
				mask = smoothstep(0, _PointsSoftness, mask);

				return lerp(1, 1-mask, _PointsOpacity);
			}

            float Waves(float uvY)
			{
				uvY += _UnscaledTime * _WavesSpeed;
				
				return Remap(-1, 1, 1 - _WavesStrength, 1 + _WavesStrength, sin(uvY * _WavesTiling));
			}
            
			float4 Frag(VaryingsDefault IN) : SV_Target
			{
				float blur =  _BlurAmount * (0.5 + pow(Luminance(_MainTex.Sample(point_clamp_sampler, IN.texcoord).rgb), 1.5)) * 1.5;
							
				float4 color = 0;

				color += _MainTex.Sample(point_clamp_sampler, IN.texcoord);
				color += _MainTex.Sample(point_clamp_sampler, IN.texcoord + float2(0, blur));
				color += _MainTex.Sample(point_clamp_sampler, IN.texcoord - float2(0, blur));
				color += _MainTex.Sample(point_clamp_sampler, IN.texcoord + float2(blur, 0));
				color += _MainTex.Sample(point_clamp_sampler, IN.texcoord - float2(blur, 0));
				//color += float4(_MainTex.Sample(point_clamp_sampler, IN.texcoord + float2(blur, 0)).r, _MainTex.Sample(point_clamp_sampler, IN.texcoord + float2(blur, 0)).g, 0, 1);
				//color += float4(0, _MainTex.Sample(point_clamp_sampler, IN.texcoord - float2(blur, 0)).g,_MainTex.Sample(point_clamp_sampler, IN.texcoord - float2(blur, 0)).b, 1);

				color /= 5;
				
				return color * _BloomColour * Waves(IN.texcoord.y) * Mask(IN.texcoord) * Lines(IN.texcoord.y);
			}
			ENDHLSL
		}

		Pass 
        {        	
            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment Frag
            
			#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
            #include "Assets/Shaders/Utilities/Lerps.hlsl"

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