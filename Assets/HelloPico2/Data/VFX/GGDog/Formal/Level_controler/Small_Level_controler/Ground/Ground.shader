Shader "Unlit/Ground"
{
    Properties
    {
		_SkyColor("Sky Color",Color) = (1,1,1,1)
		_ShadowColor("Shadow Color",Color) = (1,1,1,1)
        
        _SkyFarPos ("_SkyFarPos", Float) = 70
    }
    SubShader
    {
        Tags { "Queue"="Geometry-900" }
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
            };

            struct v2f
            {
                half4 vertex : SV_POSITION;
                half3 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv.xy = v.uv;
                o.uv.z = mul(unity_ObjectToWorld, v.vertex).z;

                return o;
            }
            
            half4 _SkyColor;
            half4 _ShadowColor;
            half _SkyFarPos;

            half4 frag (v2f i) : SV_Target
            {
				//���߶Z����
				half D =distance(float2(i.uv.x,i.uv.y),float2(0.5,0.5));

				D = smoothstep(0,0.5,D);

                half4 FinalColor = lerp(_ShadowColor,_SkyColor*2, D );
               
                clip(0.65-D);

				half D2 = smoothstep(0.5,1,1-D);

                
                //���t���h
                half Back = smoothstep(-_SkyFarPos,_SkyFarPos,i.uv.z);

                return half4(FinalColor.rgb,0.75*D2*Back);
            }
            ENDCG
        }
    }
}
