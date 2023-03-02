Shader "Unlit/AmbRim"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		[HDR]_RimColor ("Rim Color", Color) =(1,1,1,1)
        
        _Rim1("Rim1",Range(0,1)) = 0.2

        _Rim("Rim",Range(0,1)) = 0.5

        _LightSmoothMax("Rim Light Edge max",Range(0,1)) = 1
        _LightSmoothMin("Rim Light Edge min",Range(0,1)) = 0.8

        _LightDir("Light Dir",Vector) = (-1.5,1.5,-2,0)

        _BackLightDir("BackLight Dir",Vector) = (-1.5,1.5,-2,0)

       // _BackLightLerp("BackLightLerp",Range(0,1)) = 0

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
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		        o.worldNormal = mul(v.normal, unity_WorldToObject);
		        o.worldPos = mul(unity_ObjectToWorld , v.vertex);

                return o;
            }
            
            fixed _LightSmoothMax;
            fixed _LightSmoothMin;
            
			fixed3 _LightDir;
			fixed3 _BackLightDir;
            
		    uniform half _BackLightLerp;

			fixed _Rim1;
			fixed _Rim;

            fixed4 _RimColor;
            
            fixed4 frag (v2f i) : SV_Target
            {

                fixed4 col = tex2D(_MainTex, i.uv);
                
		        fixed3 worldPos = (i.worldPos);
		        float3 worldNormal = normalize(i.worldNormal);
		        fixed3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos);
                
		        fixed rim = smoothstep(_LightSmoothMin,_LightSmoothMax,1-dot(worldNormal,worldViewDir)) ;

                fixed Dir = saturate(dot(worldNormal,_LightDir));



                fixed4 Finalcol = col+rim*Dir*_Rim*_RimColor*saturate(1-worldPos.y/50)+_Rim1*rim + saturate(1-worldPos.y/10)*_RimColor*_Rim;
                
                
                fixed Dir2 = saturate(dot(worldNormal,_BackLightDir));

                
                fixed4 BackLightcol = lerp( 0 , 1 , step(Dir2,0.5) );

                Finalcol = lerp(Finalcol,BackLightcol,_BackLightLerp);

                return Finalcol;
            }
            ENDCG
        }
    }
}
