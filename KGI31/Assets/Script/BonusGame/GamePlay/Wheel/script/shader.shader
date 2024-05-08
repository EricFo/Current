Shader "Custom/shader"
{
    Properties
    {
       
    }
    SubShader
    {

   Pass
        {

        CGPROGRAM
         #pragma enable_d3d11_debug_symbols
        #pragma target 3.0
        #pragma  vertex  vert;
        #pragma fragment frag;
        #include "UnityCG.cginc"
   

        struct a2v
        {
            float4 vertex : POSITION;
            float3 normal :NORMAL;
        };

        struct v2f
        {
            float4 pos : SV_POSITION;
            float3 worldNormal :TEXCOORD0;
            float3 worldPos : TEXCOORD1;
            float3 localPos: TEXTCOORD2;
        };

        v2f vert(a2v v)
        {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.worldNormal = UnityObjectToWorldNormal(v.normal);
            o.worldPos = mul(unity_ObjectToWorld, v.vertex);
            o.localPos = v.vertex;
            return o;
        }

        fixed4 frag(v2f i) :SV_Target
        {
            return float4(1,1,1, 1);
        }
        ENDCG
    
    }
}
    }