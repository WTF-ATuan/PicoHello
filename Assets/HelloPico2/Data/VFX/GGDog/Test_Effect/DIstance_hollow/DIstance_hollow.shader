Shader "Unlit/DIstance_hollow"
{
    Properties
    {
        _Color ("Color Tint", Color) = (1, 1, 1, 1)
        _ShadowColor ("ShadowColor", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}

		_Distance_Intensive("Intensive",Range(0,3))=2.25
		_Distance_Radius("Radius",Range(0,3))=1
        _worldLightDir("LightDir",Vector) =(1,0.5,-0.5,0)
    }
    SubShader
    {

            LOD 100

        Pass
        {
			Cull Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
           // #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				float4 normal : NORMAL;
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
			
			fixed4 GLOBAL_Pos;
			
			fixed _Distance_Intensive;
			fixed _Distance_Radius;
			
            v2f vert (appdata v)
            {
                half3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz ;

				half d = saturate(distance( GLOBAL_Pos,worldPos/length(v.vertex)) / (_Distance_Radius/(1+distance( GLOBAL_Pos,worldPos)) ) );

                _Distance_Intensive*= saturate(distance( GLOBAL_Pos,worldPos/length(v.vertex))+0.25);


				half3 n =  normalize((worldPos-(v.normal/1.5)/(1+distance( GLOBAL_Pos,worldPos)))-GLOBAL_Pos)*_Distance_Intensive*smoothstep(0,1.5,1-d);

                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex + n/(2+d) );
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                o.worldNormal  = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                return o;
            }
			
            fixed4 _Color;
            fixed4 _ShadowColor;

            fixed3 _worldLightDir ;
            fixed4 frag (v2f i) : SV_Target
            {
                fixed3 worldNormal = normalize(i.worldNormal);

                fixed4 col = tex2D(_MainTex, i.uv);

				//«G­±
                fixed3 Light_Part =  col.rgb * _Color.rgb;
				
				//·t­±
                fixed3 Shadow_Part =  col.rgb *_ShadowColor;

			    col = fixed4( (lerp(Shadow_Part,Light_Part, saturate(dot(worldNormal,_worldLightDir.xyz))))  , 1);


                return col;
            }
            ENDCG
        }
    }
}
