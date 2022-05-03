Shader "Effect/Gate_shader" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        [HDR]_TintColor ("Color", Color) = (1,1,1,1)
        _Tex_Offs_U ("Tex_Offs_U", Float ) = 1
        _Tex_Offs_V ("Tex_Offs_V", Float ) = 1
        _Mask_Tex ("Mask_Tex", 2D) = "white" {}
        _Mask_R_UV_vertex_offs ("Mask_R_UV_vertex_offs", Vector) = (1,1,0,0)
        _Ver_Offs ("Ver_Offs", Float ) = 1
        _Vert_Offs_int ("Vert_Offs_int", Float ) = 0
        _Mask_G_UV_alpha ("Mask_G_UV_alpha", Vector) = (1,1,0,0)
        _Fres_R_UV ("Fres_R_UV", Vector) = (1,1,0,0)
        _Vortex ("Vortex", Float ) = 0
        _Glow_Tex_R ("Glow_Tex_R", 2D) = "white" {}
        _Glow_Tex_G ("Glow_Tex_G", 2D) = "white" {}
        _Glow_Tex_B ("Glow_Tex_B", 2D) = "white" {}
        _Glow_offs_G ("Glow_offs_G", Vector) = (1,1,0,0)
        _Glow_offs_B ("Glow_offs_B", Vector) = (1,1,0,0)
        _glow_pow ("glow_pow", Float ) = 1
        _glow_pow_copy ("glow_pow_copy", Float ) = 1
        _Normal_int ("Normal_int", Float ) = 0
        _Normal_B_UV_offs ("Normal_B_UV_offs", Vector) = (1,1,0,0)
        _alpha1 ("alpha1", Float ) = 0
        _node_2871 ("node_2871", Vector) = (0,0,1,1)
        _fres ("fres", 2D) = "gray" {}
        _fres_int ("fres_int", Float ) = 0
        _node_4305 ("node_4305", Float ) = 1
        _SoftPar_edge ("SoftPar_edge", Float ) = 1
        _SoftPar_offs ("SoftPar_offs", Float ) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        _Stencil ("Stencil ID", Float) = 0
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilComp ("Stencil Comparison", Float) = 8
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilOpFail ("Stencil Fail Operation", Float) = 0
        _StencilOpZFail ("Stencil Z-Fail Operation", Float) = 0
    }
    SubShader {
        Tags {
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
        }
        Pass {
            Name "FORWARD"
            Cull Off
            ZTest Always
            
            Stencil {
                Ref [_Stencil]
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
                Comp [_StencilComp]
                Pass [_StencilOp]
                Fail [_StencilOpFail]
                ZFail [_StencilOpZFail]
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows

            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _Mask_Tex; uniform float4 _Mask_Tex_ST;
            uniform sampler2D _Glow_Tex_R; uniform float4 _Glow_Tex_R_ST;
            uniform sampler2D _Glow_Tex_G; uniform float4 _Glow_Tex_G_ST;
            uniform sampler2D _Glow_Tex_B; uniform float4 _Glow_Tex_B_ST;
            uniform sampler2D _fres; uniform float4 _fres_ST;
            UNITY_INSTANCING_BUFFER_START( Props )
                UNITY_DEFINE_INSTANCED_PROP( float4, _TintColor)
                UNITY_DEFINE_INSTANCED_PROP( float, _Ver_Offs)
                UNITY_DEFINE_INSTANCED_PROP( float, _Tex_Offs_U)
                UNITY_DEFINE_INSTANCED_PROP( float, _Tex_Offs_V)
                UNITY_DEFINE_INSTANCED_PROP( float4, _Mask_G_UV_alpha)
                UNITY_DEFINE_INSTANCED_PROP( float4, _Mask_R_UV_vertex_offs)
                UNITY_DEFINE_INSTANCED_PROP( float4, _Fres_R_UV)
                UNITY_DEFINE_INSTANCED_PROP( float, _Vert_Offs_int)
                UNITY_DEFINE_INSTANCED_PROP( float, _Vortex)
                UNITY_DEFINE_INSTANCED_PROP( float4, _Glow_offs_G)
                UNITY_DEFINE_INSTANCED_PROP( float4, _Glow_offs_B)
                UNITY_DEFINE_INSTANCED_PROP( float, _glow_pow)
                UNITY_DEFINE_INSTANCED_PROP( float, _glow_pow_copy)
                UNITY_DEFINE_INSTANCED_PROP( float, _Normal_int)
                UNITY_DEFINE_INSTANCED_PROP( float4, _Normal_B_UV_offs)
                UNITY_DEFINE_INSTANCED_PROP( float, _alpha1)
                UNITY_DEFINE_INSTANCED_PROP( float4, _node_2871)
                UNITY_DEFINE_INSTANCED_PROP( float, _fres_int)
                UNITY_DEFINE_INSTANCED_PROP( float, _node_4305)
                UNITY_DEFINE_INSTANCED_PROP( float, _SoftPar_edge)
                UNITY_DEFINE_INSTANCED_PROP( float, _SoftPar_offs)
            UNITY_INSTANCING_BUFFER_END( Props )
            struct VertexInput {
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                UNITY_SETUP_INSTANCE_ID( v );
                UNITY_TRANSFER_INSTANCE_ID( v, o );
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
                float3 node_2458 = mul( unity_WorldToObject, float4((mul(unity_ObjectToWorld, v.vertex).rgb-objPos.rgb),0) ).xyz;
                float _Vert_Offs_int_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Vert_Offs_int );
                float4 _Mask_R_UV_vertex_offs_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Mask_R_UV_vertex_offs );
                float2 node_1309 = float2(((_Mask_R_UV_vertex_offs_var.r*o.uv0.r)+(_Mask_R_UV_vertex_offs_var.b*_Time.g)),((o.uv0.g*_Mask_R_UV_vertex_offs_var.g)+(_Time.g*_Mask_R_UV_vertex_offs_var.a)));
                float4 _Alpha_Tex_R = tex2Dlod(_Mask_Tex,float4(TRANSFORM_TEX(node_1309, _Mask_Tex),0.0,0));
                float4 _Mask_G_UV_alpha_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Mask_G_UV_alpha );
                float2 node_7622 = float2(((_Mask_G_UV_alpha_var.r*o.uv0.r)+(_Mask_G_UV_alpha_var.b*_Time.g)),((o.uv0.g*_Mask_G_UV_alpha_var.g)+(_Time.g*_Mask_G_UV_alpha_var.a)));
                float4 _Alpha_Tex_G = tex2Dlod(_Mask_Tex,float4(TRANSFORM_TEX(node_7622, _Mask_Tex),0.0,0));
                float mask_R = ((_Alpha_Tex_R.r+(0.2*_Alpha_Tex_G.g))*0.5);
                float _Ver_Offs_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Ver_Offs );
                float4 _Normal_B_UV_offs_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Normal_B_UV_offs );
                float2 node_1101 = float2(((_Normal_B_UV_offs_var.r*o.uv0.r)+(_Normal_B_UV_offs_var.b*_Time.g)),((o.uv0.g*_Normal_B_UV_offs_var.g)+(_Time.g*_Normal_B_UV_offs_var.a)));
                float4 _NormalTexure = tex2Dlod(_Mask_Tex,float4(TRANSFORM_TEX(node_1101, _Mask_Tex),0.0,0));
                float _Normal_int_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Normal_int );
                v.vertex.xyz += (((node_2458.rgb * dot(v.normal,node_2458.rgb)/dot(node_2458.rgb,node_2458.rgb))*_Vert_Offs_int_var*lerp(1.0,(mask_R+(-0.5)),_Ver_Offs_var))+(v.normal*(_NormalTexure.b+0.5)*_Normal_int_var*o.vertexColor.r));
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {

                float4 _node_2871_var = UNITY_ACCESS_INSTANCED_PROP( Props, _node_2871 );
                float _node_4305_var = UNITY_ACCESS_INSTANCED_PROP( Props, _node_4305 );
                float4 _Glow_Tex_R_var = tex2D(_Glow_Tex_R,TRANSFORM_TEX(i.uv0, _Glow_Tex_R));
                float _glow_pow_var = UNITY_ACCESS_INSTANCED_PROP( Props, _glow_pow );
                float4 _Glow_offs_G_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Glow_offs_G );
                float2 node_5179 = float2(((_Glow_offs_G_var.r*i.uv0.r)+(_Glow_offs_G_var.b*_Time.g)),((i.uv0.g*_Glow_offs_G_var.g)+(_Time.g*_Glow_offs_G_var.a)));
                float4 _Glow_Tex_G_var = tex2D(_Glow_Tex_G,TRANSFORM_TEX(node_5179, _Glow_Tex_G));
                float4 _Glow_offs_B_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Glow_offs_B );
                float2 node_1501 = float2(((_Glow_offs_B_var.r*i.uv0.r)+(_Glow_offs_B_var.b*_Time.g)),((i.uv0.g*_Glow_offs_B_var.g)+(_Time.g*_Glow_offs_B_var.a)));
                float4 _Glow_Tex_B_var = tex2D(_Glow_Tex_B,TRANSFORM_TEX(node_1501, _Glow_Tex_B));
                float _glow_pow_copy_var = UNITY_ACCESS_INSTANCED_PROP( Props, _glow_pow_copy );
                float node_1883 = pow((1.0+(_Glow_Tex_G_var.g*_Glow_Tex_B_var.b)),_glow_pow_copy_var);
                float glow_tex_mask = (pow((1.0+_Glow_Tex_R_var.r),_glow_pow_var)*node_1883);
                float _alpha1_var = UNITY_ACCESS_INSTANCED_PROP( Props, _alpha1 );
                float4 _TintColor_var = UNITY_ACCESS_INSTANCED_PROP( Props, _TintColor );
                float _SoftPar_offs_var = UNITY_ACCESS_INSTANCED_PROP( Props, _SoftPar_offs );
                float _SoftPar_edge_var = UNITY_ACCESS_INSTANCED_PROP( Props, _SoftPar_edge );
                clip(saturate(((lerp((1.0 - length(((frac((float2(_node_2871_var.b,_node_2871_var.a)*i.uv0))+(-0.5))*2.0))),saturate(((1.0 - (abs((i.uv0.g+(-0.5)))*2.0))*_node_4305_var*glow_tex_mask)),_alpha1_var)*_TintColor_var.a*saturate(((_SoftPar_offs_var+i.posWorld.g)*_SoftPar_edge_var)))+saturate(((1.0 - i.vertexColor.r)*22.0)))) - 0.5);
                float _Vortex_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Vortex );
                float _Tex_Offs_U_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Tex_Offs_U );
                float _Tex_Offs_V_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Tex_Offs_V );
                float2 node_3101 = float2((((_Vortex_var*i.uv0.g)+i.uv0.r)+(_Tex_Offs_U_var*_Time.g)),(i.uv0.g+(_Time.g*_Tex_Offs_V_var)));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_3101, _MainTex));
                float4 _Fres_R_UV_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Fres_R_UV );
                float2 node_7343 = float2(((_Fres_R_UV_var.r*i.uv0.r)+(_Fres_R_UV_var.b*_Time.g)),((i.uv0.g*_Fres_R_UV_var.g)+(_Time.g*_Fres_R_UV_var.a)));
                float4 _fres_var = tex2D(_fres,TRANSFORM_TEX(node_7343, _fres));
                float _fres_int_var = UNITY_ACCESS_INSTANCED_PROP( Props, _fres_int );
                float node_5882 = pow((_fres_var.r+1.0),_fres_int_var);
                float3 final_col = (_MainTex_var.rgb*glow_tex_mask*_TintColor_var.rgb*node_5882);
                float3 emissive = final_col;
                return fixed4(emissive,1);
            }
            ENDCG
        }
    }
}
