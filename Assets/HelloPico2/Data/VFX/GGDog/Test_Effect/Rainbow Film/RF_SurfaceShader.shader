Shader "Unlit/Unlit Specular Shader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _DiffuseCol("Diffuse",COLOR) = (1,1,1,1)
	    _SpecularCol("Specular" , COLOR) = (1,1,1,1)
	    _SpecularSharpness("Specular Sharpness",Range(0,20)) = 20
	    _SpecularScale("Specular Scale",Range(280,300)) =300
	    _max("max",Range(1,3)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
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
		        float3 Normal : TEXCOORD3;
            };

            
            sampler2D _MainTex;
            float4 _MainTex_ST;

		    fixed4 _DiffuseCol;
		    fixed4 _SpecularCol;
		    fixed _SpecularSharpness;
        
		    fixed _SpecularScale;
		    fixed _max;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
		        o.Normal = v.normal;
		        o.worldNormal = mul(v.normal, unity_WorldToObject);
		        o.worldPos = mul(unity_ObjectToWorld , v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
		        fixed3 worldPos = (i.worldPos);
		        float3 worldNormal = normalize(i.worldNormal);
		        fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
		        fixed3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos);

		        float3 halfDir = normalize( worldLightDir + worldViewDir ); 

		        float NdotH = max(0 , dot(halfDir , worldNormal));	
		        float NdotL = dot(worldNormal , worldLightDir); 
		        float NdotV = dot(worldNormal , worldViewDir); 
				
		        fixed3 diffuse = saturate(NdotL) * _DiffuseCol.rgb;
		        //fixed3 rim = saturate(pow(1-NdotV,_max)) * _DiffuseCol.rgb/3;
		        //fixed3 specular = lerp(0,i.Normal+0.25,pow(NdotH,_Gloss*_min*(1.3-tex2D(_MainTex, i.uv))) ) ;
                
		        fixed3 rim = saturate(i.Normal+0.35)*saturate(pow(1-NdotV,_max)) /5;

		        fixed3 specular = saturate(i.Normal+0.35)*pow(NdotH,_SpecularSharpness*(301-_SpecularScale)*(1.3-tex2D(_MainTex, i.uv)))  ;

		        fixed3 ads = tex2D(_MainTex, i.uv)*diffuse + rim +rim*saturate(NdotL)*5 + specular*min(2,_SpecularSharpness/10) ;
		        fixed4 col = fixed4( ads  , 1 );

		        return col;
            }
         ENDCG
        }
    }
}