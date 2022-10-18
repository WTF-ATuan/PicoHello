Shader "Unlit/JellyShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color("Color",Color) = (1,1,1,1)
		_ShadowColor("Shadow Color",Color) = (0.2,0.2,0.2,1)

        
		_RimColor("Rim Color",Color) = (1,1,1,1)
        _LightDir ("LightDir", Vector) = (-2,2,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

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
                float4 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				half2 Rim_NdotL : TEXCOORD1;
            };

            half2 Rotate_UV(half2 uv , half sin , half cos)
            {
                return float2(uv.x*cos - uv.y*sin ,uv.x*sin + uv.y*cos);
            }
            half WaterTex(half2 uv,half Tilling,half FlowSpeed)
            {
                uv.xy*=Tilling;
                half Time = _Time.y*FlowSpeed;

                uv.xy = Rotate_UV(uv,0.34,0.14);
                half2 UV = frac(uv.xy*0.75+Time* float2(-1,-0.25));
				half D = smoothstep(-10.4,4.2,1-38.7*((UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5))-1);
                
                uv.xy = Rotate_UV(uv,0.94,0.44);
                UV = frac(uv.xy*1.2+Time*0.33* float2(-0.24,0.33));
				half D2 = smoothstep(-18.4,4.2,1-38.7*((UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5))-1);
                
                uv.xy = Rotate_UV(uv,0.64,0.74);
                UV = frac(uv.xy*1+Time*1.34* float2(0.54,-0.33));
				half D3 = smoothstep(-15.4,4.2,1-38.7*((UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5))-1);

                //D = 1-max(max(D,D2),D3);
                D = smoothstep(-3.5,3.5,D+D2+D3);
                
                return D;
            }

            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            half3 _LightDir;
            
            uniform	 half3 Jelly_LerpPos;
            v2f vert (appdata v)
            {
                v2f o;
                
				half4 scrPos = ComputeScreenPos(UnityObjectToClipPos(v.vertex));  //抓取螢幕截圖的位置
                o.uv.zw = scrPos.xy/scrPos.w  + Jelly_LerpPos.xy;
                half Noise =WaterTex(o.uv.zw,50,0.5)+WaterTex(o.uv.zw,30,-1); 

                o.vertex = UnityObjectToClipPos(v.vertex + v.normal*(Noise*2-1)*0.5);
                o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);

                
                //計算Rim
                half3 ViewDir = normalize(_WorldSpaceCameraPos.xyz - unity_ObjectToWorld._m03_m13_m23 );
				half3 WorldNormal = normalize(mul(v.normal,(float3x3)unity_WorldToObject));
                half NdotV =  saturate(dot(WorldNormal,ViewDir.xyz));
				o.Rim_NdotL.x = 1-smoothstep(-1,0.75,NdotV);
                
                o.Rim_NdotL.y =  smoothstep(0,1.5,saturate(dot(WorldNormal,_LightDir)));

                
				o.Rim_NdotL.x = lerp(1-smoothstep(-0.5,0.75,NdotV),1-smoothstep(0,1,NdotV),o.Rim_NdotL.y);
                

                return o;
            }
            half4 _Color;
            half4 _ShadowColor;
            
            half4 _RimColor;
            
            fixed4 frag (v2f i) : SV_Target
            {
                half D =WaterTex(0.5*(i.uv.zw+i.uv.xy),10,1);
                fixed4 col = saturate(tex2D(_MainTex, i.uv.xy)+D);


               // return D*col + i.Rim.x;

               half4 FinalColor = lerp(_ShadowColor,_Color,i.Rim_NdotL.y) +i.Rim_NdotL.x* _RimColor;

                return FinalColor;
            }
            ENDCG
        }
    }
}
