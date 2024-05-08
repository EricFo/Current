// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Sprites/Default"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
        _ColorPart("ColorPart", int) = 6
    }

    SubShader
    {
        Tags {}




        Blend One OneMinusSrcAlpha
	
        Pass
        {
            CGPROGRAM
            #pragma vertex  vert
            #pragma fragment frag
            #pragma target 3.0
            #pragma multi_compile_instancing


            #pragma enable_d3d11_debug_symbols
            //   #include "UnitySprites.cginc"
            #include "UnityCG.cginc"
            fixed4 _RendererColor;
            fixed4 _Color;
            sampler2D _MainTex;
            sampler2D _AlphaTex;

            struct a2v
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float3 normal : NORMAL;

                float2 texcoord : TEXCOORD0;
            };

            struct v2f_s
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float4 localPos :TEXCOORD2;
                float3 worldNormal : TEXCOORD3;
            };
		fixed _ColorPart;

            v2f_s vert(a2v IN)
            {
                v2f_s OUT;


                //  OUT.vertex = UnityFlipSprite(IN.vertex, _Flip);
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.uv = IN.texcoord;
                OUT.color = IN.color * _Color * _RendererColor;
                OUT.localPos = IN.vertex;
                // OUT.tangent = IN.tangent;
                OUT.worldNormal = UnityObjectToWorldNormal(IN.normal);

                OUT.worldPos = mul(unity_ObjectToWorld, IN.vertex).xyz;

                return OUT;
            }
	fixed3 hsb2rgb(fixed3 c) {
				fixed3 rgb = clamp(abs(fmod(c.x*6.0 + fixed3(0.0, 4.0, 2.0),
					6.0)-3.0)-1.0,0.0,1.0);
				rgb = rgb * rgb*(3.0 - 2.0 * rgb);
				return c.z * lerp(fixed3(1.0,1.0,1.0), rgb, c.y);
			}


            fixed4 frag(v2f_s i) : SV_Target
            {
                fixed4 color = tex2D(_MainTex, i.uv);
                color.rgb *= color.a;
                fixed2 dir = fixed2(i.uv.x, i.uv.y) - fixed2(0.5, 0.5);
                fixed radius = length(dir);
                fixed theta = atan2(dir.y, dir.x);
                fixed s = 1 - radius;
                fixed h = theta / (2 * 3.14) + 0.5;
                fixed4 col = fixed4(hsb2rgb(fixed3(floor(h * _ColorPart) / _ColorPart, s, 1)), 1);


                return color * col;
            }
            ENDCG
        }
    }
}