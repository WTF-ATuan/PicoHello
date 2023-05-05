Shader "Unlit/Gel"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
	    _a("a" , Range(0,1)) =1
        [HDR]_GlowColor("Diffuse",COLOR) = (1,1,1,1)
        [HDR]_DarkRimColor("Diffuse",COLOR) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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
		        half3 worldNormal : TEXCOORD1;
		        half3 worldPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _GlowColor;
            float _a;
            float4 _DarkRimColor;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                o.worldNormal = mul(v.normal, unity_WorldToObject);
		        o.worldPos = mul(unity_ObjectToWorld , v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                _MainTex_ST.y*= sin(_Time.y);

                fixed4 col = tex2D(_MainTex, i.uv*half2(1-0.5*(sin(_Time.y)+0.5)*0.1,1-0.5*(sin(_Time.y)+0.5)*0.1  ));
                
		        half3 worldNormal = normalize(i.worldNormal);
		        half3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);

		        half NdotV = dot(worldNormal , worldViewDir); 
                
		        half4 rim = saturate(smoothstep(0.5,0.9,1-NdotV));

                
		        half4 Col_rim = saturate(smoothstep(-0.25,1,NdotV))*_GlowColor;
                
		        half4 Col_rim2 = saturate(smoothstep(0,0.75,NdotV))*_GlowColor;
               col.a*=_a;

                col = lerp(Col_rim,col,col.a);

                rim = lerp(rim,_DarkRimColor*0.85*rim.a,col.a);

                return col+rim + col.a*Col_rim2/5;
            }
            ENDCG
        }
    }
}
