Shader "Hidden/Matt-Rhu/Blur Post Process"
{
    //Original tutorial: https://www.ronja-tutorials.com/post/023-postprocessing-blur/
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        HLSLINCLUDE
            // shader_feature would be better if I had a way to set the keyword before compilation
            //#pragma shader_feature GAUSS
            #pragma multi_compile __ GAUSS

            #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

            TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
            TEXTURE2D_SAMPLER2D(_Mask, sampler_Mask);

            float _BlurSize;
            int _BlurSamples;
            float _StandardDeviation;

            #define E 2.71828182846
        ENDHLSL

        Pass
        {
            Name "Vertical Blur"

            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment Frag

            
            float4 Frag(VaryingsDefault i) : SV_Target
            {
                #if GAUSS
					if (_StandardDeviation == 0) return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
                #endif

                float4 col = 0;

                #if GAUSS
					float sum = 0;
                #else
                float sum = _BlurSamples;
                #endif

                float mask = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.texcoord).r;
                _BlurSize = _BlurSize * mask;

                for (float index = 0; index < _BlurSamples; index++)
                {
                    float offset = (index / (_BlurSamples - 1) - 0.5) * _BlurSize;
                    //get uv coordinate of sample
                    float2 uv = i.texcoord + float2(0, offset);
                    #if !GAUSS
                    col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                    #else
					    float stDevSquared = _StandardDeviation * _StandardDeviation;
						float gauss = (1 / sqrt(2 * PI * stDevSquared)) * pow(E, -((offset * offset) / (2 * stDevSquared)));
						sum += gauss;
						//multiply color with influence from gaussian function and add it to sum color
						col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv) * gauss;
                    #endif
                }
                col = col / sum;

                return col;
            }
            ENDHLSL
        }

        Pass
        {
            Name "Horizontal Blur"

            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment Frag

            float4 Frag(VaryingsDefault i) : SV_Target
            {
                float invAspect = _ScreenParams.y / _ScreenParams.x;

                #if GAUSS
					if (_StandardDeviation == 0) return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
                #endif

                float4 col = 0;

                #if GAUSS
					float sum = 0;
                #else
                float sum = _BlurSamples;
                #endif

                float mask = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.texcoord).r;
                _BlurSize = _BlurSize * mask;

                for (float index = 0; index < _BlurSamples; index++)
                {
                    float offset = (index / (_BlurSamples - 1) - 0.5) * _BlurSize * invAspect;
                    //get uv coordinate of sample
                    float2 uv = i.texcoord + float2(offset, 0);
                    #if !GAUSS
                    col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                    #else
					    float stDevSquared = _StandardDeviation * _StandardDeviation;
						float gauss = (1 / sqrt(2 * PI * stDevSquared)) * pow(E, -((offset * offset) / ( 2 * stDevSquared)));
						sum += gauss;
						//multiply color with influence from gaussian function and add it to sum color
						col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv) * gauss;
                    #endif
                }
                col = col / sum;

                return col;
            }
            ENDHLSL
        }
    }
}