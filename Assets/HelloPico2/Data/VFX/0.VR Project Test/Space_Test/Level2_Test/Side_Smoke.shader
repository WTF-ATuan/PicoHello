Shader "Unlit/Side_Smoke"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _ShadowColor ("ShadowColor", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
		_Far("Far",Range(0,500))=100

		_IntersectWidth("IntersectWidth", Range(0, 3.5)) = 1.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
			Blend SrcAlpha OneMinusSrcAlpha
			Cull off
			Zwrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float3 worldNormal : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
			    float4 screenPos : TEXCOORD3;
            };
			
			sampler2D _CameraDepthTexture;
	
			float _IntersectWidth;
			
            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;

                fixed3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                o.vertex = UnityObjectToClipPos(v.vertex +  fixed3( cos( worldPos.z*5 - _Time.y*2 ), 0, sin( worldPos.z*10 - _Time.y*2 )-cos( worldPos.z*1 - _Time.y*1 ) )/10);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				
				o.worldNormal = mul(v.normal,(float3x3)unity_WorldToObject);
				
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
			    o.screenPos = ComputeScreenPos(o.vertex);
			    COMPUTE_EYEDEPTH(o.screenPos.z);

                return o;
            }

            float4 _Color;
            float4 _ShadowColor;
			
			float _Far;
			
            fixed4 frag (v2f i) : SV_Target
            {
			
			    float partZ = i.screenPos.z *smoothstep(0,_Far/10,i.screenPos.y) ;
				
				partZ = saturate(smoothstep(0,_Far, partZ));


				fixed3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
				
				fixed3 worldNormal = normalize(i.worldNormal);
				
				float Rim = saturate(smoothstep(0.1*(partZ),0.75*(partZ),dot(worldNormal,worldViewDir)));

                fixed4 col = tex2D(_MainTex, i.uv - _Time.y*float2(0,0.2))*5+0.5;
				
				col = lerp(_ShadowColor,_Color,saturate(col.r+partZ*10));

				col.a*=(1-smoothstep(0,0.5,i.worldPos.y))*Rim;

                return col;
            }
            ENDCG
        }
    }
}
