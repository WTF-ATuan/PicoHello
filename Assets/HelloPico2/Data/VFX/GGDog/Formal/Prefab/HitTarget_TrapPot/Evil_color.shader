Shader "GGDog/Evil_color"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

		[HDR]_Color("Color",Color) = (1,1,1,1)
		[HDR]_ShadowColor("Shadow Color",Color) = (0.5,0.5,0.5,1)
        
        _ReflectTilling ("Reflect Tilling", Range(0,500)) = 100
        _Reflect ("Reflect", Range(0,1)) = 1
		[HDR]_ReflectColor("Reflect Color",Color) = (1,1,1,1)

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
            
            half frac_Noise(half2 UV, half Tilling)
            {

                UV = UV*Tilling/100;

                half2 n_UV =half2( UV.x *0.75 - UV.y*0.15 ,UV.x*0.15 + UV.y*0.75);

                half2 n2_UV =half2( UV.x *0.25 - UV.y*0.5 ,UV.x*0.5 + UV.y*0.25);

                half timeY =_Time.y;
                half n0 =  smoothstep(0.15,1,1-distance(frac(1*n_UV+timeY*half2(-0.3,-0.75)*0.55),0.5));
                half n01 =  smoothstep(0.3,1,1-distance(frac(0.75*n2_UV+timeY*half2(0.75,0.5)*0.25),0.5));

                half n02 =  smoothstep(0.5,1,1-distance(frac(0.25*UV+timeY*half2(0.5,-0.25)*0.75),0.5));

                half n03 =  smoothstep(0.5,1,1-distance(frac(0.15*UV+timeY*half2(0.25,-0.5)*0.75),0.5));


                half n =  smoothstep(0.15,1,distance(frac(1*n_UV+n0/3-n02/1+timeY*half2(0.7,1)*0.25),0.5)) ;

                half n2 =  smoothstep(0.3,1,distance(frac(1.25*n2_UV-n01/3+n02/1.5+timeY*half2(-0.2,-0.75)*0.75),0.5)) ;

                n+= n2;

               return saturate(n+0.25);
            }

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
            half _Reflect;
            half _ReflectTilling;
            half3 _ReflectColor;
            
            fixed4 frag (v2f i) : SV_Target
            {

                fixed4 col = tex2D(_MainTex, i.uv + _Time.y*_MainTex_ST.zw);

                half3 normalDir = normalize(i.worldNormal);

                half3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

                col.rgb =  lerp( _ShadowColor * col, _Color * col, (max(dot(normalDir,lightDir),0+0.25))) ;

                col.rgb += frac_Noise(i.worldPos.xy,_ReflectTilling)*_Reflect*_ReflectColor;

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
            
            half frac_Noise(half2 UV, half Tilling)
            {

                UV = UV*Tilling/100;

                half2 n_UV =half2( UV.x *0.75 - UV.y*0.15 ,UV.x*0.15 + UV.y*0.75);

                half2 n2_UV =half2( UV.x *0.25 - UV.y*0.5 ,UV.x*0.5 + UV.y*0.25);

                half timeY =_Time.y;
                half n0 =  smoothstep(0.15,1,1-distance(frac(1*n_UV+timeY*half2(-0.3,-0.75)*0.55),0.5));
                half n01 =  smoothstep(0.3,1,1-distance(frac(0.75*n2_UV+timeY*half2(0.75,0.5)*0.25),0.5));

                half n02 =  smoothstep(0.5,1,1-distance(frac(0.25*UV+timeY*half2(0.5,-0.25)*0.75),0.5));

                half n03 =  smoothstep(0.5,1,1-distance(frac(0.15*UV+timeY*half2(0.25,-0.5)*0.75),0.5));


                half n =  smoothstep(0.15,1,distance(frac(1*n_UV+n0/3-n02/1+timeY*half2(0.7,1)*0.25),0.5)) ;

                half n2 =  smoothstep(0.3,1,distance(frac(1.25*n2_UV-n01/3+n02/1.5+timeY*half2(-0.2,-0.75)*0.75),0.5)) ;

                n+= n2;

               return saturate(n+0.25);
            }

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
            half _Reflect;
            half _ReflectTilling;
            half3 _ReflectColor;
            

            
            fixed4 frag (v2f i) : SV_Target
            {

                fixed4 col = tex2D(_MainTex, i.uv + _Time.y*_MainTex_ST.zw);

                half3 normalDir = normalize(i.worldNormal);

                half3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

                col.rgb =  lerp( _ShadowColor * col, _Color * col, (max(dot(normalDir,lightDir),0+0.25))) ;
                
                col.rgb += frac_Noise(i.worldPos.xy,_ReflectTilling)*_Reflect*_ReflectColor;

                clip(col.a-_AlphaClip);

                return col;
            }
            ENDCG
        }
    }
}
