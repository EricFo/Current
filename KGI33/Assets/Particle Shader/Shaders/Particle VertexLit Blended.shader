// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Develope/VertexLit Blended" {
Properties {
    _EmisColor ("Emissive Color", Color) = (.2,.2,.2,0)
    _MainTex ("Particle Texture", 2D) = "white" {}
	
    _StencilValue ("Stencil Value", Int) = 0
    [HideInInspector] _CompareValue ("Compare Value", Int) = 3
    [HideInInspector] _WriteMask ("Write Mask", Int) = 1
}

SubShader {
    Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
    Tags { "LightMode" = "Vertex" }
    Cull Off
    Lighting On
    Material { Emission [_EmisColor] }
    ColorMaterial AmbientAndDiffuse
    ZWrite Off
    ColorMask RGB
    Blend SrcAlpha OneMinusSrcAlpha
    
    Stencil 
	{
		Ref [_StencilValue]
		WriteMask [_WriteMask]
		Comp [_CompareValue]
		Pass keep
		Fail keep
	}
    
    Pass {
        SetTexture [_MainTex] { combine primary * texture }
    }
}
}
