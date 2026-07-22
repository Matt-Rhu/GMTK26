// Made with Amplify Shader Editor v1.9.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Distort/Distort_Ridge"
{
	Properties
	{
		_DistortAmount("DistortAmount", Range( -1 , 1)) = 0
		_DistortionMultiplier("DistortionMultiplier", Float) = 0
		_LateralFalloff("LateralFalloff", Range( 0 , 5)) = 3
		_LongitudinalFalloff("LongitudinalFalloff", Range( 0 , 5)) = 0.4782608
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
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform float _DistortAmount;
		uniform float _DistortionMultiplier;
		uniform float _LateralFalloff;
		uniform float _LongitudinalFalloff;


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
			float2 uv_TexCoord81 = i.uv_texcoord + float2( -0.5,-0.5 );
			float lerpResult55 = lerp( _DistortAmount , ( ( i.vertexColor.b - i.vertexColor.r ) * _DistortionMultiplier ) , 0.0);
			float Strength75 = lerpResult55;
			float2 uv_TexCoord72 = i.uv_texcoord + float2( -0.5,-0.5 );
			float Distortion130 = ( saturate( pow( ( 1.0 - ( abs( uv_TexCoord72.y ) * 2.0 ) ) , _LateralFalloff ) ) * saturate( pow( ( 1.0 - i.uv_texcoord.x ) , _LongitudinalFalloff ) ) * saturate( pow( (0.0 + (i.uv_texcoord.x - 0.0) * (1.0 - 0.0) / (0.14 - 0.0)) , _LongitudinalFalloff ) ) );
			float4 screenColor8 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,( (ase_grabScreenPosNorm).xy + ( uv_TexCoord81 * Strength75 * Distortion130 ) ));
			float4 lerpResult133 = lerp( screenColor8 , ( screenColor8 * ( i.vertexColor.a * 2.0 ) * float4( (i.vertexColor).rgb , 0.0 ) ) , Distortion130);
			o.Emission = lerpResult133.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19200
Node;AmplifyShaderEditor.CommentaryNode;107;-949.0161,-60.85888;Inherit;False;972.8016;260.4384;Comment;4;5;6;4;8;Sample screen colour with distortion offset;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;106;-2189.592,985.1756;Inherit;False;1899.104;774.427;Comment;17;130;128;129;123;122;118;121;112;119;72;109;111;110;135;138;139;141;Ridge distortion;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;101;-3918.119,-88.62897;Inherit;False;1541.176;397.2803;Comment;7;42;55;75;48;10;46;47;Get values from particle system (vert col) or material properties;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;52;-1625.221,-229.8576;Inherit;False;100;100;;0;Whenever the shader is edited in ASE, remember to put "ZTest Off ZWrite Off" in the script;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;-3354.534,81.8318;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;46;-3567.118,14.56385;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-3375.261,-38.62893;Inherit;False;Property;_DistortAmount;DistortAmount;0;0;Create;True;0;0;0;False;0;False;0;0.55;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;48;-3583.533,151.8318;Inherit;False;Property;_DistortionMultiplier;DistortionMultiplier;1;0;Create;True;0;0;0;False;0;False;0;0.89;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;75;-2644.466,92.71651;Inherit;False;Strength;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;55;-2840.712,54.66396;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;81;-1610.167,254.8733;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;-0.5,-0.5;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;85;-530.6202,256.0917;Inherit;True;3;3;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;5;-652.0161,-10.42051;Inherit;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;6;-368.0161,-10.42051;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GrabScreenPosition;4;-899.0161,-9.420514;Inherit;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScreenColorNode;8;-186.2145,-10.85887;Inherit;False;Global;_GrabScreen0;Grab Screen 0;1;0;Create;True;0;0;0;False;0;False;Object;-1;False;False;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;76;-1013.616,363.5533;Inherit;False;75;Strength;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;110;-1704.991,1107.886;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;111;-1474.48,1116.932;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;109;-1870.326,1092.459;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;72;-2139.592,1080.883;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;-0.5,-0.5;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;119;-2112.906,1383.85;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;112;-1269.008,1102.101;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;2.32;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;118;-1463.099,1019.946;Inherit;False;Property;_LateralFalloff;LateralFalloff;2;0;Create;True;0;0;0;False;0;False;3;0;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;42;-3870.005,24.5256;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;123;-826.9529,1264.317;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;128;-988.4481,1233.486;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;130;-582.7791,1271.645;Inherit;False;Distortion;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;131;-749.1981,418.1673;Inherit;False;130;Distortion;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;3;900.4535,-63.65259;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Distort/Distort_Ridge;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;False;0;True;TransparentCutout;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;5;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;124;339.5757,27.73421;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;133;555.5031,-193.5132;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;132;163.7395,-186.7007;Inherit;False;130;Distortion;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;134;18.50305,373.4868;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;126;-19.38885,544.8391;Inherit;False;Property;_Tint;Tint;6;0;Create;True;0;0;0;False;0;False;1,1,1,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;125;185.1261,561.174;Inherit;False;Property;_Brightness;Brightness;4;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;136;242.4602,366.728;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;137;205.2432,252.8211;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;135;-1809.816,1418.262;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;121;-1634.71,1427.19;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;2.32;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;129;-1429.447,1424.486;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;122;-2080.71,1683.19;Inherit;False;Property;_LongitudinalFalloff;LongitudinalFalloff;3;0;Create;True;0;0;0;False;0;False;0.4782608;0;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;139;-1184.885,1596.333;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;138;-1377.979,1599.333;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;2.32;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;141;-1648.286,1562.84;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.14;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
WireConnection;47;0;46;0
WireConnection;47;1;48;0
WireConnection;46;0;42;3
WireConnection;46;1;42;1
WireConnection;75;0;55;0
WireConnection;55;0;10;0
WireConnection;55;1;47;0
WireConnection;85;0;81;0
WireConnection;85;1;76;0
WireConnection;85;2;131;0
WireConnection;5;0;4;0
WireConnection;6;0;5;0
WireConnection;6;1;85;0
WireConnection;8;0;6;0
WireConnection;110;0;109;0
WireConnection;111;0;110;0
WireConnection;109;0;72;2
WireConnection;112;0;111;0
WireConnection;112;1;118;0
WireConnection;123;0;128;0
WireConnection;123;1;129;0
WireConnection;123;2;139;0
WireConnection;128;0;112;0
WireConnection;130;0;123;0
WireConnection;3;2;133;0
WireConnection;124;0;8;0
WireConnection;124;1;137;0
WireConnection;124;2;136;0
WireConnection;133;0;8;0
WireConnection;133;1;124;0
WireConnection;133;2;132;0
WireConnection;136;0;134;0
WireConnection;137;0;134;4
WireConnection;135;0;119;1
WireConnection;121;0;135;0
WireConnection;121;1;122;0
WireConnection;129;0;121;0
WireConnection;139;0;138;0
WireConnection;138;0;141;0
WireConnection;138;1;122;0
WireConnection;141;0;119;1
ASEEND*/
//CHKSM=32732AFF76C2DD843B7C41B046D62AD4A389B8D5