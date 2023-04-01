Shader "Unlit/Glass"
{
    Properties
    {
        _RenderTex("Render Tex", 2D) = "white" {}
        
		_Vector ("Light Direction", Vector) = (0, 0, 0)
		_Specular ("Specular",Range(8.0,256)) = 20
		[HDR]_GlassColor ("GlassColor",Color) = (1,1,1,1)
		[HDR]_LiquidColor ("LiquidColor",Color) = (1,1,1,1)
		[HDR]_LiquidColor2 ("LiquidColor2",Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha

        ZWrite Off
        

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
            };

            struct v2f
            {
                half2 uv : TEXCOORD0;
                half4 vertex : SV_POSITION;
				half3 worldNormal : TEXCOORD1;
				half3 worldPos : TEXCOORD2;
				half4 scrPos : TEXCOORD3;
            };
            
            half2 Rotate_UV(half2 uv , half sin , half cos)
            {
                return half2(uv.x*cos - uv.y*sin ,uv.x*sin + uv.y*cos);
            }
            half WaterTex(half2 uv,half Tilling,half FlowSpeed)
            {
                uv.xy*=Tilling;
                half Time = _Time.y*FlowSpeed;

                uv.xy = Rotate_UV(uv,0.34,0.14);
                half2 UV = frac(uv.xy*0.75+Time* half2(-1.0,-0.25));
                half UV_Center = (UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5);
				half D = smoothstep(-10.4,4.2,1.0-38.7*UV_Center-1.0);
                
                uv.xy = Rotate_UV(uv,0.94,0.44);
                UV = frac(uv.xy*1.2+Time*0.33* half2(-1.74,0.33));
                UV_Center = (UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5);
				half D2 = smoothstep(-18.4,4.2,1.0-38.7*UV_Center-1.0);
                
                D = max(D,D2);
                
                return D;
            }

            v2f vert (appdata v)
            {
                v2f o;

                o.worldNormal  = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                
				o.scrPos = ComputeScreenPos(o.vertex);  //抓取螢幕截圖的位置

                return o;
            }
            
			sampler2D _RenderTex;
			
			half3 _WorldLightDir;
			half _Specular;
			half4 _LiquidColor;
			half4 _LiquidColor2;
			half4 _GlassColor;
            
            half3 Fixed_WorldPos;
            half FullPos;
            half4 frag (v2f i) : SV_Target
            {
                
				half3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
				half3 worldNormal = normalize(i.worldNormal);
                
				half NdotV = dot(worldNormal,worldViewDir);

				//Rim
				half Rim = saturate(1-smoothstep(0,1,NdotV));
                
                half Rimscruv = 1-saturate(smoothstep(0,0.5,NdotV));
                half Rimscruv2 = saturate(smoothstep(0.25,1,NdotV));

				Rimscruv+=Rimscruv2;
				half2 scruv = i.scrPos.xy/i.scrPos.w;
				half4 refrCol = tex2D(_RenderTex, scruv + Rimscruv/200) ;


                Rim = smoothstep(0.65,1,Rim);
                
				//高光
		        half3 halfDir = normalize( _WorldLightDir + worldViewDir+0.5); 
		        half NdotH = max(0 , dot(halfDir , worldNormal));	
		        half specular = smoothstep(0.995,0.996,NdotH)  ;

                half Y =i.worldPos.y-Fixed_WorldPos.y-FullPos;

                half Flow = (Y-0.01) *((WaterTex(i.worldPos.xz,10,1)-0.5));

                half4 Liquid = smoothstep(0,0.001,1-normalize(Y + Flow ));

                _LiquidColor = lerp(_LiquidColor2,_LiquidColor,smoothstep(-0.025,0,Y));

                half4 FinalColor = _LiquidColor*Liquid  + (Rim+specular)*_GlassColor;
                

                return FinalColor;
            }
            ENDCG
        }
    }
}
