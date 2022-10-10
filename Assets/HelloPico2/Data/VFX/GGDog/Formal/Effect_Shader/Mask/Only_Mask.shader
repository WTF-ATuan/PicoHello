Shader "GGDog/Only_Mask"
{
	Properties
	{
		_Layer("Layer",Range(0,30)) = 0
	}
	SubShader
	{
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry"}
        ZWrite off
        Stencil {
            Ref [_Layer]
            Comp always
            Pass replace
        }
        Pass
        {
            ColorMask 0
        }
	}
}
