// Made with Amplify Shader Editor v1.9.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Herringen/ObjectSpaceTriplanarFortress"
{
	Properties
	{
		_TextureScale("TextureScale", Float) = 1
		_ProjectionTransition1("Projection Transition", Range( 0 , 50)) = 0
		_Brightness("Brightness", Range( 0 , 2)) = 0
		_Add("Add", Float) = 0
		_ColourY("ColourY", 2D) = "white" {}
		_ColourXZ("ColourXZ", 2D) = "white" {}
		_FogColor("FogColor", Color) = (0.4290846,0.461,0.4534151,0)
		_FogDistance("FogDistance", Range( 0 , 0.01)) = 0.00065
		_FogFalloff("FogFalloff", Range( 0 , 5)) = 0.82
		_CloseSaturation("CloseSaturation", Float) = 0
		_CloseColourY("CloseColourY", 2D) = "white" {}
		_CloseColourXZ("CloseColourXZ", 2D) = "white" {}
		_CloseColourIntensity("CloseColourIntensity", Range( 0 , 1)) = 0.15
		_CloseColourLength("CloseColourLength", Float) = 250
		_CloseColourOffset("CloseColourOffset", Float) = 50
		_NormalY("NormalY", 2D) = "linearGrey" {}
		_NormalXZ("NormalXZ", 2D) = "linearGrey" {}
		_NormalIntensity("Normal Intensity", Range( 0 , 1)) = 0.2
		_NormalFadeLength("Normal Fade Length", Float) = 200
		_NormalFadeOffset("Normal Fade Offset", Float) = 750
		_SmoothnessY("SmoothnessY", 2D) = "black" {}
		_SmoothnessXZ("SmoothnessXZ", 2D) = "black" {}
		_SmoothnessIntensity("SmoothnessIntensity", Range( 0 , 3)) = 1
		_SmoothnessFadeLength("SmoothnessFadeLength", Float) = 1125
		_SmoothnessFadeOffset("SmoothnessFadeOffset", Float) = 0
		_WireframeTex("WireframeTex", 2D) = "white" {}
		_WireframeDistanceThreshold("WireframeDistanceThreshold", Float) = 125
		_WireScale("WireScale", Float) = 0.03
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Off
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float customSurfaceDepth54;
			float customSurfaceDepth80;
			float customSurfaceDepth185;
		};

		uniform sampler2D _NormalXZ;
		uniform float _TextureScale;
		uniform float _ProjectionTransition1;
		uniform sampler2D _NormalY;
		uniform float _NormalIntensity;
		uniform float _NormalFadeLength;
		uniform float _NormalFadeOffset;
		uniform sampler2D _ColourXZ;
		uniform sampler2D _ColourY;
		uniform sampler2D _CloseColourXZ;
		uniform sampler2D _CloseColourY;
		uniform float _CloseSaturation;
		uniform float _CloseColourLength;
		uniform float _CloseColourOffset;
		uniform float _CloseColourIntensity;
		uniform float _Brightness;
		uniform float _Add;
		uniform float4 _FogColor;
		uniform float _FogDistance;
		uniform float _FogFalloff;
		uniform sampler2D _WireframeTex;
		uniform float _WireScale;
		uniform float3 ShipPos;
		uniform float _WireframeDistanceThreshold;
		uniform float DoWireframe;
		uniform float4 WireframeColour;
		uniform float _SmoothnessFadeLength;
		uniform float _SmoothnessFadeOffset;
		uniform sampler2D _SmoothnessXZ;
		uniform sampler2D _SmoothnessY;
		uniform float _SmoothnessIntensity;


		float3 HSVToRGB( float3 c )
		{
			float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
			float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
			return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
		}


		float3 RGBToHSV(float3 c)
		{
			float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
			float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
			float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
			float d = q.x - min( q.w, q.y );
			float e = 1.0e-10;
			return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
		}

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float3 customSurfaceDepth54 = ase_vertex3Pos;
			o.customSurfaceDepth54 = -UnityObjectToViewPos( customSurfaceDepth54 ).z;
			float3 customSurfaceDepth80 = ase_vertex3Pos;
			o.customSurfaceDepth80 = -UnityObjectToViewPos( customSurfaceDepth80 ).z;
			float3 customSurfaceDepth185 = ase_vertex3Pos;
			o.customSurfaceDepth185 = -UnityObjectToViewPos( customSurfaceDepth185 ).z;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 _NoNormal = float3(0,0,0.5);
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float3 appendResult334 = (float3(_TextureScale , _TextureScale , _TextureScale));
			float3 ase_objectScale = float3( length( unity_ObjectToWorld[ 0 ].xyz ), length( unity_ObjectToWorld[ 1 ].xyz ), length( unity_ObjectToWorld[ 2 ].xyz ) );
			float3 temp_output_228_0 = ( ase_vertex3Pos * appendResult334 * ase_objectScale * 0.001 );
			float2 TriplanarYZ236 = (temp_output_228_0).yz;
			float2 TriplanarXY235 = (temp_output_228_0).xy;
			float Projection_Transition66 = _ProjectionTransition1;
			float temp_output_247_0 = ( 0.0 - Projection_Transition66 );
			float temp_output_243_0 = ( Projection_Transition66 + 1.0 );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_vertexNormal = mul( unity_WorldToObject, float4( ase_worldNormal, 0 ) );
			ase_vertexNormal = normalize( ase_vertexNormal );
			float3 break240 = ase_vertexNormal;
			float lerpResult244 = lerp( temp_output_247_0 , temp_output_243_0 , abs( break240.z ));
			float TriplanarHorizontalTransition250 = saturate( lerpResult244 );
			float3 lerpResult201 = lerp( UnpackNormal( tex2D( _NormalXZ, TriplanarYZ236 ) ) , UnpackNormal( tex2D( _NormalXZ, TriplanarXY235 ) ) , TriplanarHorizontalTransition250);
			float2 TriplanarXZ234 = (temp_output_228_0).xz;
			float lerpResult245 = lerp( temp_output_247_0 , temp_output_243_0 , abs( break240.y ));
			float TriplanarVerticalTransition249 = saturate( lerpResult245 );
			float3 lerpResult203 = lerp( lerpResult201 , UnpackNormal( tex2D( _NormalY, TriplanarXZ234 ) ) , TriplanarVerticalTransition249);
			float3 lerpResult49 = lerp( _NoNormal , lerpResult203 , _NormalIntensity);
			float cameraDepthFade54 = (( i.customSurfaceDepth54 -_ProjectionParams.y - _NormalFadeOffset ) / _NormalFadeLength);
			float3 lerpResult48 = lerp( lerpResult49 , _NoNormal , saturate( cameraDepthFade54 ));
			float3 Normal58 = lerpResult48;
			o.Normal = Normal58;
			float4 lerpResult256 = lerp( tex2D( _ColourXZ, TriplanarYZ236 ) , tex2D( _ColourXZ, TriplanarXY235 ) , TriplanarHorizontalTransition250);
			float4 lerpResult257 = lerp( lerpResult256 , tex2D( _ColourY, TriplanarXZ234 ) , TriplanarVerticalTransition249);
			float4 lerpResult269 = lerp( tex2D( _CloseColourXZ, TriplanarYZ236 ) , tex2D( _CloseColourXZ, TriplanarXY235 ) , TriplanarHorizontalTransition250);
			float4 lerpResult270 = lerp( lerpResult269 , tex2D( _CloseColourY, TriplanarXZ234 ) , TriplanarVerticalTransition249);
			float3 hsvTorgb304 = RGBToHSV( lerpResult270.rgb );
			float3 hsvTorgb306 = HSVToRGB( float3(hsvTorgb304.x,( hsvTorgb304.y * _CloseSaturation ),hsvTorgb304.z) );
			float cameraDepthFade80 = (( i.customSurfaceDepth80 -_ProjectionParams.y - _CloseColourOffset ) / _CloseColourLength);
			float4 lerpResult78 = lerp( lerpResult257 , float4( hsvTorgb306 , 0.0 ) , ( ( 1.0 - saturate( cameraDepthFade80 ) ) * _CloseColourIntensity ));
			float3 ase_worldPos = i.worldPos;
			float4 lerpResult1_g55 = lerp( ( ( lerpResult78 * _Brightness ) + _Add ) , _FogColor , saturate( pow( ( _FogDistance * distance( ase_worldPos , _WorldSpaceCameraPos ) ) , _FogFalloff ) ));
			float4 Colour94 = lerpResult1_g55;
			float3 appendResult165 = (float3(_WireScale , _WireScale , _WireScale));
			float3 temp_output_6_0_g53 = ( ase_worldPos * appendResult165 );
			float temp_output_33_0_g53 = Projection_Transition66;
			float temp_output_23_0_g53 = ( 0.0 - temp_output_33_0_g53 );
			float temp_output_19_0_g53 = ( temp_output_33_0_g53 + 1.0 );
			float3 break16_g53 = ase_worldNormal;
			float lerpResult20_g53 = lerp( temp_output_23_0_g53 , temp_output_19_0_g53 , abs( break16_g53.z ));
			float4 lerpResult12_g53 = lerp( tex2D( _WireframeTex, (temp_output_6_0_g53).yz ) , tex2D( _WireframeTex, (temp_output_6_0_g53).xy ) , saturate( lerpResult20_g53 ));
			float lerpResult21_g53 = lerp( temp_output_23_0_g53 , temp_output_19_0_g53 , abs( break16_g53.y ));
			float4 lerpResult30_g53 = lerp( lerpResult12_g53 , tex2D( _WireframeTex, (temp_output_6_0_g53).xz ) , saturate( lerpResult21_g53 ));
			float4 temp_output_179_35 = lerpResult30_g53;
			float temp_output_171_0 = ( step( distance( ase_worldPos , ShipPos ) , _WireframeDistanceThreshold ) * DoWireframe );
			clip( temp_output_179_35.a - ( 0.25 * temp_output_171_0 ));
			float4 lerpResult172 = lerp( Colour94 , ( temp_output_179_35 * WireframeColour ) , temp_output_171_0);
			float4 ColourWithWireframe166 = lerpResult172;
			o.Emission = ColourWithWireframe166.rgb;
			float cameraDepthFade185 = (( i.customSurfaceDepth185 -_ProjectionParams.y - _SmoothnessFadeOffset ) / _SmoothnessFadeLength);
			float4 lerpResult282 = lerp( tex2D( _SmoothnessXZ, TriplanarYZ236 ) , tex2D( _SmoothnessXZ, TriplanarXY235 ) , TriplanarHorizontalTransition250);
			float4 lerpResult283 = lerp( ( 0.5 * lerpResult282 ) , tex2D( _SmoothnessY, TriplanarXZ234 ) , TriplanarVerticalTransition249);
			float4 SmoothMetallic187 = saturate( ( ( 1.0 - pow( saturate( cameraDepthFade185 ) , 0.8 ) ) * lerpResult283 * _SmoothnessIntensity ) );
			float4 temp_output_193_0 = SmoothMetallic187;
			o.Metallic = temp_output_193_0.r;
			o.Smoothness = temp_output_193_0.r;
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
				float3 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
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
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.x = customInputData.customSurfaceDepth54;
				o.customPack1.y = customInputData.customSurfaceDepth80;
				o.customPack1.z = customInputData.customSurfaceDepth185;
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
				surfIN.customSurfaceDepth54 = IN.customPack1.x;
				surfIN.customSurfaceDepth80 = IN.customPack1.y;
				surfIN.customSurfaceDepth185 = IN.customPack1.z;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
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
Node;AmplifyShaderEditor.CommentaryNode;67;-4197.581,-3447.924;Inherit;False;1492.487;1232.871;Comment;4;62;66;294;295;Common Vars;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;295;-4149.32,-2817.322;Inherit;False;1391.124;512.1541;Transition factors between triplanar directionss;13;248;240;246;250;249;247;245;244;243;242;241;238;331;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;294;-4143.917,-3193.904;Inherit;False;1371.338;358.0371;World-based texture coordinates;12;236;228;232;233;234;235;231;332;334;333;362;365;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;175;-2592.159,-822.2514;Inherit;False;2217.16;1713.127;Comment;21;115;166;118;117;165;162;107;108;109;170;110;171;111;119;174;95;172;164;179;114;330;Wireframe effect;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;99;-5657.512,-835.614;Inherit;False;2982.384;1633.218;Comment;43;81;82;87;98;79;83;80;97;267;268;92;93;269;270;280;279;278;17;261;260;259;257;256;274;273;272;271;94;77;78;258;14;300;301;303;302;304;306;307;308;309;310;311;Colour;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;57;-5403.931,-1996.125;Inherit;False;2723.268;1070.94;Comment;24;255;254;251;60;64;252;253;217;216;202;200;203;201;296;48;55;56;47;49;50;52;54;53;58;Normal;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;58;-2901.519,-1460.137;Inherit;False;Normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PosVertexDataNode;53;-3829.662,-1948.983;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CameraDepthFade;54;-3612.573,-1828.107;Inherit;False;3;2;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;52;-3602.879,-1070.894;Inherit;False;Property;_NormalIntensity;Normal Intensity;17;0;Create;True;0;0;0;False;0;False;0.2;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;50;-3784.842,-1447.241;Inherit;False;Constant;_NoNormal;NoNormal;20;0;Create;True;0;0;0;False;0;False;0,0,0.5;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.LerpOp;49;-3316.902,-1234.644;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;47;-3302.625,-1833.373;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;56;-3861.208,-1801.296;Inherit;False;Property;_NormalFadeLength;Normal Fade Length;18;0;Create;True;0;0;0;False;0;False;200;200;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;55;-3794.632,-1711.731;Inherit;False;Property;_NormalFadeOffset;Normal Fade Offset;19;0;Create;True;0;0;0;False;0;False;750;500;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;48;-3110.841,-1460.802;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;115;-1243.426,-491.4547;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;166;-648.9985,-487.3244;Inherit;False;ColourWithWireframe;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.BreakToComponentsNode;118;-1759.159,-409.9357;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.ClipNode;117;-1537.259,-491.5357;Inherit;False;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0.9;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;165;-2256.517,-368.4506;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldPosInputsNode;107;-2518.593,494.8755;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DistanceOpNode;109;-2255.193,502.4768;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;170;-1984.424,322.3111;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;110;-2505.338,341.3108;Inherit;False;Property;_WireframeDistanceThreshold;WireframeDistanceThreshold;26;0;Create;True;0;0;0;False;0;False;125;125;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;171;-1785.525,321.0113;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;174;-1787.078,-20.43218;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;172;-908.3502,-519.4103;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;164;-2467.117,-311.2505;Inherit;False;Property;_WireScale;WireScale;27;0;Create;True;0;0;0;False;0;False;0.03;0.03;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;119;-2163.269,-20.11629;Inherit;False;Constant;_ClipThreshold;ClipThreshold;17;0;Create;True;0;0;0;False;0;False;0.25;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;108;-2518.593,705.8759;Inherit;False;Global;ShipPos;ShipPos;17;0;Create;True;0;0;0;False;0;False;0,0,0;2967.67,-574.521,-247.1609;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;111;-1989.753,530.5687;Inherit;False;Global;DoWireframe;DoWireframe;17;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;95;-1141.813,-714.2554;Inherit;False;94;Colour;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;162;-2347.383,-211.5087;Inherit;False;66;Projection Transition;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;180;-4602.672,893.7109;Inherit;False;1918.204;1328.539;Reduce smoothness with distance;26;188;190;293;292;291;289;288;287;286;285;284;283;282;195;194;181;185;182;186;187;297;298;299;312;313;366;Smoothness;1,1,1,1;0;0
Node;AmplifyShaderEditor.CameraDepthFade;185;-4001.172,1097.478;Inherit;False;3;2;FLOAT3;0,0,0;False;0;FLOAT;1125;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;181;-4229.056,970.8956;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;194;-4289.031,1121.922;Inherit;False;Property;_SmoothnessFadeLength;SmoothnessFadeLength;23;0;Create;True;0;0;0;False;0;False;1125;500;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;195;-4287.82,1200.026;Inherit;False;Property;_SmoothnessFadeOffset;SmoothnessFadeOffset;24;0;Create;True;0;0;0;False;0;False;0;350;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;820.1332,-470.0429;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Herringen/ObjectSpaceTriplanarFortress;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.FunctionNode;179;-2071.338,-506.0728;Inherit;False;TriplanarMapping;-1;;53;1ad21b610b6395e48b133853da89feee;0;4;27;SAMPLER2D;0;False;28;SAMPLER2D;0;False;29;FLOAT3;1,1,1;False;33;FLOAT;0;False;1;COLOR;35
Node;AmplifyShaderEditor.TexturePropertyNode;114;-2464.364,-572.5226;Inherit;True;Property;_WireframeTex;WireframeTex;25;0;Create;True;0;0;0;False;0;False;6ca9706251e81834592caaa98e3ef520;6ca9706251e81834592caaa98e3ef520;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RegisterLocalVarNode;66;-3363.668,-3339.162;Inherit;False;Projection Transition;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;238;-3287.088,-2539.268;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;241;-3762.031,-2436.113;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;242;-3764.031,-2508.113;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;243;-3736.088,-2676.267;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;244;-3530.088,-2716.267;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;245;-3515.088,-2514.268;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;247;-3765.088,-2775.267;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;249;-3088.547,-2539.463;Inherit;False;TriplanarVerticalTransition;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;250;-3086.706,-2705.098;Inherit;False;TriplanarHorizontalTransition;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;246;-3287.891,-2703.611;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;248;-4086.081,-2657.567;Inherit;False;66;Projection Transition;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;240;-3908.846,-2493.789;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode;62;-4147.581,-3341.347;Inherit;False;Property;_ProjectionTransition1;Projection Transition;1;0;Create;True;0;0;0;False;0;False;0;3.2;0;50;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;296;-3886.907,-1208.404;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;271;-5222.284,44.92684;Inherit;True;Property;_Y2;Y;3;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;272;-4817.606,417.3961;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;273;-5227.712,486.0562;Inherit;True;Property;_Z2;Z;3;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;274;-5227.083,267.5729;Inherit;True;Property;_X2;X;3;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;256;-4672.694,-509.9924;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;257;-4375.932,-359.5386;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;258;-5179.518,-733.5556;Inherit;True;Property;_Y1;Y;3;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;259;-4774.84,-361.0856;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;260;-5184.946,-292.4256;Inherit;True;Property;_Z1;Z;3;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;261;-5184.317,-510.9094;Inherit;True;Property;_X1;X;3;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;14;-5582.583,-733.9014;Inherit;True;Property;_ColourY;ColourY;4;0;Create;True;0;0;0;False;0;False;None;c1b881ceb1a30314db3b19cc00013cf4;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexturePropertyNode;17;-5579.597,-546.06;Inherit;True;Property;_ColourXZ;ColourXZ;5;0;Create;True;0;0;0;False;0;False;None;c1b881ceb1a30314db3b19cc00013cf4;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.GetLocalVarNode;278;-5576.902,-81.62056;Inherit;False;236;TriplanarYZ;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;279;-5571.517,-168.7218;Inherit;False;234;TriplanarXZ;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;280;-5573.579,-3.621552;Inherit;False;235;TriplanarXY;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;270;-4409.204,234.7264;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;269;-4721.158,232.4063;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TexturePropertyNode;93;-5562.558,465.8526;Inherit;True;Property;_CloseColourXZ;CloseColourXZ;11;0;Create;True;0;0;0;False;0;False;None;1c662ed6c6b744343a9a9ed14d11cfec;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexturePropertyNode;92;-5565.542,278.0115;Inherit;True;Property;_CloseColourY;CloseColourY;10;0;Create;True;0;0;0;False;0;False;None;1c662ed6c6b744343a9a9ed14d11cfec;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.GetLocalVarNode;268;-4584.675,-53.7523;Inherit;False;249;TriplanarVerticalTransition;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;267;-4931.084,-55.15688;Inherit;False;250;TriplanarHorizontalTransition;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;97;-3567.963,352.7894;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CameraDepthFade;80;-4090.468,603.0941;Inherit;False;3;2;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;83;-4307.56,482.218;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;79;-3833.505,602.0139;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;98;-3833.425,510.3773;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;87;-4001.976,376.1294;Inherit;False;Property;_CloseColourIntensity;CloseColourIntensity;12;0;Create;True;0;0;0;False;0;False;0.15;0.35;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;82;-4339.413,633.275;Inherit;False;Property;_CloseColourLength;CloseColourLength;13;0;Create;True;0;0;0;False;0;False;250;200;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;81;-4337.986,709.0458;Inherit;False;Property;_CloseColourOffset;CloseColourOffset;14;0;Create;True;0;0;0;False;0;False;50;45;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;284;-4173.084,1448.229;Inherit;True;Property;_Y3;Y;3;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;285;-3768.406,1820.699;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;286;-4178.513,1889.359;Inherit;True;Property;_Z3;Z;3;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;287;-4177.884,1670.875;Inherit;True;Property;_X3;X;3;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;291;-4481.016,1532.31;Inherit;False;236;TriplanarYZ;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;292;-4475.631,1445.209;Inherit;False;234;TriplanarXZ;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;293;-4477.693,1610.309;Inherit;False;235;TriplanarXY;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;193;524.2272,-361.6084;Inherit;False;187;SmoothMetallic;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;167;493.9997,-446.8476;Inherit;False;166;ColourWithWireframe;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;59;556.136,-531.2435;Inherit;False;58;Normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;182;-3417.809,1097.396;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;297;-3588.525,1096.061;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;0.8;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;298;-3752.64,1099.677;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;289;-3833.421,1949.476;Inherit;False;249;TriplanarVerticalTransition;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;288;-3798.446,2101.628;Inherit;False;250;TriplanarHorizontalTransition;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;190;-4515.243,1759.356;Inherit;True;Property;_SmoothnessY;SmoothnessY;20;0;Create;True;0;0;0;False;0;False;None;ea08701ce4175904db212c6fe64ef4ae;False;black;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexturePropertyNode;188;-4491.257,1965.198;Inherit;True;Property;_SmoothnessXZ;SmoothnessXZ;21;0;Create;True;0;0;0;False;0;False;None;ea08701ce4175904db212c6fe64ef4ae;False;black;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;186;-3229.06,1099.021;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;78;-3534.965,-62.87389;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;300;-3187.516,43.00823;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;303;-3113.508,297.9619;Inherit;False;Property;_Add;Add;3;0;Create;True;0;0;0;False;0;False;0;-0.01;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;301;-3469.72,140.5221;Inherit;False;Property;_Brightness;Brightness;2;0;Create;True;0;0;0;False;0;False;0;0.5;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;299;-3444.54,1287.01;Inherit;False;Property;_SmoothnessIntensity;SmoothnessIntensity;22;0;Create;True;0;0;0;False;0;False;1;1;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.RGBToHSVNode;304;-4217.191,88.28308;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.HSVToRGBNode;306;-3808.191,108.2831;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;307;-3969.191,141.2831;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;308;-4159.191,257.2831;Inherit;False;Property;_CloseSaturation;CloseSaturation;9;0;Create;True;0;0;0;False;0;False;0;0.05;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;302;-2928.604,-26.75966;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;94;-2856.697,-629.0954;Inherit;False;Colour;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;309;-3149.197,-440.2478;Inherit;False;SettableFog;-1;;55;fb24b60e11e675b4284e9d00fbe009cf;0;4;15;FLOAT;0.00065;False;16;FLOAT;0.82;False;13;COLOR;0,0,0,0;False;12;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;282;-3676.26,1674.792;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;313;-3561.218,1530.956;Inherit;False;Constant;_Float0;Float 0;29;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;283;-3279.498,1798.247;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;312;-3478.218,1651.956;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;311;-3420.045,-555.1098;Inherit;False;Property;_FogFalloff;FogFalloff;8;0;Create;True;0;0;0;False;0;False;0.82;0.82;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;310;-3427.176,-650.155;Inherit;False;Property;_FogDistance;FogDistance;7;0;Create;True;0;0;0;False;0;False;0.00065;0.00065;0;0.01;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;201;-4467.429,-1521.988;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;203;-4170.668,-1371.534;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;202;-4569.576,-1373.081;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;253;-5282.186,-1661.47;Inherit;False;236;TriplanarYZ;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;251;-5278.862,-1583.471;Inherit;False;235;TriplanarXY;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;254;-4742.615,-1067.152;Inherit;False;250;TriplanarHorizontalTransition;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;255;-4434.589,-1170.305;Inherit;False;249;TriplanarVerticalTransition;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;60;-5323.05,-1223.677;Inherit;True;Property;_NormalXZ;NormalXZ;16;0;Create;True;0;0;0;False;0;False;None;7689d46de8cd9394aa3b33f6e3883214;True;linearGrey;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SamplerNode;217;-4979.052,-1522.905;Inherit;True;Property;_X;X;3;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;216;-4979.682,-1304.421;Inherit;True;Property;_Z;Z;3;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;64;-5325.796,-1410.512;Inherit;True;Property;_NormalY;NormalY;15;0;Create;True;0;0;0;False;0;False;None;7689d46de8cd9394aa3b33f6e3883214;True;linearGrey;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.ColorNode;77;-3451.509,-344.6704;Inherit;False;Property;_FogColor;FogColor;6;0;Create;True;0;0;0;False;0;False;0.4290846,0.461,0.4534151,0;0.4768627,0.5019608,0.5019608,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;200;-4974.253,-1745.551;Inherit;True;Property;_Y;Y;3;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;252;-5276.8,-1748.571;Inherit;False;234;TriplanarXZ;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ColorNode;330;-1474.3,-286.5394;Inherit;False;Global;WireframeColour;WireframeColour;23;0;Create;True;0;0;0;False;0;False;0.8431373,0.8,0.5372549,1;0.7019608,0.5254902,0.1254902,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;231;-3243.96,-2940.641;Inherit;False;True;True;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;235;-3022.431,-2941.268;Inherit;False;TriplanarXY;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;234;-3023.339,-3131.602;Inherit;False;TriplanarXZ;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;233;-3244.775,-3041.605;Inherit;False;False;True;True;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;232;-3248.38,-3131.608;Inherit;False;True;False;True;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;236;-3023.691,-3040.192;Inherit;False;TriplanarYZ;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NormalVertexDataNode;331;-4119.557,-2493.084;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;228;-3514.902,-3132.605;Inherit;False;4;4;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PosVertexDataNode;332;-4123.834,-3149.416;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ObjectScaleNode;362;-4123.478,-3006.674;Inherit;False;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;333;-3909.352,-2942.962;Inherit;False;Property;_TextureScale;TextureScale;0;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;334;-3687.618,-2992.957;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;365;-3526.351,-2908.748;Inherit;False;Constant;_Float1;Float 1;28;0;Create;True;0;0;0;False;0;False;0.001;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;187;-2958.179,1098.762;Inherit;False;SmoothMetallic;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;366;-3059.986,1220.781;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
WireConnection;58;0;48;0
WireConnection;54;2;53;0
WireConnection;54;0;56;0
WireConnection;54;1;55;0
WireConnection;49;0;50;0
WireConnection;49;1;296;0
WireConnection;49;2;52;0
WireConnection;47;0;54;0
WireConnection;48;0;49;0
WireConnection;48;1;50;0
WireConnection;48;2;47;0
WireConnection;115;0;117;0
WireConnection;115;1;330;0
WireConnection;166;0;172;0
WireConnection;118;0;179;35
WireConnection;117;0;179;35
WireConnection;117;1;118;3
WireConnection;117;2;174;0
WireConnection;165;0;164;0
WireConnection;165;1;164;0
WireConnection;165;2;164;0
WireConnection;109;0;107;0
WireConnection;109;1;108;0
WireConnection;170;0;109;0
WireConnection;170;1;110;0
WireConnection;171;0;170;0
WireConnection;171;1;111;0
WireConnection;174;0;119;0
WireConnection;174;1;171;0
WireConnection;172;0;95;0
WireConnection;172;1;115;0
WireConnection;172;2;171;0
WireConnection;185;2;181;0
WireConnection;185;0;194;0
WireConnection;185;1;195;0
WireConnection;0;1;59;0
WireConnection;0;2;167;0
WireConnection;0;3;193;0
WireConnection;0;4;193;0
WireConnection;179;27;114;0
WireConnection;179;28;114;0
WireConnection;179;29;165;0
WireConnection;179;33;162;0
WireConnection;66;0;62;0
WireConnection;238;0;245;0
WireConnection;241;0;240;1
WireConnection;242;0;240;2
WireConnection;243;0;248;0
WireConnection;244;0;247;0
WireConnection;244;1;243;0
WireConnection;244;2;242;0
WireConnection;245;0;247;0
WireConnection;245;1;243;0
WireConnection;245;2;241;0
WireConnection;247;1;248;0
WireConnection;249;0;238;0
WireConnection;250;0;246;0
WireConnection;246;0;244;0
WireConnection;240;0;331;0
WireConnection;296;0;203;0
WireConnection;271;0;92;0
WireConnection;271;1;279;0
WireConnection;272;0;271;0
WireConnection;273;0;93;0
WireConnection;273;1;280;0
WireConnection;274;0;93;0
WireConnection;274;1;278;0
WireConnection;256;0;261;0
WireConnection;256;1;260;0
WireConnection;256;2;267;0
WireConnection;257;0;256;0
WireConnection;257;1;259;0
WireConnection;257;2;268;0
WireConnection;258;0;14;0
WireConnection;258;1;279;0
WireConnection;259;0;258;0
WireConnection;260;0;17;0
WireConnection;260;1;280;0
WireConnection;261;0;17;0
WireConnection;261;1;278;0
WireConnection;270;0;269;0
WireConnection;270;1;272;0
WireConnection;270;2;268;0
WireConnection;269;0;274;0
WireConnection;269;1;273;0
WireConnection;269;2;267;0
WireConnection;97;0;98;0
WireConnection;97;1;87;0
WireConnection;80;2;83;0
WireConnection;80;0;82;0
WireConnection;80;1;81;0
WireConnection;79;0;80;0
WireConnection;98;0;79;0
WireConnection;284;0;190;0
WireConnection;284;1;292;0
WireConnection;285;0;284;0
WireConnection;286;0;188;0
WireConnection;286;1;293;0
WireConnection;287;0;188;0
WireConnection;287;1;291;0
WireConnection;182;0;297;0
WireConnection;297;0;298;0
WireConnection;298;0;185;0
WireConnection;186;0;182;0
WireConnection;186;1;283;0
WireConnection;186;2;299;0
WireConnection;78;0;257;0
WireConnection;78;1;306;0
WireConnection;78;2;97;0
WireConnection;300;0;78;0
WireConnection;300;1;301;0
WireConnection;304;0;270;0
WireConnection;306;0;304;1
WireConnection;306;1;307;0
WireConnection;306;2;304;3
WireConnection;307;0;304;2
WireConnection;307;1;308;0
WireConnection;302;0;300;0
WireConnection;302;1;303;0
WireConnection;94;0;309;0
WireConnection;309;15;310;0
WireConnection;309;16;311;0
WireConnection;309;13;77;0
WireConnection;309;12;302;0
WireConnection;282;0;287;0
WireConnection;282;1;286;0
WireConnection;282;2;288;0
WireConnection;283;0;312;0
WireConnection;283;1;285;0
WireConnection;283;2;289;0
WireConnection;312;0;313;0
WireConnection;312;1;282;0
WireConnection;201;0;217;0
WireConnection;201;1;216;0
WireConnection;201;2;254;0
WireConnection;203;0;201;0
WireConnection;203;1;202;0
WireConnection;203;2;255;0
WireConnection;202;0;200;0
WireConnection;217;0;60;0
WireConnection;217;1;253;0
WireConnection;216;0;60;0
WireConnection;216;1;251;0
WireConnection;200;0;64;0
WireConnection;200;1;252;0
WireConnection;231;0;228;0
WireConnection;235;0;231;0
WireConnection;234;0;232;0
WireConnection;233;0;228;0
WireConnection;232;0;228;0
WireConnection;236;0;233;0
WireConnection;228;0;332;0
WireConnection;228;1;334;0
WireConnection;228;2;362;0
WireConnection;228;3;365;0
WireConnection;334;0;333;0
WireConnection;334;1;333;0
WireConnection;334;2;333;0
WireConnection;187;0;366;0
WireConnection;366;0;186;0
ASEEND*/
//CHKSM=C01733E3ED95338B420D93AADEC0BF8DAE7494B0