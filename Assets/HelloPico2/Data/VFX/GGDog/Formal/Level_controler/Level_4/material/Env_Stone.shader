Shader "GGDog/Env_StoneParticle"
{
	Properties
	{
		_Color("Color",Color) = (1,1,1,1)
		_Color_light("Color light",Color) = (1,1,1,1)

        
        _ColorLerp("ColorLerp", Range(0,1)) = 0
        
		_Color2("Color",Color) = (1,1,1,1)
		_Color2_light("Color light",Color) = (1,1,1,1)

	}
    SubShader
    {
        Tags { "RenderType"="Opaque"}

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                half4 vertex : POSITION;
            };

            struct v2f
            {
                half4 vertex : SV_POSITION;
				half3 worldPos : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

			fixed4 _Color;
			fixed4 _Color_light;
			fixed4 _Color2;
			fixed4 _Color2_light;

			fixed _ColorLerp;
            
            fixed4 frag (v2f i) : SV_Target
            {
                _Color = lerp(_Color,_Color2,_ColorLerp);
                _Color_light = lerp(_Color_light,_Color2_light,_ColorLerp);

                fixed4 FinalColor = lerp(_Color,_Color_light,smoothstep(-20,0,i.worldPos.y));
				
                return FinalColor;
            }
            ENDCG
        }
    }
}
