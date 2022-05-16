Shader "GGDog/dew"
{
	Properties
	{
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
			Tags { "LightMode"="ForwardBase" "RenderType" = "Opaque" "Queue" = "Geometry+1"}

			ZWrite off

			Blend SrcAlpha OneMinusSrcAlpha

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
			};

			half4 _RimColor;
			half4 _ShadowColor;
			half4 _SpecularColor;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				
				//o.worldNormal = UnityObjectToWorldNormal(v.normal); 
				o.worldNormal = mul(v.normal,(half3x3)unity_WorldToObject);
				
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

				return o;
			}
            half _Gloss;
			
			half3 _Vector;

			half4 frag (v2f i) : SV_Target
			{

				half Time_y = abs(fmod(_Time.y*3,1.0f)*2.0f-1.0f);

				_Gloss = _Gloss+30*Time_y;

               // half3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
                half3 worldLightDir = half3(_Vector.x +0.005*Time_y,_Vector.y+0.002*Time_y,_Vector.z+0.0025*Time_y);

				half3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
				
				half3 worldNormal = normalize(i.worldNormal);


				//Rim
				half Rim = saturate(1-smoothstep(0,1.5,dot(worldNormal,worldViewDir)));

				//Dark
                half DarkPart =  smoothstep( 0,0.35, (1-(2*dot(worldNormal,half3(0.1,0.5,-1) )-1)) * (2*dot(worldNormal,worldViewDir)-0.5) -0.35);
				
                half DarkPart2 = smoothstep( 0.5,1, 1-dot(worldNormal,worldViewDir+half3(-0.2,-0.5,0)) +0.1)/1.5;

				DarkPart = lerp(DarkPart+DarkPart2,DarkPart2,DarkPart2)/2;


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



				Specular = smoothstep(0,0.001,Specular) + smoothstep(0,0.75,Specular_Bottom);


				half4 FinalColor =lerp( _SpecularColor, half4(_ShadowColor.rgb,DarkPart) ,1-saturate(Specular));


				return saturate(FinalColor)+Rim*_RimColor;
			}
			ENDCG
		}
	}
}
