Shader "Example/Bumped Specular Extrusion" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	_Shininess ("Shininess", Range (0.03, 1)) = 0.078125
	_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_Amount ("Extrusion Amount", Range(-1,1)) = 0.5
}
SubShader { 
	Tags { "RenderType"="Opaque" }
	LOD 400
	
	Cull Back
	
CGPROGRAM
#pragma surface surf BlinnPhong vertex:vert


sampler2D _MainTex;
sampler2D _BumpMap;
fixed4 _Color;
half _Shininess;
float _Amount;

struct Input {
	float2 uv_MainTex;
	float2 uv_BumpMap;
};

 
void vert (inout appdata_full v) {
     v.vertex.xyz += v.normal * _Amount;
}
 
void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = tex.rgb * _Color.rgb;
	o.Gloss = tex.a;
	o.Alpha = tex.a * _Color.a;
	o.Specular = _Shininess;
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
}
ENDCG
}

FallBack "Specular"
}