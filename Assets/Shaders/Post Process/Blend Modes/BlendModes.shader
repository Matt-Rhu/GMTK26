Shader "Hidden/Matt-Rhu/Blend Modes"
{
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        HLSLINCLUDE
        
		#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
		
		TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        TEXTURE2D_SAMPLER2D(_Tex, sampler_Tex);

        float4 _Colour;
        float _Strength;
        int _BlendType;

		float4 GetBlendValue(float2 uv)
		{
			if (_BlendType == 0)
				return _Colour;
			else if (_BlendType == 1)
				return _Tex.Sample(sampler_Tex, uv);
			else
				return _MainTex.Sample(sampler_MainTex, uv);
		}

		float Luminance(float3 colour)
		{
			return dot(colour, float3(0.299f, 0.587f, 0.114f));
		}

		float3 HueToRGB(float hue)
		{
			hue = frac(hue);
			
			float r = abs(hue * 6 - 3) - 1;
			float g = 2 - abs(hue * 6 - 2);
			float b = 2 - abs(hue * 6 - 4);
			float3 rgb = float3(r, g, b);
			rgb = saturate(rgb);
			
			return rgb;
		}
		
		float3 HSVtoRGB(float3 hsv)
		{
			float3 rgb = HueToRGB(hsv.x);
			rgb = lerp(1, rgb, hsv.y);
			rgb = rgb * hsv.z;

			return rgb;
		}
		
		float3 RGBtoHSV(float3 rgb)
		{
			float maxComponent = max(rgb.r, max(rgb.g, rgb.b));
			float minComponent = min(rgb.r, min(rgb.g, rgb.b));
			float diff = maxComponent - minComponent;

			float hue = 0;

			if (maxComponent == rgb.r)
				hue = 0 + (rgb.g - rgb.b) / diff;
			else if (maxComponent == rgb.g)
				hue = 2 + (rgb.b - rgb.r) / diff;
			else if (maxComponent == rgb.b)
				hue = 4 + (rgb.r - rgb.g) / diff;

			hue = frac(hue / 6);
			float saturation = diff/maxComponent;
			float value = maxComponent;

			return float3(hue, saturation, value);
		}
		
		ENDHLSL
        
		//No Blend
		Pass
		{
			HLSLPROGRAM

			#pragma vertex VertDefault
			#pragma fragment Frag
            
			float4 Frag(VaryingsDefault IN) : SV_Target
			{
				return _MainTex.Sample(sampler_MainTex, IN.texcoord);
			}
			
			ENDHLSL
		}

		//Darken
        Pass
        {
            HLSLPROGRAM

			#pragma vertex VertDefault
			#pragma fragment Frag
            
			float4 Frag(VaryingsDefault IN) : SV_Target
			{
				float4 a = _MainTex.Sample(sampler_MainTex, IN.texcoord);
				float4 b = GetBlendValue(IN.texcoord);

				float3 blended = 1;

				float c = Luminance(a.rgb);
				float d = Luminance(b.rgb);

				if (c > d)
					blended = b.rgb;
				else
					blended = a.rgb;
				
				return saturate(float4(lerp(a.rgb, blended, _Strength), a.a));
			}
			
			ENDHLSL
		}

		//Multiply
		Pass
		{
			HLSLPROGRAM

			#pragma vertex VertDefault
			#pragma fragment Frag
            
			float4 Frag(VaryingsDefault IN) : SV_Target
			{
				float4 a = _MainTex.Sample(sampler_MainTex, IN.texcoord);
				float4 b = GetBlendValue(IN.texcoord);
				b -= 0.001;

				return saturate(float4(lerp(a.rgb, a.rgb * b.rgb, _Strength), a.a));
			}
			
			ENDHLSL
		}

		//Colour Burn
        Pass
        {
            HLSLPROGRAM

			#pragma vertex VertDefault
			#pragma fragment Frag
            
			float4 Frag(VaryingsDefault IN) : SV_Target
			{
				float4 a = _MainTex.Sample(sampler_MainTex, IN.texcoord);
				float4 b = GetBlendValue(IN.texcoord);
				b += 0.001;

				float4 blended = 1 - (1 - a) / b;
				blended = saturate(blended);
				
				return saturate(float4(lerp(a.rgb, blended.rgb, _Strength), a.a));
			}
			
			ENDHLSL
		}

		//Linear Burn
        Pass
        {
            HLSLPROGRAM

			#pragma vertex VertDefault
			#pragma fragment Frag
            
			float4 Frag(VaryingsDefault IN) : SV_Target
			{
				float4 a = _MainTex.Sample(sampler_MainTex, IN.texcoord);
				float4 b = GetBlendValue(IN.texcoord);
				b += 0.001;

				float3 blended = saturate(a.rgb + b.rgb - 1);
				
				return saturate(float4(lerp(a.rgb, blended.rgb, _Strength), a.a));
			}
			
			ENDHLSL
		}
        
		//Lighten
        Pass
        {
            HLSLPROGRAM

			#pragma vertex VertDefault
			#pragma fragment Frag
            
			float4 Frag(VaryingsDefault IN) : SV_Target
			{
				float4 a = _MainTex.Sample(sampler_MainTex, IN.texcoord);
				float4 b = GetBlendValue(IN.texcoord);

				float3 blended = 1;

				float c = Luminance(a.rgb);
				float d = Luminance(b.rgb);

				if (c > d)
					blended = a.rgb;
				else
					blended = b.rgb;
				
				return saturate(float4(lerp(a.rgb, blended, _Strength), a.a));
			}
			
			ENDHLSL
		}

		//Screen
		Pass
		{
			HLSLPROGRAM

			#pragma vertex VertDefault
			#pragma fragment Frag
            
			float4 Frag(VaryingsDefault IN) : SV_Target
			{
				float4 a = _MainTex.Sample(sampler_MainTex, IN.texcoord);
				float4 b = GetBlendValue(IN.texcoord);
				b -= 0.001;

				float3 blended = 1 - (1 - a.rgb) * (1 - b.rgb);

				return saturate(float4(lerp(a.rgb, blended, _Strength), a.a));
			}
			
			ENDHLSL
		}

		//Colour Dodge
        Pass
        {
            HLSLPROGRAM

			#pragma vertex VertDefault
			#pragma fragment Frag
            
			float4 Frag(VaryingsDefault IN) : SV_Target
			{
				float4 a = _MainTex.Sample(sampler_MainTex, IN.texcoord);
				float4 b = GetBlendValue(IN.texcoord);
				b -= 0.001;

				float4 blended = a / (1 - b);
				blended = saturate(blended);
				
				return saturate(float4(lerp(a.rgb, blended.rgb, _Strength), a.a));
			}
			
			ENDHLSL
		}

		//Linear Dodge
        Pass
        {
            HLSLPROGRAM

			#pragma vertex VertDefault
			#pragma fragment Frag
            
			float4 Frag(VaryingsDefault IN) : SV_Target
			{
				float4 a = _MainTex.Sample(sampler_MainTex, IN.texcoord);
				float4 b = GetBlendValue(IN.texcoord);
				b += 0.001;

				float3 blended = saturate(a.rgb + b.rgb);
				
				return saturate(float4(lerp(a.rgb, blended.rgb, _Strength), a.a));
			}
			
			ENDHLSL
		}

		//Overlay
		Pass
		{
			HLSLPROGRAM

			#pragma vertex VertDefault
			#pragma fragment Frag
            
			float4 Frag(VaryingsDefault IN) : SV_Target
			{
				float4 a = _MainTex.Sample(sampler_MainTex, IN.texcoord);
				float4 b = GetBlendValue(IN.texcoord);
				b -= 0.001;

				float3 blended = 1;

				if (Luminance(a.rgb) < 0.5)
					blended = 2 * a.rgb * b.rgb;
				else
					blended = 1 - 2 * (1 - a.rgb) * (1 - b.rgb);

				return saturate(float4(lerp(a.rgb, blended, _Strength), a.a));
			}
			
			ENDHLSL
		}

		//Soft Light
		Pass
		{
			HLSLPROGRAM

			#pragma vertex VertDefault
			#pragma fragment Frag
            
			float4 Frag(VaryingsDefault IN) : SV_Target
			{
				float4 a = _MainTex.Sample(sampler_MainTex, IN.texcoord);
				float4 b = GetBlendValue(IN.texcoord);
				b -= 0.001;

				float4 blended = 1;

				float l = Luminance(b.rgb);
				if (l < 0.5)
					blended = 2 * a * b + a * a * (1 - 2 * b);
				else
					blended = 2 * a * (1 - b) + sqrt(a) * (2 * b - 1);
				
				return saturate(float4(lerp(a.rgb, blended.rgb, _Strength), a.a));
			}
			
			ENDHLSL
		}	

		//Hard Light
		Pass
		{
			HLSLPROGRAM

			#pragma vertex VertDefault
			#pragma fragment Frag
            
			float4 Frag(VaryingsDefault IN) : SV_Target
			{
				float4 a = _MainTex.Sample(sampler_MainTex, IN.texcoord);
				float4 b = GetBlendValue(IN.texcoord);
				b -= 0.001;

				float3 blended = 1;

				if (Luminance(b.rgb) < 0.5)
					blended = 2 * a.rgb * b.rgb;
				else
					blended = 1 - 2 * (1 - a.rgb) * (1 - b.rgb);

				return saturate(float4(lerp(a.rgb, blended, _Strength), a.a));
			}
			
			ENDHLSL
		}	
        
		//Vivid Light
        Pass
        {
            HLSLPROGRAM

			#pragma vertex VertDefault
			#pragma fragment Frag
            
			float4 Frag(VaryingsDefault IN) : SV_Target
			{
				float4 a = _MainTex.Sample(sampler_MainTex, IN.texcoord);
				float4 b = GetBlendValue(IN.texcoord);

				float3 blended = 1;

				if (Luminance(b.rgb) <= 0.5)
				{
					b += 0.001;
					blended = 1 - ((1 - a.rgb) / (2 * b.rgb));
				}
				else
				{
					b -= 0.001f;
					blended = a.rgb / (2 * (1 - b.rgb));
				}
				
				return saturate(float4(lerp(a.rgb, blended, _Strength), a.a));
			}
			
			ENDHLSL
		}

		//Linear Light
        Pass
        {
            HLSLPROGRAM

			#pragma vertex VertDefault
			#pragma fragment Frag
            
			float4 Frag(VaryingsDefault IN) : SV_Target
			{
				float4 a = _MainTex.Sample(sampler_MainTex, IN.texcoord);
				float4 b = GetBlendValue(IN.texcoord);

				float3 blended = 1;

				blended = (Luminance(b.rgb) <= 0.5) * saturate(a + 2 * b - 1) + (Luminance(b.rgb) > 0.5) * saturate(a + 2 * (b - 0.5));
					
				return saturate(float4(lerp(a.rgb, blended, _Strength), a.a));
			}
			
			ENDHLSL
		}
		
		//Pin Light
        Pass
        {
            HLSLPROGRAM

			#pragma vertex VertDefault
			#pragma fragment Frag
            
			float4 Frag(VaryingsDefault IN) : SV_Target
			{
				float4 a = _MainTex.Sample(sampler_MainTex, IN.texcoord);
				float4 b = GetBlendValue(IN.texcoord);

				float3 blended = 1;

				float c = Luminance(a.rgb);
				float d = Luminance(b.rgb);

				if (d > 0.5)
				{
					d = Luminance(2 * (b.rgb - 0.5));

					if (c > d)
						blended = a.rgb;
					else
						blended = 2 * (b.rgb - 0.5);
				}
				else
				{
					d = Luminance(2 * b.rgb);

					if (c > d)
						blended = 2 * b.rgb;
					else
						blended = a.rgb;
				}
				
				return saturate(float4(lerp(a.rgb, blended, _Strength), a.a));
			}
			
			ENDHLSL
		}

		//Hue
        Pass
        {
            HLSLPROGRAM

			#pragma vertex VertDefault
			#pragma fragment Frag
            
			float4 Frag(VaryingsDefault IN) : SV_Target
			{
				float4 a = _MainTex.Sample(sampler_MainTex, IN.texcoord);
				float4 b = GetBlendValue(IN.texcoord);
				
				float3 c = RGBtoHSV(a.rgb);
				float3 d = RGBtoHSV(b.rgb);

				float3 blended = HSVtoRGB(float3(d.x, c.yz));

				return saturate(float4(lerp(a.rgb, blended, _Strength), a.a));
			}
			
			ENDHLSL
		}

		//Hue
        Pass
        {
            HLSLPROGRAM

			#pragma vertex VertDefault
			#pragma fragment Frag
            
			float4 Frag(VaryingsDefault IN) : SV_Target
			{
				float4 a = _MainTex.Sample(sampler_MainTex, IN.texcoord);
				float4 b = GetBlendValue(IN.texcoord);
				
				float3 c = RGBtoHSV(a.rgb);
				float3 d = RGBtoHSV(b.rgb);

				float3 blended = HSVtoRGB(float3(c.x, d.y, c.z));

				return saturate(float4(lerp(a.rgb, blended, _Strength), a.a));
			}
			
			ENDHLSL
		}

		//Colour
        Pass
        {
            HLSLPROGRAM

			#pragma vertex VertDefault
			#pragma fragment Frag
            
			float4 Frag(VaryingsDefault IN) : SV_Target
			{
				float4 a = _MainTex.Sample(sampler_MainTex, IN.texcoord);
				float4 b = GetBlendValue(IN.texcoord);
				
				float3 c = RGBtoHSV(a.rgb);
				float3 d = RGBtoHSV(b.rgb);

				float3 blended = HSVtoRGB(float3(d.xy, c.z));

				return saturate(float4(lerp(a.rgb, blended, _Strength), a.a));
			}
			
			ENDHLSL
		}

		//Value
        Pass
        {
            HLSLPROGRAM

			#pragma vertex VertDefault
			#pragma fragment Frag
            
			float4 Frag(VaryingsDefault IN) : SV_Target
			{
				float4 a = _MainTex.Sample(sampler_MainTex, IN.texcoord);
				float4 b = GetBlendValue(IN.texcoord);
				
				float3 c = RGBtoHSV(a.rgb);
				float3 d = RGBtoHSV(b.rgb);

				float3 blended = HSVtoRGB(float3(c.xy, d.z));

				return saturate(float4(lerp(a.rgb, blended, _Strength), a.a));
			}
			
			ENDHLSL
		}
    }
}