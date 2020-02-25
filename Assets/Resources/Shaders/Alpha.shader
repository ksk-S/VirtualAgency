// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/AlphaBlend" {
Properties {
    _MainTex ("Texture", 2D) = "white" { }
    _Param("parameter", float) = 1.0
  
}
SubShader {

	Tags { "Queue"="Transparent" "RenderType"="Transparent" }

    Pass {

		// ZWrite Off // don't write to depth buffer 
            // in order not to occlude other objects
 		ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha // use alpha blending


CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

sampler2D _MainTex;
float _Param;

struct v2f {
    float4  pos : SV_POSITION;
    float2  uv : TEXCOORD0;
};

float4 _MainTex_ST;

v2f vert (appdata_base v)
{
    v2f o;
    o.pos = UnityObjectToClipPos (v.vertex);
    o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
    return o;
}

half4 frag (v2f i) : COLOR
{
    half4 texcol = tex2D (_MainTex, i.uv);

	texcol.a = _Param;
 
    return texcol;
        
    //float3 rgbcol = HSVtoRGB(hsbcol);    
    //return half4(rgbcol.x, rgbcol.y, rgbcol.z, texcol.w);
}
ENDCG

    }
}
Fallback "VertexLit"
} 