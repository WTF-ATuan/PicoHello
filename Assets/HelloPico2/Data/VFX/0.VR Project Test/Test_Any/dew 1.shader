Shader "GGDog/dew 1"
{
	Properties
	{
        _NoiseTex("NoiseTex", 2D) = "white" {}

		_SpecularColor("Specular Color",Color) = (1,1,1,1)

		_ShadowColor("Shadow Color",Color) = (0,0,0,1)
		
		_RimColor("Rim Color",Color) = (1,1,1,1)
		
        _Gloss("Gloss",Range(1,200)) = 10

		_Vector ("Light Direction", Vector) = (0, 0, 0)
		
	}
	SubShader
	{
        GrabPass { "_BackTex123" }

		Pass
		{	
			Tags { "LightMode"="ForwardBase" "RenderType" = "Opaque" "Queue" = "Geometry+1"}
			CGPROGRAM
            #include "Lighting.cginc"
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
				half4 CameraDistance : TEXCOORD4;
			};
			
            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;

			half4 _RimColor;
			half4 _ShadowColor;
			half4 _SpecularColor;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

                o.uv = v.uv;
				
				o.worldNormal = UnityObjectToWorldNormal(v.normal); 
				
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				
				o.scrPos = ComputeGrabScreenPos(o.vertex);  //抓取螢幕截圖的位置
				o.CameraDistance = length(mul(UNITY_MATRIX_MV,v.vertex).xyz);
				
				half2 scruv = o.scrPos.xy/o.scrPos.w;

				half4 Noise1 = tex2Dlod(_NoiseTex, half4( scruv*_NoiseTex_ST.xy +_Time.y*(0.25,-0.25)*2 ,0,0));
				half4 Noise2 = tex2Dlod(_NoiseTex, half4( scruv*_NoiseTex_ST.xy +_Time.y*(-0.25,0.25)*2 ,0,0));

				half4 Noise3 = tex2Dlod(_NoiseTex, half4( 0.55*v.vertex*_NoiseTex_ST.xy +_Time.y*(-0.3,-0.3)*2 ,0,0));
				half uvdis = (smoothstep(0,1,v.uv.y)+smoothstep(0,1,1-v.uv.y))/2;

				o.vertex = UnityObjectToClipPos(v.vertex + v.normal * pow((Noise1+Noise2+Noise3)*uvdis/2,2) );
				
				return o;
			}

            half _Gloss;
			
			half3 _Vector;

            sampler2D _BackTex123;

			half4 frag (v2f i) : SV_Target
			{

				half Time_y = abs(fmod(_Time.y*3,1.0f)*2.0f-1.0f);

				//閃動高光
				_Gloss = _Gloss+30*Time_y;

               // half3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
                half3 worldLightDir = half3(_Vector.x +0.005*Time_y,_Vector.y+0.002*Time_y,_Vector.z+0.0025*Time_y);

				half3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
				
				half3 worldNormal = normalize(i.worldNormal);


				//Rim
				half Rim = saturate(1-smoothstep(0,1,dot(worldNormal,worldViewDir)));

				//Dark
                half DarkPart =  smoothstep( 0,0.35, (1-(2*dot(worldNormal,half3(0.1,0.5,-1) )-1)) * (2*dot(worldNormal,worldViewDir)-0.5) -0.35);
				
                half DarkPart2 = smoothstep( 0.5,1, 1-dot(worldNormal,worldViewDir+half3(-0.2,-0.5,0)) +0.1)/1.5;

				DarkPart = lerp(DarkPart+DarkPart2,DarkPart2,DarkPart2)/2;
				DarkPart*=Rim;


				//高光
                half3 reflectDir = reflect(-worldLightDir,worldNormal);
                half3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos );

                half Specular =  pow(max(0,dot(viewDir,reflectDir)),_Gloss);
				
                half Specular2 = pow(max(0,dot(worldNormal,worldLightDir) ),_Gloss*100);
                half Specular3 = pow(max(0,dot(worldNormal,worldLightDir-0.35)/1.157 ),_Gloss*10);

				Specular = Specular+Specular2+Specular3;


				//底光邊緣
                half Specular_Bottom = pow(saturate(dot(viewDir,-reflectDir)),_Gloss/20);
				
				//大塊的
                half Specular_Bottom2 =  smoothstep( 0,0.5, (1-(2*dot(worldNormal,worldLightDir )+0.15)) * (2*dot(worldNormal,worldViewDir )-0.5) +0.1)/3;

				Specular_Bottom = Specular_Bottom+Specular_Bottom2;

				Specular_Bottom*=Rim;

				Specular = smoothstep(0,0.001,Specular) + smoothstep(0,0.75,Specular_Bottom);


				half4 FinalColor =lerp( _SpecularColor, half4(_ShadowColor.rgb/2,DarkPart) ,1-saturate(Specular));

				
				//內容底背景

				half2 scruv = i.scrPos.xy/i.scrPos.w;
				/*
				i.CameraDistance/=15;

				scruv = (scruv+0.5*(i.CameraDistance-1))/i.CameraDistance;
				
				half4 refrCol = tex2D(_BackTex123, scruv +(1-Rim)/50) ;
				*/
				half4 refrCol = tex2D(_BackTex123, scruv) ;

				FinalColor = lerp(FinalColor,refrCol*_ShadowColor,1-FinalColor.a);

				return FinalColor + Rim*_RimColor;
			}
			ENDCG
		}
	}
}
