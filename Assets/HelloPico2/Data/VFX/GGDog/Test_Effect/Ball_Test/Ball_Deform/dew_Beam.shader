Shader "GGDog/dew/dew_Beam"
{
	Properties
	{
        _RenderTex("Render Tex", 2D) = "white" {}

		_SpecularColor("Specular Color",Color) = (1,1,1,1)

		_ShadowColor("Shadow Color",Color) = (0,0,0,1)
		
		_RimColor("Rim Color",Color) = (1,1,1,1)
		
        _Gloss("Gloss",Range(1,200)) = 10

		_Vector ("Light Direction", Vector) = (0, 0, 0)
		
	}
	SubShader
	{
		Pass
		{	
			Tags { "RenderType" = "Opaque" "Queue" = "Geometry+1"}
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
				half3 worldNormal : TEXCOORD1;
				half3 worldPos : TEXCOORD2;
				half4 scrPos : TEXCOORD3;
				half CameraDistance : TEXCOORD4;
                UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			half4 _RimColor;
			half4 _ShadowColor;
			half4 _SpecularColor;
			
			v2f vert (appdata v)
			{
				v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
				
				o.CameraDistance = length(mul(UNITY_MATRIX_MV,v.vertex).xyz);
				
                o.worldNormal  = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

				o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				
				o.scrPos = ComputeScreenPos(o.vertex);  //抓取螢幕截圖的位置

				return o;
			}
			
            half _Gloss;
			
			half3 _Vector;

            sampler2D _BackTex123;

			sampler2D _RenderTex;

			half4 frag (v2f i) : SV_Target
			{
                UNITY_SETUP_INSTANCE_ID(i);


				half CameraDistance950_1500 =  saturate(smoothstep(30,70,i.CameraDistance));

				_ShadowColor = lerp(_ShadowColor,1,CameraDistance950_1500);
				_RimColor = lerp(_ShadowColor,0,CameraDistance950_1500);
				_Gloss = lerp(_Gloss,1000,saturate(smoothstep(700,900,i.CameraDistance)));

				half Time_y = abs(fmod(_Time.y*3,1.0f)*2.0f-1.0f);

				//閃動高光
				_Gloss = _Gloss+30*Time_y;

                half3 worldLightDir = half3(_Vector.x +0.005*Time_y,_Vector.y+0.002*Time_y,_Vector.z+0.0025*Time_y);

				half3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
				
				half3 worldNormal = normalize(i.worldNormal);

				
				half NdotV = dot(worldNormal,worldViewDir);
				//Rim
				half Rim = saturate(1-smoothstep(0,1,NdotV));

				half DarkPart = lerp((1-i.uv.y),0,CameraDistance950_1500)/2;

				//高光
                half3 reflectDir = reflect(-worldLightDir,worldNormal);
                half3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos );

                half Specular =  pow(max(0,dot(viewDir,reflectDir)),_Gloss);
                half Specular2 = pow(max(0,NdotV),_Gloss*100);
                half Specular3 = pow(max(0,dot(worldNormal,worldLightDir-0.35)/1.157 ),_Gloss*10);

				Specular = Specular+Specular2+Specular3;

				Specular = smoothstep(0,0.001,Specular) ;
				
				Specular = lerp(Specular,0,CameraDistance950_1500);

				half4 FinalColor =lerp( _SpecularColor, half4(_ShadowColor.rgb,DarkPart) ,1-saturate(Specular));
				
				FinalColor =lerp(FinalColor,FinalColor/1.75,smoothstep(0,1.5,1-dot(viewDir,reflectDir)));
				
				

				//內容底背景

				half2 scruv = i.scrPos.xy/i.scrPos.w;

				Rim = lerp(Rim,0,CameraDistance950_1500);
				
				
                half Rimscruv = 1-saturate(smoothstep(0,0.5,NdotV));
                half Rimscruv2 = saturate(smoothstep(0.25,1,NdotV));

				Rimscruv+=Rimscruv2;
				half4 refrCol = tex2D(_RenderTex, scruv + Rimscruv/50) ;

				//refrCol = lerp(refrCol,refrCol*_ShadowColor,smoothstep(-0.25,0.35,Rim*i.uv.y));

				refrCol = lerp(refrCol,refrCol*_ShadowColor,1-smoothstep(-0.25,1,Rim*i.uv.y));

				FinalColor = lerp(FinalColor,refrCol,1-FinalColor.a);
				
				FinalColor += Rim*smoothstep(0.25,1.15,1-i.uv.y)*_RimColor*1.5;
				

				return FinalColor;
			}
			ENDCG
		}
	}
}
