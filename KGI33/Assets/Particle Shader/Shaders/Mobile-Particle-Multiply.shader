// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Simplified Multiply Particle shader. Differences from regular Multiply Particle one:
// - no Smooth particle support
// - no AlphaTest
// - no ColorMask

Shader "Develope/Mobile/Multiply" {
Properties {
    _MainTex ("Particle Texture", 2D) = "white" {}
    
    _StencilValue ("Stencil Value", Int) = 0
    [HideInInspector] _CompareValue ("Compare Value", Int) = 3
    [HideInInspector] _WriteMask ("Write Mask", Int) = 1
}

Category {
    Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
    Blend Zero SrcColor
    Cull Off Lighting Off ZWrite Off Fog { Color (1,1,1,1) }

    Stencil 
	{
		Ref [_StencilValue]
		WriteMask [_WriteMask]
		Comp [_CompareValue]
		Pass keep
		Fail keep
	}
    
    BindChannels {
        Bind "Color", color
        Bind "Vertex", vertex
        Bind "TexCoord", texcoord
    }

    SubShader {
        Pass {
            SetTexture [_MainTex] {
                combine texture * primary
            }
            SetTexture [_MainTex] {
                constantColor (1,1,1,1)
                combine previous lerp (previous) constant
            }
        }
    }
}
}
