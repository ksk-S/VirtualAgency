// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/FrameSplit" {
Properties {
    _MainTex ("Texture", 2D) = "white" { }
    _XRatio("xratio", float) = 1.0
    _YRatio("yratio", float) = 1.0
    _Width("width", float) = 0.01
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
float _XRatio;
float _YRatio;
float _Width;
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
	
	float _InvXRatio = 1 - _XRatio;
	float _InvYRatio = 1 - _YRatio;
	
	if ( wcoord.x > _XRatio || wcoord.x < _InvXRatio || wcoord.y > _YRatio || wcoord.y < _InvYRatio){
		texcol.a = 1.0;      
       
    }else{
	    texcol.a = 0;
    }
 
 	if(
 		(wcoord.x > _XRatio  && wcoord.x < _XRatio + _Width && wcoord.y < _YRatio + _Width && wcoord.y > _InvYRatio - _Width) || 
 		(wcoord.x > _InvXRatio - _Width && wcoord.x < _InvXRatio  && wcoord.y < _YRatio + _Width && wcoord.y > _InvYRatio  - _Width)  ||
 		(wcoord.y > _YRatio  && wcoord.y < _YRatio + _Width && wcoord.x < _XRatio && wcoord.x > _InvXRatio ) || 
 		(wcoord.y > _InvYRatio - _Width && wcoord.y < _InvYRatio  && wcoord.x < _XRatio && wcoord.x > _InvXRatio )  
 	){
 		texcol.x = 0;
 		texcol.y = 0;
		texcol.z = 0;
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