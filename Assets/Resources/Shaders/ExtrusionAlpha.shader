Shader "Example/Bumped Specular Alpha Extrusion" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	_Shininess ("Shininess", Range (0.03, 1)) = 0.078125
	_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_Amount ("Extrusion Amount", Range(-0.1,0.1)) = 0.5
	_Alpha("Transparency", Range(0,1)) = 1
}
SubShader { 
	Pass
	{
		Zwrite On
		ColorMask 0
		Lighting OFF
	}

	Tags{ "Queue" = "Transparent"
          "RenderType" = "Transparent"}
	LOD 400

	Cull Back
	
CGPROGRAM
#pragma surface surf BlinnPhong vertex:vert alpha

sampler2D _MainTex;
sampler2D _BumpMap;
fixed4 _Color;
half _Shininess;
float _Amount;
float _Alpha;

struct Input {
	float2 uv_MainTex;
	float2 uv_BumpMap;
};

 
void vert (inout appdata_full v) {
     v.vertex.xyz += v.normal * _Amount;
}

/*
half4 frag(v2f i) : COLOR
{
	half4 texcol = tex2D(_MainTex, i.uv);

	texcol.a = _Alpha;

	return texcol;

}
*/

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = tex.rgb * _Color.rgb;
	o.Gloss = tex.a;
	o.Alpha = _Alpha; //tex.a * _Color.a;
	o.Specular = _Shininess;
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
}
ENDCG
}

FallBack "Specular"
}