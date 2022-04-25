Shader "GGDog/SkyBox/Sky_Color"
{
    Properties
    {
        _SkyColor ("Sky Color", Color) = (1, 1, 1, 1)
        [HDR]_HorizonColor ("Horizon Color", Color) = (1, 1, 1, 1)
		
        _SmoothStepMin ("漸層度(最低)", Range(0, 1)) = 0
        _SmoothStepMax ("漸層度(最高)", Range(0, 1)) = 1

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Front
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                fixed4 vertex : POSITION;
                fixed2 uv : TEXCOORD0;
            };

            struct v2f
            {
                fixed2 uv : TEXCOORD0;
                fixed4 vertex : SV_POSITION;
            };

            fixed4 _SkyColor;
            fixed4 _HorizonColor;

            fixed _SmoothStepMin;
            fixed _SmoothStepMax;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				fixed4 col = lerp(_HorizonColor,_SkyColor,smoothstep(_SmoothStepMin,_SmoothStepMax,i.uv.y));

                return col;
            }
            ENDCG
        }
    }
}
