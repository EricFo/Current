Shader "Demo/ParticleAddtive"
{
    Properties {
        _MainTex ("Particle Texture", 2D) = "white" {}
        _Stencil("Stencil Value", Range(0, 8)) = 0
        _CompValue("Compare Value", float) = 0
    }

    Category {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
        Blend SrcAlpha One
        Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }

        BindChannels {
            Bind "Color", color
            Bind "Vertex", vertex
            Bind "TexCoord", texcoord
        }

        SubShader {
            Stencil 
		    {
                Ref [_Stencil]
                WriteMask 255
                Comp [_CompValue]
                Pass keep
                Fail keep
		    }

            Pass {
                SetTexture [_MainTex] {
                    combine texture * primary
                }
            }
        }
    }
}
