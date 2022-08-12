Shader "GGDog/SSS"
{
    Properties
    {
        _Color("Color",Color) = (0.75,0.75,0.75,1)
        _ShadowColor("ShadowColor",Color) = (0.25,0.25,0.25,1)
        [HDR]_BackRimColor("BackRim Color",Color) = (1,0.5,0.5,1)
        [HDR]_DirRimColor("DirRim Color",Color) = (1,0.5,0.5,1)
        _ThicknessTex ("Thickness Tex", 2D) = "black" {}
        _Gloss("Gloss",Range(1,200)) = 10
        _LightDir ("LightDir", Vector) = (0,0,0,0)
    }
    SubShader
    {
            Tags { "LightMode"="ForwardBase" }
        LOD 100

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
			
            sampler2D _ThicknessTex;
            
            half _Gloss;
            
            half3 _LightDir;
            half4 frag (v2f i) : SV_Target
            {
                half4 thickness = tex2D(_ThicknessTex, i.uv+half2(0,0.05)*_Time.y);

                half3 WorldNormal = normalize(i.worldNormal);
                half3 LightDir = _LightDir;
                half3 ViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos );

				half NdotL = saturate(dot(WorldNormal,LightDir));
				half Rim = 1-saturate(smoothstep(0,1,dot(WorldNormal,ViewDir)));

                half3 reflectDir = reflect(-LightDir,WorldNormal);
                half Specular =  pow(max(0,dot(ViewDir,reflectDir)),_Gloss);

                half4 BackRimColor =  ( smoothstep(0,1,Rim*smoothstep(0.99,1.02,saturate(1-NdotL-0.0015)))  * _BackRimColor )*thickness;
                half4 DirRimColor =  (smoothstep(-0.5,1,Rim*smoothstep(0,0.25,saturate(NdotL+0.0015)))  * _DirRimColor )*thickness;

                _ShadowColor = lerp(_ShadowColor,_ShadowColor/2,smoothstep(-0.5,1,Rim));
                _Color = lerp(_Color,_Color/2,smoothstep(-0.5,1,Rim));

				half4 FinalColor = lerp(_ShadowColor + BackRimColor ,_Color + DirRimColor  ,NdotL ) +Specular*saturate(BackRimColor*3.5);

                return FinalColor;
            }
            ENDCG
        }
    }
}
