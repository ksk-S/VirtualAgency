// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/HorizontalSplit" {
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

float4 _MainTex_ST;

struct v2f {
    float4  pos : SV_POSITION;
    float2  uv : TEXCOORD0;
    float4  scrPos:TEXCOORD2;
};

v2f vert (appdata_base v)
{
    v2f o;
    o.pos = UnityObjectToClipPos (v.vertex);
    o.scrPos = ComputeScreenPos(o.pos);
    o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
    return o;
}

half4 frag (v2f i) : COLOR
{
    half4 texcol = tex2D (_MainTex, i.uv);
	//float2 pos = i.uv;
	float2 wcoord = (i.scrPos.xy/i.scrPos.w);
	
	if (wcoord.y > _Param) {
		//clip(-1.0);
        texcol.a = 0;      
    }else{
	    texcol.a = 1.0;
    }
 
    return texcol;
        
    //float3 rgbcol = HSVtoRGB(hsbcol);    
    //return half4(rgbcol.x, rgbcol.y, rgbcol.z, texcol.w);
}
ENDCG

    }
}
Fallback "VertexLit"
} 