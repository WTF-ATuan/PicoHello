Shader "GGDog/PlayerB_God"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _DarkColor ("Dark Color", Color) = (0.5,0.5,0.5,1)
        [Enum(Order,4,AlwaysOnTop,8)] _ZTest("ZTest", Float) = 8
    }
    SubShader
    {
        Tags {"RenderType"="Transparent"  "Queue"="Transparent+2000" }
        
        ZTest[_ZTest]
		
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                half4 vertex : POSITION;
                half2 uv : TEXCOORD0;
				half3 normal : NORMAL;
                half4 color : COLOR;
            };

            struct v2f
            {
                half2 uv : TEXCOORD0;
                half4 vertex : SV_POSITION;
				half3 worldNormal : TEXCOORD1;
				half3 worldPos : TEXCOORD2;
                half4 color : COLOR;
            };

            sampler2D _MainTex;
            half4 _MainTex_ST;
            half4 _Color;
            half4 _DarkColor;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = v.color;

				o.worldNormal = mul(v.normal,(half3x3)unity_WorldToObject);
				
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
				half3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
				
				half3 worldNormal = normalize(i.worldNormal);

				half Rim = (smoothstep(0.5,0.5,dot(worldNormal,worldViewDir)));

                half4 col = tex2D(_MainTex, i.uv);
                
                col.a = i.color.a;

                col.rgb = lerp(col.rgb*_Color,_DarkColor,Rim);

                
                return col;
            }
            ENDCG
        }
    }
}
