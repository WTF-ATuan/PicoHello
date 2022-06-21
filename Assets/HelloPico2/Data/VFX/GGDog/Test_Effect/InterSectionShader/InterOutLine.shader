Shader "Unlit/InterOutLine"
{
Properties
{
    _OutLineColor("OutLineColor", Color) = (0,0,0,0)
	_IntersectWidth("IntersectWidth", Range(0, 1.5)) = 1.1
    _IntersectPower("IntersectPower", Float) = 2
}
SubShader
{
    ZWrite Off
    Cull Off
	Blend SrcAlpha OneMinusSrcAlpha
    Tags
    {
        "RenderType" = "Transparent"
        "Queue" = "Transparent"
    }
Pass
{
    CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
    struct appdata
{
    float4 vertex : POSITION;
};
struct v2f
{
    float4 vertex : SV_POSITION;
    float4 screenPos : TEXCOORD1;
};

sampler2D _CameraDepthTexture;

float _IntersectWidth;
float _IntersectPower;

v2f vert(appdata v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.screenPos = ComputeScreenPos(o.vertex);
    COMPUTE_EYEDEPTH(o.screenPos.z);
    return o;
}
fixed4 _OutLineColor;
fixed4 frag(v2f i) : SV_Target
{
    //判斷相交
    float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)));
    float partZ = i.screenPos.z;
    float diff = sceneZ - partZ;
    float intersect =  pow( saturate((1-diff)*_IntersectWidth) ,_IntersectPower);

	//輸出顏色: 相交+背面顏色、Rim邊緣光雙色混合
    float4 col = saturate(intersect)*_OutLineColor ;

    return col;
}
ENDCG
}
}
}
