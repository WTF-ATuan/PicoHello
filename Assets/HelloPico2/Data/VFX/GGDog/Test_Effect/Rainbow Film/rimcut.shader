Shader "Unlit/rimcut"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MainTex2 ("Texture2", 2D) = "white" {}
    }
    SubShader
    {
        Pass
        {
            Tags { "Queue"="Geometry-1" }
            ZWrite Off
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
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            sampler2D _MainTex2;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex + v.normal*0.75);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		        o.worldNormal = mul(v.normal, unity_WorldToObject);
		        o.worldPos = mul(unity_ObjectToWorld , v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 col2 = tex2D(_MainTex2, i.uv);

		        fixed3 worldPos = (i.worldPos);
		        float3 worldNormal = normalize(i.worldNormal);
		        fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
		        fixed3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos);
                
		        float3 halfDir = normalize( worldLightDir + worldViewDir); 
		        float NdotH = max(0 , dot(halfDir , worldNormal));	
		        fixed s = step(NdotH,0.95) ;
                
		        fixed r = step(dot(worldNormal,worldViewDir),0.5) ;

                fixed c =smoothstep(0.5,0.8,dot(worldNormal,worldViewDir));
                
		        float NdotL = dot(worldNormal , worldLightDir); 
		        fixed diffuse = smoothstep(0,1,NdotL-col*0.5);

                clip(1-r-1);
                
                clip(col*c-0.25);
                return diffuse;
            }
            ENDCG
        }
        Pass
        {
            Tags { "Queue"="Geometry" }
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
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            sampler2D _MainTex2;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		        o.worldNormal = mul(v.normal, unity_WorldToObject);
		        o.worldPos = mul(unity_ObjectToWorld , v.vertex );
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 col2 = tex2D(_MainTex2, i.uv);
                
		        fixed3 worldPos = (i.worldPos);
		        float3 worldNormal = normalize(i.worldNormal);
		        fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
		        fixed3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos);
                
		        float NdotL = dot(worldNormal , worldLightDir); 
		        fixed diffuse = smoothstep(0,1,saturate(NdotL-col*0.5)) ;

                
		        float NdotV = dot(worldNormal , worldViewDir); 
		        fixed rim = smoothstep(0.25,1,1-saturate(NdotV-col*0.5)) /3;

                return diffuse;
            }
            ENDCG
        }
        
    }
}
