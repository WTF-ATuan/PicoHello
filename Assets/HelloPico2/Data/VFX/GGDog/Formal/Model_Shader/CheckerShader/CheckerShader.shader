Shader "Custom/Normal_Check"
{
    Properties
    {
        _LineLength("LineLength",float) = 0.03
        _LineColor("LineColor",COLOR) = (1,0,0,1)
    }
        SubShader
    {
        
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return smoothstep(0+0.5*sin(_Time.y*5),1-0.5*sin(_Time.y*5),i.uv.y);
            }
            ENDCG
        }


        Pass
        {
            Tags { "RenderType" = "Opaque" }
            LOD 200
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
                #pragma target 5.0
                #pragma vertex VS_Main
                #pragma fragment FS_Main
                #pragma geometry GS_Main
                #include "UnityCG.cginc"
 
                float _LineLength;
                fixed4 _LineColor;
                 
 
                struct GS_INPUT
                {
                    float4    pos       : POSITION;
                    float3    normal    : NORMAL;
                    float2  tex0        : TEXCOORD0;
                };
                struct FS_INPUT
                {
                    float4    pos       : POSITION;
                    float2  tex0        : TEXCOORD0;
                };
                //step1
                GS_INPUT VS_Main(appdata_base v)
                {
                    GS_INPUT output = (GS_INPUT)0;
                    output.pos = mul(unity_ObjectToWorld, v.vertex);
                    //output.pos = UnityObjectToClipPos(v.vertex);
                    output.normal = v.normal;
                    //float4 viewNormal = mul(UNITY_MATRIX_IT_MV, float4(v.normal, 0));
                    float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                    output.normal = normalize(worldNormal);
                    output.tex0 = float2(0, 0);
                    return output;
                }
                [maxvertexcount(4)]
                void GS_Main(point GS_INPUT p[1], inout LineStream<FS_INPUT> triStream)
                {
                    FS_INPUT pIn;
                    pIn.pos =mul(UNITY_MATRIX_VP, p[0].pos);// UnityObjectToClipPos(p[0].pos);
                    pIn.tex0 = float2(0.0f, 0.0f);
                    triStream.Append(pIn);
                    FS_INPUT pIn1;
                    float4 pos= p[0].pos + float4(p[0].normal,0) *_LineLength;
                    pIn1.pos = mul(UNITY_MATRIX_VP, pos);
                    pIn1.tex0 = float2(0.0f, 0.0f);
                    triStream.Append(pIn1);
 
                }
 
                //step3
                fixed4 FS_Main(FS_INPUT input) : COLOR
                {
                    return _LineColor;
                }
            ENDCG
        }


    }
}