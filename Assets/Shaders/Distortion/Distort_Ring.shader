// Made with Amplify Shader Editor v1.9.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Distort/Distort_Ring"
{
	Properties
	{
		_Size("Size", Range( 0 , 1)) = 0.3099631
		_DistortAmount("DistortAmount", Range( -1 , 1)) = 0
		_DistortionMultiplier("DistortionMultiplier", Float) = 0
		_RingWidth("RingWidth", Range( 0 , 0.4)) = 0.05
		_RingWidthMultiplier("RingWidthMultiplier", Float) = 1
		_RingFalloffInner("Ring Falloff Inner", Range( -0.1 , 3)) = 1
		_RingFalloffOuter("Ring Falloff Outer", Range( 0.1 , 3)) = 1
		_IsParticleSystem("IsParticleSystem", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent-10" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		GrabPass{ }
		CGPROGRAM
		#pragma target 3.0
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
		#pragma surface surf Unlit alpha:fade keepalpha noshadow 
		struct Input
		{
			float4 screenPos;
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform float _Size;
		uniform float _IsParticleSystem;
		uniform float _RingWidth;
		uniform float _RingWidthMultiplier;
		uniform float _RingFalloffOuter;
		uniform float _RingFalloffInner;
		uniform float _DistortAmount;
		uniform float _DistortionMultiplier;


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
			float lerpResult99 = lerp( _Size , i.vertexColor.a , _IsParticleSystem);
			float Size100 = lerpResult99;
			float lerpResult97 = lerp( _RingWidth , ( i.vertexColor.g * 0.4 * _RingWidthMultiplier ) , _IsParticleSystem);
			float Width96 = lerpResult97;
			float2 uv_TexCoord72 = i.uv_texcoord + float2( -0.5,-0.5 );
			float smoothstepResult60 = smoothstep( ( Size100 - Width96 ) , ( Width96 + Size100 ) , length( uv_TexCoord72 ));
			float lerpResult55 = lerp( _DistortAmount , ( ( i.vertexColor.b - i.vertexColor.r ) * _DistortionMultiplier ) , _IsParticleSystem);
			float Strength75 = lerpResult55;
			float temp_output_86_0 = saturate( (1.0 + (length( uv_TexCoord81 ) - 0.2) * (0.0 - 1.0) / (0.5 - 0.2)) );
			float4 screenColor8 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,( (ase_grabScreenPosNorm).xy + ( uv_TexCoord81 * ( saturate( pow( ( 1.0 - smoothstepResult60 ) , _RingFalloffOuter ) ) * saturate( pow( smoothstepResult60 , _RingFalloffInner ) ) ) * Strength75 * temp_output_86_0 ) ));
			o.Emission = screenColor8.rgb;
			o.Alpha = temp_output_86_0;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19200
Node;AmplifyShaderEditor.CommentaryNode;107;-949.0161,-60.85888;Inherit;False;972.8016;260.4384;Comment;4;5;6;4;8;Sample screen colour with distortion offset;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;106;-2214.731,925.5258;Inherit;False;1702.148;774.427;Comment;15;63;64;60;57;68;69;90;92;89;91;93;98;102;94;72;Ring distortion;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;105;-1315.403,359.8063;Inherit;False;698.1183;304.3958;Comment;3;80;84;86;Reduce distortion to zero near the sides;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;101;-3918.119,-88.62897;Inherit;False;1529.86;921.5845;Comment;16;75;47;46;10;55;54;42;48;62;97;96;99;100;79;104;108;Get values from particle system (vert col) or material properties;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;52;-1625.221,-229.8576;Inherit;False;100;100;;0;Whenever the shader is edited in ASE, remember to put "ZTest Off ZWrite Off" in the script;1,1,1,1;0;0
Node;AmplifyShaderEditor.WireNode;87;-628.9644,677.709;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;-3354.534,81.8318;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;46;-3567.118,14.56385;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-3375.261,-38.62893;Inherit;False;Property;_DistortAmount;DistortAmount;1;0;Create;True;0;0;0;False;0;False;0;1;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;54;-3274.791,719.9556;Inherit;False;Property;_IsParticleSystem;IsParticleSystem;7;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;48;-3583.533,151.8318;Inherit;False;Property;_DistortionMultiplier;DistortionMultiplier;2;0;Create;True;0;0;0;False;0;False;0;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;62;-3224.646,229.7643;Inherit;False;Property;_RingWidth;RingWidth;3;0;Create;True;0;0;0;False;0;False;0.05;0.178;0;0.4;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;97;-2838.298,230.8637;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;96;-2630.259,234.6523;Inherit;False;Width;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;99;-2843.483,439.0671;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;100;-2635.445,440.8262;Inherit;False;Size;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;79;-3229.272,400.8147;Inherit;False;Property;_Size;Size;0;0;Create;True;0;0;0;False;0;False;0.3099631;0.303;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;42;-3868.119,288.5637;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;76;-1095.772,731.1666;Inherit;False;75;Strength;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LengthOpNode;80;-1265.403,409.8063;Inherit;True;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;84;-1076.236,410.2021;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0.2;False;2;FLOAT;0.5;False;3;FLOAT;1;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;86;-795.285,508.8282;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;63;-1628.24,1432.823;Inherit;False;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;64;-1638.24,1252.823;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LengthOpNode;57;-1890.609,1023.023;Inherit;True;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;92;-993.9721,1319.414;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;93;-1150.479,1441.526;Inherit;False;Property;_RingFalloffInner;Ring Falloff Inner;5;0;Create;True;0;0;0;False;0;False;1;1.41;-0.1;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;98;-2025.163,1586.953;Inherit;False;96;Width;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;102;-2061.255,1440.46;Inherit;False;100;Size;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;94;-1157.479,975.5258;Inherit;False;Property;_RingFalloffOuter;Ring Falloff Outer;6;0;Create;True;0;0;0;False;0;False;1;0.25;0.1;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;72;-2164.731,1021.233;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;-0.5,-0.5;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;3;116.6607,-57.89372;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Distort/Distort_Ring;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;2;False;;0;False;;False;0;False;;0;False;;False;0;Transparent;0.5;True;False;0;True;Transparent;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;8;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.PowerNode;91;-1157.074,1319.971;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;60;-1441.973,1212.365;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;68;-1180.389,1080.367;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;90;-899.625,1080.73;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;89;-1038.215,1080.287;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;0.98;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;69;-742.1875,1236.042;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;75;-2644.466,92.71651;Inherit;False;Strength;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;55;-2840.712,54.66396;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;81;-1610.167,254.8733;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;-0.5,-0.5;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;85;-530.6202,256.0917;Inherit;False;4;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;5;-652.0161,-10.42051;Inherit;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;6;-368.0161,-10.42051;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GrabScreenPosition;4;-899.0161,-9.420514;Inherit;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScreenColorNode;8;-186.2145,-10.85887;Inherit;False;Global;_GrabScreen0;Grab Screen 0;1;0;Create;True;0;0;0;False;0;False;Object;-1;False;False;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;104;-3488.994,279.8207;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0.4;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;108;-3443.236,340.0819;Inherit;False;Property;_RingWidthMultiplier;RingWidthMultiplier;4;0;Create;True;0;0;0;False;0;False;1;4.03;0;0;0;1;FLOAT;0
WireConnection;87;0;76;0
WireConnection;47;0;46;0
WireConnection;47;1;48;0
WireConnection;46;0;42;3
WireConnection;46;1;42;1
WireConnection;97;0;62;0
WireConnection;97;1;104;0
WireConnection;97;2;54;0
WireConnection;96;0;97;0
WireConnection;99;0;79;0
WireConnection;99;1;42;4
WireConnection;99;2;54;0
WireConnection;100;0;99;0
WireConnection;80;0;81;0
WireConnection;84;0;80;0
WireConnection;86;0;84;0
WireConnection;63;0;102;0
WireConnection;63;1;98;0
WireConnection;64;0;98;0
WireConnection;64;1;102;0
WireConnection;57;0;72;0
WireConnection;92;0;91;0
WireConnection;3;2;8;0
WireConnection;3;9;86;0
WireConnection;91;0;60;0
WireConnection;91;1;93;0
WireConnection;60;0;57;0
WireConnection;60;1;63;0
WireConnection;60;2;64;0
WireConnection;68;0;60;0
WireConnection;90;0;89;0
WireConnection;89;0;68;0
WireConnection;89;1;94;0
WireConnection;69;0;90;0
WireConnection;69;1;92;0
WireConnection;75;0;55;0
WireConnection;55;0;10;0
WireConnection;55;1;47;0
WireConnection;55;2;54;0
WireConnection;85;0;81;0
WireConnection;85;1;69;0
WireConnection;85;2;87;0
WireConnection;85;3;86;0
WireConnection;5;0;4;0
WireConnection;6;0;5;0
WireConnection;6;1;85;0
WireConnection;8;0;6;0
WireConnection;104;0;42;2
WireConnection;104;2;108;0
ASEEND*/
//CHKSM=9DD0CAD98E0999A240946D51B03026113ACC6729