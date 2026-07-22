// Made with Amplify Shader Editor v1.9.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Distort/Distort_Bulge"
{
	Properties
	{
		_DistortAmount("DistortAmount", Range( -1 , 1)) = 1
		_DistortionMultiplier("DistortionMultiplier", Float) = 0
		_IsParticleSystem("IsParticleSystem", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back ZTest Off ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		GrabPass{ }
		CGPROGRAM
		#pragma target 3.0
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
		#pragma surface surf Unlit keepalpha noshadow 
		struct Input
		{
			float4 screenPos;
			float4 vertexColor : COLOR;
			float2 uv_texcoord;
		};

		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform float _DistortAmount;
		uniform float _DistortionMultiplier;
		uniform float _IsParticleSystem;


		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float lerpResult55 = lerp( _DistortAmount , ( ( i.vertexColor.b - i.vertexColor.r ) * _DistortionMultiplier ) , _IsParticleSystem);
			float2 uv_TexCoord28 = i.uv_texcoord + float2( -0.5,-0.5 );
			float2 CenteredUV15_g1 = ( i.uv_texcoord - float2( 0.5,0.5 ) );
			float2 break17_g1 = CenteredUV15_g1;
			float2 appendResult23_g1 = (float2(( length( CenteredUV15_g1 ) * 1.0 * 2.0 ) , ( atan2( break17_g1.x , break17_g1.y ) * ( 1.0 / 6.28318548202515 ) * 1.0 )));
			float4 screenColor8 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,( (ase_grabScreenPosNorm).xy + ( lerpResult55 * uv_TexCoord28 * saturate( (0.0 + (( 1.0 - (appendResult23_g1).x ) - 0.2) * (1.0 - 0.0) / (0.8 - 0.2)) ) * saturate( (1.0 + (length( uv_TexCoord28 ) - 0.2) * (0.0 - 1.0) / (0.5 - 0.2)) ) ) ));
			o.Emission = screenColor8.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19200
Node;AmplifyShaderEditor.CommentaryNode;52;-1625.221,-229.8576;Inherit;False;100;100;;0;Whenever the shader is edited in ASE, remember to put "ZTest Off ZWrite Off" in the script;1,1,1,1;0;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;3;123.0648,-71.98271;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Distort/Distort_Bulge;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;False;0;True;TransparentCutout;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;3;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.SimpleAddOpNode;6;-368.0161,-10.42051;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ScreenColorNode;8;-126.0161,-13.42051;Inherit;False;Global;_GrabScreen0;Grab Screen 0;1;0;Create;True;0;0;0;False;0;False;Object;-1;False;False;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;5;-652.0161,-10.42051;Inherit;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GrabScreenPosition;4;-899.0161,-9.420514;Inherit;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;42;-2009.593,238.0045;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;-1536.008,355.2725;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;48;-1807.008,415.2725;Inherit;False;Property;_DistortionMultiplier;DistortionMultiplier;1;0;Create;True;0;0;0;False;0;False;0;0.89;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;46;-1748.593,288.0045;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;54;-1601.265,95.39645;Inherit;False;Property;_IsParticleSystem;IsParticleSystem;2;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;55;-821.6387,302.0439;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-1556.736,234.8118;Inherit;False;Property;_DistortAmount;DistortAmount;0;0;Create;True;0;0;0;False;0;False;1;0.55;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;57;-1210.457,594.0449;Inherit;False;698.1183;304.3958;Comment;3;60;59;58;Reduce distortion to zero near the sides;1,1,1,1;0;0
Node;AmplifyShaderEditor.LengthOpNode;58;-1160.457,644.045;Inherit;True;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;59;-971.2898,644.4407;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0.2;False;2;FLOAT;0.5;False;3;FLOAT;1;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;60;-690.3389,743.0668;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;24;-1595.428,1164.219;Inherit;True;True;False;True;True;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;25;-1328.736,1162.75;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;23;-1924.326,1196.026;Inherit;True;Polar Coordinates;-1;;1;7dab8e02884cf104ebefaa2e788e4162;0;4;1;FLOAT2;0,0;False;2;FLOAT2;0.5,0.5;False;3;FLOAT;1;False;4;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TFHCRemapNode;34;-1112.781,1132.734;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0.2;False;2;FLOAT;0.8;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;38;-810.4102,1145.151;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-472.552,343.4963;Inherit;True;4;4;0;FLOAT;0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;28;-1584.114,614.22;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;-0.5,-0.5;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
WireConnection;3;2;8;0
WireConnection;6;0;5;0
WireConnection;6;1;9;0
WireConnection;8;0;6;0
WireConnection;5;0;4;0
WireConnection;47;0;46;0
WireConnection;47;1;48;0
WireConnection;46;0;42;3
WireConnection;46;1;42;1
WireConnection;55;0;10;0
WireConnection;55;1;47;0
WireConnection;55;2;54;0
WireConnection;58;0;28;0
WireConnection;59;0;58;0
WireConnection;60;0;59;0
WireConnection;24;0;23;0
WireConnection;25;0;24;0
WireConnection;34;0;25;0
WireConnection;38;0;34;0
WireConnection;9;0;55;0
WireConnection;9;1;28;0
WireConnection;9;2;38;0
WireConnection;9;3;60;0
ASEEND*/
//CHKSM=25FD1758174DF7EA5572C060B132DEDB105E5B07