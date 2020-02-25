// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/CardiacSkin" {
Properties {
    _MainTex ("Texture", 2D) = "white" { }
    _PColor ("Peak Color", Color) = (1.0,0.0,0.0,0.0)
    _Param("parameter", float) = 0.5
    _TLower ("Lower Threashhold", Vector) = (0.74, 0.12, 0.0)
    _TUpper ("Upper Threashhold", Vector) = (0.02, 1.0, 1.0)

}
SubShader {
    Pass {

CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

sampler2D _MainTex;
float4 _PColor;
float _Param;
float4 _TLower;
float4 _TUpper;

struct v2f {
    float4  pos : SV_POSITION;
    float2  uv : TEXCOORD0;
};

float4 _MainTex_ST;

float3 HUEtoRGB(in float H)
{
	float R = abs(H * 6 - 3) - 1;
	float G = 2 - abs(H * 6 - 2);
	float B = 2 - abs(H * 6 - 4);
	return saturate(float3(R,G,B));
}
		
float3 HSVtoRGB(in float3 HSV)
{
	float3 RGB = HUEtoRGB(HSV.x);
	return ((RGB - 1) * HSV.y + 1) * HSV.z;
}
		
float Epsilon = 1e-10;

float3 RGBtoHCV(in float3 RGB)
{
	float4 P = (RGB.g < RGB.b) ? float4(RGB.bg, -1.0, 2.0/3.0) : float4(RGB.gb, 0.0, -1.0/3.0);
	float4 Q = (RGB.r < P.x) ? float4(P.xyw, RGB.r) : float4(RGB.r, P.yzx);
	float C = Q.x - min(Q.w, Q.y);
	float H = abs((Q.w - Q.y) / (6 * C + Epsilon) + Q.z);
	return float3(H, C, Q.x);
}

float3 RGBtoHSV(in float3 RGB)
{
	float3 HCV = RGBtoHCV(RGB);
	float S = HCV.y / (HCV.z + Epsilon);
	return float3(HCV.x, S, HCV.z);
}


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
//    texcol.r -= 0.5;
	bool skincolor = false;
    
    float3 hsbcol = RGBtoHSV(float3(texcol.r,texcol.g,texcol.b));
    
    if((hsbcol.x > _TLower.x || hsbcol.x < _TUpper.x) &&
       (hsbcol.y > _TLower.y && hsbcol.y < _TUpper.y) && 
       (hsbcol.z > _TLower.z && hsbcol.z < _TUpper.z) ){
 	
    	//if(_TUpper.x > _TLower.x){
    		//if( hsbcol.x > _TLower.x && hsbcol.x < _TUpper.x)
    		//{
    		//	skincolor = true;
    		//}
	    //}else{
    		//if(hsbcol.x > _TLower.x || hsbcol.x < _TUpper.x){
	    	//	skincolor = true;
    		//}
    	//}
    	texcol.r = _PColor.r *(_Param) + texcol.r * (1.0 - _Param);
		texcol.g = _PColor.g *(_Param) + texcol.g * (1.0 - _Param);
		texcol.b = _PColor.b *(_Param) + texcol.b * (1.0 - _Param);
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