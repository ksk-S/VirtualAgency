// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Pixelation" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
		_Redness("redness", float) = 0.0
        _Param("parameter", float) = 0.0
    }
    SubShader {
        Pass {
            ZTest Always Cull Off ZWrite Off
            Fog { Mode off }

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest

            #include "UnityCG.cginc"

            uniform sampler2D _MainTex;
            uniform sampler2D _SmallTex;
			uniform float _Param;
			uniform float _Redness;

            struct v2f {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };
		


            v2f vert(appdata_img v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = MultiplyUV (UNITY_MATRIX_TEXTURE0, v.texcoord);
                return o;
            }

            half4 frag (v2f i) : COLOR
            {
                fixed4 main = tex2D(_MainTex, i.uv);
                fixed4 small = tex2D(_SmallTex, i.uv);
                
                small.x =  (1 - _Redness) * small.x + _Redness;;
				small.y =  small.y * (1 - _Redness);
				small.z =  small.z * (1 - _Redness);
                
                
				//fixed4 result;
				//if( main.w > 0.5 )
				//{
				//	result = lerp(main, small, (main.w - 0.5) *2);
                //	main.w = 1;
				//}else{
				//	main.w = main.w * 2;
				//	result = main;
				//}
                
                fixed4 result = lerp(main, small, main.w);
                
                
                return result;
            }

            ENDCG
        }
    }
    Fallback off
}
