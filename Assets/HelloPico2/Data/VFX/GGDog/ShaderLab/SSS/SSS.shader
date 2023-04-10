Shader "Unlit/SSS"
{
    Properties
    {
        _LightDir("Light Dir",Vector) = (-1.5,1.5,-2,0)
        _MainColor("Main Color",Color) = (1,0.82,0.64,1)
        _ShadowColor("Shadow Color",Color) = (0.49,0.5,0.8,1)
        _SSSColor("SSS Color",Color) = (0.49,0.5,0.8,1)
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
            
			#pragma target 3.0
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct appdata
            {
                half4 vertex : POSITION;
                half2 uv : TEXCOORD0;
				half3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                half2 uv : TEXCOORD0;
                half4 vertex : SV_POSITION;
				half2 L_VDotN : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
			half3 _LightDir;
            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                half3 normal = normalize(mul(v.normal, (float3x3)unity_WorldToObject));

                half3 viewDir = normalize(_WorldSpaceCameraPos.xyz - unity_ObjectToWorld._m03_m13_m23);
                
                half3 normal_VS = normalize(mul(UNITY_MATRIX_MV,v.normal));
                
                o.L_VDotN.x = dot(normal_VS,_LightDir) ;
                
                o.L_VDotN.y = dot(viewDir,normal);

                return o;
            }
            
            half4 _MainColor;
            half4 _ShadowColor;
            half4  _SSSColor;

            half4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);

                half SSS = smoothstep(-0.75,1,saturate(1-i.L_VDotN.y)) ;
                
                half Rim = smoothstep(0.5,0.75,1-i.L_VDotN.y)*0.5;

                half4 FinalCol = lerp( SSS*_SSSColor+_ShadowColor ,_MainColor , i.L_VDotN.x - i.L_VDotN.x*Rim) ;

                return FinalCol;
            }
            ENDCG
        }
    }
}
