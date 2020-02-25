// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Mask1" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_MaskTex ("Mask", 2D) = "white" {}
		_Param("parameter", float) = 1.0
	}
	SubShader {
		Pass {
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

sampler2D _MainTex;
sampler2D _MaskTex;
float _Param;

struct v2f {
    float4 pos : SV_POSITION;
    float2 uv1 : TEXCOORD0;
    float2 uv2 : TEXCOORD1;
};

float4 _MainTex_ST;
float4 _MaskTex_ST;

v2f vert (appdata_base v)
{
    v2f o;
    o.pos = UnityObjectToClipPos (v.vertex);
    o.uv1 = TRANSFORM_TEX (v.texcoord, _MainTex);
    o.uv2 = TRANSFORM_TEX (v.texcoord, _MaskTex);
    return o;
}

half4 frag (v2f i) : COLOR
{

 	half4 texcol = tex2D (_MainTex, i.uv1);
	half4 maskcol = tex2D (_MaskTex, i.uv2);
	
	texcol.x =  maskcol.x * (maskcol.a * _Param) + texcol.x * ( 1 - (maskcol.a * _Param));
	texcol.y =  maskcol.y * (maskcol.a * _Param) + texcol.y * ( 1 - (maskcol.a * _Param));
	texcol.z =  maskcol.z * (maskcol.a * _Param) + texcol.z * ( 1 - (maskcol.a * _Param));

//	texcol.a = 1.0f;
 
    return texcol;
}
			ENDCG
		}
	} 
	FallBack Off
}