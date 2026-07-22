// Made with Amplify Shader Editor v1.9.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Grid"
{
	Properties
	{
		_GridSize("Grid Size", Float) = 1
		_LineWidth("Line Width", Float) = 0.5
		_LineColour("Line Colour", Color) = (1,1,1,0)
		_CheckersContrast("Checkers Contrast", Range( 0 , 1)) = 0.5
		_WallColour("Wall Colour", Color) = (0.5188679,0.5188679,0.5188679,0)
		_FloorColour("Floor Colour", Color) = (0.4528302,0.4528302,0.4528302,0)
		_ProjectionTransition("Projection Transition", Range( 0 , 50)) = 1
		_SmallGridSizeFactor("SmallGridSizeFactor", Float) = 10
		_SmallGridLineWidth("SmallGridLineWidth", Float) = 0.1
		_SmallGridIntensity("SmallGridIntensity", Range( 0 , 1)) = 0.6521739
		_SmallGridFadeLength("SmallGridFadeLength", Float) = 10
		_SmallGridFadeOffset("SmallGridFadeOffset", Float) = 10
		_UseSmallGridint("Use Small Grid (int)", Range( 0 , 1)) = 0
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			float customSurfaceDepth96;
		};

		uniform float _GridSize;
		uniform float _ProjectionTransition;
		uniform float _CheckersContrast;
		uniform float4 _WallColour;
		uniform float4 _FloorColour;
		uniform float _LineWidth;
		uniform float4 _LineColour;
		uniform float _SmallGridSizeFactor;
		uniform float _SmallGridLineWidth;
		uniform float _SmallGridFadeLength;
		uniform float _SmallGridFadeOffset;
		uniform float _SmallGridIntensity;
		uniform float _UseSmallGridint;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float3 customSurfaceDepth96 = ase_vertex3Pos;
			o.customSurfaceDepth96 = -UnityObjectToViewPos( customSurfaceDepth96 ).z;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldPos = i.worldPos;
			float Size32 = _GridSize;
			float temp_output_44_0_g4 = Size32;
			float3 temp_cast_0 = (temp_output_44_0_g4).xxx;
			float temp_output_9_0_g4 = ( temp_output_44_0_g4 / 2.0 );
			float3 temp_cast_1 = (temp_output_9_0_g4).xxx;
			float3 temp_cast_2 = (temp_output_44_0_g4).xxx;
			float3 temp_cast_3 = (temp_output_9_0_g4).xxx;
			float3 lerpResult16_g4 = lerp( ( ( ( ( ase_worldPos * -1 ) % temp_cast_0 ) - temp_cast_1 ) * -1 ) , ( ( ase_worldPos % temp_cast_2 ) - temp_cast_3 ) , saturate( ( ase_worldPos * 100 ) ));
			float3 break18_g4 = lerpResult16_g4;
			float temp_output_45_0_g4 = _ProjectionTransition;
			float temp_output_32_0_g4 = ( 0.0 - temp_output_45_0_g4 );
			float temp_output_31_0_g4 = ( temp_output_45_0_g4 + 1.0 );
			float3 ase_worldNormal = i.worldNormal;
			float3 break27_g4 = ase_worldNormal;
			float lerpResult33_g4 = lerp( temp_output_32_0_g4 , temp_output_31_0_g4 , abs( break27_g4.z ));
			float lerpResult25_g4 = lerp( step( ( break18_g4.y * break18_g4.z ) , 0.0 ) , step( ( break18_g4.x * break18_g4.y ) , 0.0 ) , saturate( lerpResult33_g4 ));
			float lerpResult34_g4 = lerp( temp_output_32_0_g4 , temp_output_31_0_g4 , abs( break27_g4.y ));
			float temp_output_36_0_g4 = saturate( lerpResult34_g4 );
			float lerpResult37_g4 = lerp( lerpResult25_g4 , step( ( break18_g4.z * break18_g4.x ) , 0.0 ) , temp_output_36_0_g4);
			float lerpResult38_g4 = lerp( 1.0 , lerpResult37_g4 , _CheckersContrast);
			float4 lerpResult43_g4 = lerp( _WallColour , _FloorColour , temp_output_36_0_g4);
			float3 temp_cast_4 = (Size32).xxx;
			float3 temp_output_2_0_g5 = ( ase_worldPos % temp_cast_4 );
			float3 temp_cast_5 = (( _LineWidth / 100.0 )).xxx;
			float3 temp_output_4_0_g5 = step( ( temp_output_2_0_g5 * temp_output_2_0_g5 ) , temp_cast_5 );
			float3 temp_cast_6 = (( _LineWidth / 100.0 )).xxx;
			float3 break16_g5 = ase_worldNormal;
			float2 lerpResult10_g5 = lerp( (temp_output_4_0_g5).xz , (temp_output_4_0_g5).yz , abs( break16_g5.x ));
			float3 temp_cast_7 = (( _LineWidth / 100.0 )).xxx;
			float2 lerpResult11_g5 = lerp( lerpResult10_g5 , (temp_output_4_0_g5).xy , abs( break16_g5.z ));
			float2 temp_cast_8 = 1;
			float dotResult12_g5 = dot( lerpResult11_g5 , temp_cast_8 );
			float4 temp_output_91_0 = ( ( lerpResult38_g4 * lerpResult43_g4 ) + ( dotResult12_g5 * _LineColour ) );
			float3 temp_cast_9 = (( Size32 / _SmallGridSizeFactor )).xxx;
			float3 temp_output_2_0_g6 = ( ase_worldPos % temp_cast_9 );
			float3 temp_cast_10 = (( _SmallGridLineWidth / 100.0 )).xxx;
			float3 temp_output_4_0_g6 = step( ( temp_output_2_0_g6 * temp_output_2_0_g6 ) , temp_cast_10 );
			float3 temp_cast_11 = (( _SmallGridLineWidth / 100.0 )).xxx;
			float3 break16_g6 = ase_worldNormal;
			float2 lerpResult10_g6 = lerp( (temp_output_4_0_g6).xz , (temp_output_4_0_g6).yz , abs( break16_g6.x ));
			float3 temp_cast_12 = (( _SmallGridLineWidth / 100.0 )).xxx;
			float2 lerpResult11_g6 = lerp( lerpResult10_g6 , (temp_output_4_0_g6).xy , abs( break16_g6.z ));
			float2 temp_cast_13 = 1;
			float dotResult12_g6 = dot( lerpResult11_g6 , temp_cast_13 );
			float cameraDepthFade96 = (( i.customSurfaceDepth96 -_ProjectionParams.y - _SmallGridFadeOffset ) / _SmallGridFadeLength);
			float4 lerpResult116 = lerp( saturate( temp_output_91_0 ) , saturate( ( temp_output_91_0 + ( ( ( dotResult12_g6 * ( 1.0 - saturate( cameraDepthFade96 ) ) ) * _LineColour ) * _SmallGridIntensity ) ) ) , step( 1.0 , _UseSmallGridint ));
			o.Albedo = lerpResult116.rgb;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float1 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float3 worldNormal : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
				o.customPack1.x = customInputData.customSurfaceDepth96;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.customSurfaceDepth96 = IN.customPack1.x;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19200
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;87;-1290.265,1483.748;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;89;-1297.063,1056.577;Inherit;False;GridCheckers;-1;;4;ff9bd92ba635e3a4093d55d708a6b3d3;0;5;44;FLOAT;1;False;45;FLOAT;0;False;46;FLOAT;1;False;47;COLOR;0.5294118,0.5294118,0.5294118,0;False;48;COLOR;0.5294118,0.5294118,0.5294118,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;91;-936.7788,1074.698;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;90;-1541.794,865.7375;Inherit;False;32;Size;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;92;-1609.562,949.0914;Inherit;False;Property;_ProjectionTransition;Projection Transition;6;0;Create;True;0;0;0;False;0;False;1;0;0;50;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;93;-1613.629,1027.393;Inherit;False;Property;_CheckersContrast;Checkers Contrast;3;0;Create;True;0;0;0;False;0;False;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;94;-1773.283,1099.592;Inherit;False;Property;_WallColour;Wall Colour;4;0;Create;True;0;0;0;False;0;False;0.5188679,0.5188679,0.5188679,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;95;-1569.902,1192.131;Inherit;False;Property;_FloorColour;Floor Colour;5;0;Create;True;0;0;0;False;0;False;0.4528302,0.4528302,0.4528302,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PosVertexDataNode;97;-2484.787,2061.896;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;98;-2572.787,2210.896;Inherit;False;Property;_SmallGridFadeLength;SmallGridFadeLength;10;0;Create;True;0;0;0;False;0;False;10;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;99;-2554.787,2293.896;Inherit;False;Property;_SmallGridFadeOffset;SmallGridFadeOffset;11;0;Create;True;0;0;0;False;0;False;10;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CameraDepthFade;96;-2267.699,2182.772;Inherit;False;3;2;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;100;-2001.787,2183.896;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;101;-1846.787,2184.896;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;84;-1645.009,1483.86;Inherit;False;GridLines;-1;;5;214dc4e8b6c60cd4fad403d956e35e77;0;2;20;FLOAT;1;False;21;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;103;-1533.246,2001.242;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;102;-1811.787,1965.896;Inherit;False;GridLines;-1;;6;214dc4e8b6c60cd4fad403d956e35e77;0;2;20;FLOAT;1;False;21;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;85;-1863.216,1457.881;Inherit;False;32;Size;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;107;-1918.246,1845.242;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;106;-2242.246,1799.242;Inherit;False;32;Size;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;105;-2147.246,1864.242;Inherit;False;Property;_SmallGridSizeFactor;SmallGridSizeFactor;7;0;Create;True;0;0;0;False;0;False;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;86;-1833.328,1553.546;Inherit;False;Property;_LineWidth;Line Width;1;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;108;-2054.246,1983.242;Inherit;False;Property;_SmallGridLineWidth;SmallGridLineWidth;8;0;Create;True;0;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;88;-1495.866,1592.348;Inherit;False;Property;_LineColour;Line Colour;2;0;Create;True;0;0;0;False;0;False;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;109;-1329.246,1999.242;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;110;-1054.75,1997.173;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;111;-1302.75,2122.173;Inherit;False;Property;_SmallGridIntensity;SmallGridIntensity;9;0;Create;True;0;0;0;False;0;False;0.6521739;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;112;-742.7878,1133.194;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;115;-745.0134,1308.906;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;113;-503.1524,1145.69;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;114;-492.1049,1317.541;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-2274.057,478.3008;Inherit;False;Property;_GridSize;Grid Size;0;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;32;-2103.619,480.4736;Inherit;False;Size;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;116;-153.8756,1250.919;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;119;-489.8756,1488.919;Inherit;False;Property;_UseSmallGridint;Use Small Grid (int);12;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;121;-183.8756,1475.919;Inherit;True;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;276.9052,1122.416;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Grid;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;87;0;84;0
WireConnection;87;1;88;0
WireConnection;89;44;90;0
WireConnection;89;45;92;0
WireConnection;89;46;93;0
WireConnection;89;47;94;0
WireConnection;89;48;95;0
WireConnection;91;0;89;0
WireConnection;91;1;87;0
WireConnection;96;2;97;0
WireConnection;96;0;98;0
WireConnection;96;1;99;0
WireConnection;100;0;96;0
WireConnection;101;0;100;0
WireConnection;84;20;85;0
WireConnection;84;21;86;0
WireConnection;103;0;102;0
WireConnection;103;1;101;0
WireConnection;102;20;107;0
WireConnection;102;21;108;0
WireConnection;107;0;106;0
WireConnection;107;1;105;0
WireConnection;109;0;103;0
WireConnection;109;1;88;0
WireConnection;110;0;109;0
WireConnection;110;1;111;0
WireConnection;112;0;91;0
WireConnection;112;1;110;0
WireConnection;115;0;91;0
WireConnection;113;0;112;0
WireConnection;114;0;115;0
WireConnection;32;0;3;0
WireConnection;116;0;114;0
WireConnection;116;1;113;0
WireConnection;116;2;121;0
WireConnection;121;1;119;0
WireConnection;0;0;116;0
ASEEND*/
//CHKSM=3E3BF608280B7128027C98D823937A9DA1EAAA23