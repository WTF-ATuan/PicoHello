Shader "Interactive_HandIntersect"
{
	Properties
	{
		_ASEOutlineWidth( "Outline Width", Float ) = 0
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_DepthFadeDistance("Depth Fade Distance", Float) = 1
		_Color("Color", Color) = (0.6415094,0.239053,0.239053,0)
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_IntersectionColor("Intersection Color", Color) = (0.4338235,0.4377282,1,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0"}
		ZWrite Off
		ZTest Always
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface outlineSurf Outline nofog alpha:fade  keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nometa noforwardadd vertex:outlineVertexDataFunc 
		
		#include "UnityCG.cginc"
		
		
		struct Input
		{
			float4 screenPos;
		};
		uniform float4 _IntersectionColor;
		uniform sampler2D _CameraDepthTexture;
		uniform float _DepthFadeDistance;
		uniform half _ASEOutlineWidth;
		
		void outlineVertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			v.vertex.xyz += ( v.normal * _ASEOutlineWidth );
		}
		inline half4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return half4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o )
		{
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth89 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD( ase_screenPos ))));
			float distanceDepth89 = abs( ( screenDepth89 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( ( 0.1 * _DepthFadeDistance ) ) );
			float screenDepth74 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD( ase_screenPos ))));
			float distanceDepth74 = abs( ( screenDepth74 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _DepthFadeDistance ) );
			o.Emission = ( (_IntersectionColor).rgb + ( 1.0 - saturate( distanceDepth89 ) ) );
			o.Alpha = ( 1.0 - saturate( distanceDepth74 ) );
		}
		ENDCG
		

		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+1" }
		Cull Back
		ZWrite On
		ZTest LEqual
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _TextureSample0;
		uniform float4 _Color;
		uniform float _Metallic;
		uniform float _Smoothness;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 PenetrateDepthFade75 = 0;
			v.vertex.xyz += PenetrateDepthFade75;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Albedo = ( tex2D( _TextureSample0, i.uv_texcoord ) * _Color ).rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}