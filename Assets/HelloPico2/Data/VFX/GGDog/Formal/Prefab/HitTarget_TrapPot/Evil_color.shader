Shader "GGDog/Evil_color"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

		[HDR]_Color("Color",Color) = (1,1,1,1)
		[HDR]_ShadowColor("Shadow Color",Color) = (0.5,0.5,0.5,1)

        _AlphaClip ("AlphaClip", Range(0,1)) = 0.001
    }
    SubShader
    {
		Tags{ "RenderType"="transparent" "Queue"="transparent"}

		ZWrite Off

		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Cull Back
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag


            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                half3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				half3 worldPos : TEXCOORD1;
                half3 worldNormal : TEXCOORD2;
                half3 normal : NORMAL;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

				o.worldNormal = mul(v.normal,(half3x3)unity_WorldToObject);

                o.normal = v.normal;

                return o;
            }
            float _AlphaClip;
            
            half3 _Color;
            half3 _ShadowColor;

            fixed4 frag (v2f i) : SV_Target
            {

                fixed4 col = tex2D(_MainTex, i.uv + _Time.y*_MainTex_ST.zw);

                half3 normalDir = normalize(i.worldNormal);

                half3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

                col.rgb =  lerp( _ShadowColor * col, _Color * col, (max(dot(normalDir,lightDir),0+0.25))) ;


                clip(col.a-_AlphaClip);

                return col;
            }
            ENDCG
        }
        Pass
        {
            Cull Front
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag


            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                half3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				half3 worldPos : TEXCOORD1;
                half3 worldNormal : TEXCOORD2;
                half3 normal : NORMAL;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

				o.worldNormal = mul(v.normal,(half3x3)unity_WorldToObject);

                o.normal = v.normal;

                return o;
            }
            
            float _AlphaClip;
            
            half3 _Color;
            half3 _ShadowColor;

            
            fixed4 frag (v2f i) : SV_Target
            {

                fixed4 col = tex2D(_MainTex, i.uv + _Time.y*_MainTex_ST.zw);

                half3 normalDir = normalize(i.worldNormal);

                half3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

                col.rgb =  lerp( _ShadowColor * col, _Color * col, (max(dot(normalDir,lightDir),0))) ;


                clip(col.a-_AlphaClip);

                return col;
            }
            ENDCG
        }
    }
}
