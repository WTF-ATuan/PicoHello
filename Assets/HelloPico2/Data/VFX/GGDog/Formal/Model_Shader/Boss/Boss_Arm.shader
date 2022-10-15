Shader "GGDog/SSS"
{
    Properties
    {
        _Color("Color",Color) = (0.75,0.75,0.75,1)
        _ShadowColor("ShadowColor",Color) = (0.25,0.25,0.25,1)
        [HDR]_BackRimColor("BackRim Color",Color) = (1,0.5,0.5,1)
        [HDR]_DirRimColor("DirRim Color",Color) = (1,0.5,0.5,1)
        _Gloss("Gloss",Range(1,200)) = 10
    }
    SubShader
    {
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                half4 vertex : POSITION;
                half2 uv : TEXCOORD0;
				half3 normal : NORMAL;
            };

            struct v2f
            {
                half2 uv : TEXCOORD0;
                half4 vertex : SV_POSITION;
                half3 worldNormal : TEXCOORD1;
                half3 worldPos : TEXCOORD2;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				
				o.worldNormal = mul(v.normal,(half3x3)unity_WorldToObject);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                return o;
            }
			half4 _Color;
			half4 _ShadowColor;
			half4 _BackRimColor;
			half4 _DirRimColor;
			
            half _Gloss;
            
            half3 _LightDir;
            half4 frag (v2f i) : SV_Target
            {
                float n =  smoothstep(0.5,1,distance(frac(20*i.uv+_Time.y*half2(0.7,1)*0.25),0.5));
                float n2 =  smoothstep(0.3,1,distance(frac(10*i.uv+_Time.y*half2(0.9,0.75)*0.75),0.5));
                n+=n2;
                n = saturate(n+0.25);
                half3 WorldNormal = normalize(i.worldNormal);
                half3 LightDir = _LightDir;
                half3 ViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos );

				half NdotL = saturate(dot(WorldNormal,LightDir));
				half Rim = 1-saturate(smoothstep(0,1,dot(WorldNormal,ViewDir)));

                half3 reflectDir = reflect(-LightDir,WorldNormal);
                half Specular =  pow(max(0,dot(ViewDir,reflectDir)),_Gloss);

                half4 BackRimColor =  ( smoothstep(0,1,Rim*smoothstep(0.99,1.02,saturate(1-NdotL-0.0015)))  * _BackRimColor )*n;
                half4 DirRimColor =  (smoothstep(-0.5,1,Rim*smoothstep(0,0.25,saturate(NdotL+0.0015)))  * _DirRimColor )*n;

                _ShadowColor = lerp(_ShadowColor,_ShadowColor/2,smoothstep(-0.5,1,Rim));
                _Color = lerp(_Color,_Color/2,smoothstep(-0.5,1,Rim));

				half4 FinalColor = lerp(_ShadowColor + BackRimColor*smoothstep(0,0.25,i.uv.y),_Color + DirRimColor*smoothstep(0,0.25,i.uv.y)  ,NdotL ) +Specular*saturate(BackRimColor*3.5);

                FinalColor.a = smoothstep(0,0.15,i.uv.y);

                return FinalColor;
            }
            ENDCG
        }
    }
    SubShader
    {
		LOD 0 
        Pass
        {
			Tags { "RenderType"="Opaque" }
 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }
            float4 frag (v2f i) : SV_Target
            {
                return 0.25 ;
            }
            ENDCG
        }
    }
}
