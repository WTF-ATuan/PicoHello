Shader "GGDog/AttractDew"
{
    Properties
    {
        _RenderTex("Render Tex", 2D) = "white" {}

		_SpecularColor("Specular Color",Color) = (1,1,1,1)
		_Color("Color",Color) = (0,0,0,1)

		_ShadowColor("Shadow Color",Color) = (0,0,0,1)
		
		_RimColor("Rim Color",Color) = (1,1,1,1)
		
        _Gloss("Gloss",Range(1,200)) = 10

		_Vector ("Light Direction", Vector) = (0, 0, 0)
		
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
                half4 vertex : POSITION;
                half2 uv : TEXCOORD0;
				half3 normal : NORMAL;
            };

            struct v2f
            {
                half3 uv : TEXCOORD0;
                half4 vertex : SV_POSITION;
                half3 worldNormal : TEXCOORD1;
                half3 worldPos : TEXCOORD2;
				half4 scrPos : TEXCOORD3;
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
                half2 UV = frac(uv.xy*0.75+Time* half2(-1,-0.25));
				half D = smoothstep(-10.4,4.2,1-38.7*((UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5))-1);
                
                uv.xy = Rotate_UV(uv,0.94,0.44);
                UV = frac(uv.xy*1.2+Time*0.33* half2(-0.24,0.33));
				half D2 = smoothstep(-18.4,4.2,1-38.7*((UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5))-1);
                
                uv.xy = Rotate_UV(uv,0.64,0.74);
                UV = frac(uv.xy*1+Time*1.34* half2(0.54,-0.33));
				half D3 = smoothstep(-15.4,4.2,1-38.7*((UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5))-1);

                //D = 1-max(max(D,D2),D3);
                D = smoothstep(-3.5,3.5,D+D2+D3);
                
                return D;
            }
            
			half4 _Color;
			half4 _RimColor;
			half4 _ShadowColor;
			half4 _SpecularColor;
			
			
            float3 SDF_Pos;

            v2f vert (appdata v)
            {
                v2f o;

                half3 WorldPos = unity_ObjectToWorld._m03_m13_m23;
                
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal  = UnityObjectToWorldNormal(v.normal);

                o.uv.xy = v.uv;
				
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.scrPos = ComputeScreenPos(o.vertex);  //抓取螢幕截圖的位置

                /*
                half d = 1-smoothstep(0,1,distance(o.worldPos,SDF_Pos)*3-1);

                half3 n = (normalize(SDF_Pos-WorldPos)/length(SDF_Pos-WorldPos))*smoothstep(0.15,0.5,dot((v.normal),(SDF_Pos-WorldPos))*d) *0.5;

                half Noise =WaterTex(v.vertex.xy,25,2.5) + WaterTex(v.vertex.xy,15,-5); 

                half Noise2 =WaterTex(v.vertex.xy,5,2.5) + WaterTex(v.vertex.xy,5,-5); 
                
                half Noise_always =WaterTex(v.vertex.xy,3,0.75) + WaterTex(v.vertex.xy,1,-1.5); 

                o.vertex = UnityObjectToClipPos(v.vertex + d*n*(Noise-1) +(Noise2-1)*smoothstep(0,3,1/distance(o.worldPos,SDF_Pos)-3.5)*v.normal*0.25 + (Noise_always-1.5)*v.normal*0.25*(1-d/2));
                */

                
                half d = 1-smoothstep(0,1,distance(o.worldPos,SDF_Pos)*3-1);

                half d_near = 2.5*smoothstep(0,0.35,distance(o.worldPos,SDF_Pos)*3);

                half3 n = normalize(SDF_Pos-WorldPos) * smoothstep(0.35,1.5,dot(normalize(o.worldNormal),normalize(SDF_Pos-WorldPos))*d) *0.5;
                
                half Noise =WaterTex(v.vertex.xy,25,2.5) + WaterTex(v.vertex.xy,15,-5); 
                
                half Noise2 =WaterTex(v.vertex.xy,5,2.5) + WaterTex(v.vertex.xy,5,-5); 
                
                half Noise_always =WaterTex(v.vertex.xy,3,0.75) + WaterTex(v.vertex.xy,1,-1.5); 

                o.vertex = UnityObjectToClipPos(v.vertex + d*d_near*n*(Noise-1) +(Noise2-1)*smoothstep(6,10,1/distance(o.worldPos,SDF_Pos)-3.5)*o.worldNormal*0.25 + (Noise_always-1.5)*o.worldNormal*0.35*(1-d/2));
                
                o.uv.z = smoothstep(0.05,0.15,distance(o.worldPos,SDF_Pos)*1);

                return o;
            }
            

            half _Gloss;
			
			half3 _Vector;

            sampler2D _BackTex123;

			sampler2D _RenderTex;

            half4 frag (v2f i) : SV_Target
            {


				half Time_y = abs(fmod(_Time.y*3,1.0f)*2.0f-1.0f);

				//閃動高光
				_Gloss = _Gloss+30*Time_y;

                half3 worldLightDir = half3(_Vector.x +0.005*Time_y,_Vector.y+0.002*Time_y,_Vector.z+0.0025*Time_y);

				half3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
				
				half3 worldNormal = normalize(i.worldNormal);

				half NdotV = dot(worldNormal,worldViewDir);
				//Rim
				half Rim = saturate(1-smoothstep(0,1,NdotV));

				half DarkPart = i.uv.z*(1-i.uv.y)/2;

				//高光
                half3 reflectDir = reflect(-worldLightDir,worldNormal);
                half3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos );

                half Specular =  pow(max(0,dot(viewDir,reflectDir)),(1/i.uv.z)*_Gloss);
                half Specular2 = pow(max(0,NdotV),(1/i.uv.z)*_Gloss*100);
                half Specular3 = pow(max(0,dot(worldNormal,worldLightDir-0.35)/1.157 ),(1/i.uv.z)*_Gloss*10);

				Specular = Specular+Specular2+Specular3;

				Specular = smoothstep(0,0.001, Specular) ;
				
				half4 FinalColor =lerp( _SpecularColor, half4(_ShadowColor.rgb,DarkPart) ,1-saturate(Specular));
				
				FinalColor =lerp(FinalColor,FinalColor/1.75,smoothstep(0,1.5,1-dot(viewDir,reflectDir)));
				
				

				//內容底背景

				half2 scruv = i.scrPos.xy/i.scrPos.w;

                half Rimscruv = 1-saturate(smoothstep(0,0.5,NdotV));
                half Rimscruv2 = saturate(smoothstep(0.25,1,NdotV));

				Rimscruv+=Rimscruv2;
				half4 refrCol = tex2D(_RenderTex, scruv + Rimscruv/50) ;

				//refrCol = lerp(refrCol,refrCol*_ShadowColor,smoothstep(-0.25,0.35,Rim*i.uv.y));

				refrCol = lerp(refrCol,refrCol*_ShadowColor,1-smoothstep(-0.25,1,Rim*i.uv.y));

				FinalColor = lerp(FinalColor,refrCol,1-FinalColor.a);
				
				FinalColor += i.uv.z*Rim*smoothstep(0.25,1.15,1-i.uv.y)*_RimColor*1.5 ;
				 
				return FinalColor+_Color*smoothstep(-0.5,1.15,i.uv.y);
				
				
            }
            ENDCG
        }
    }
}
