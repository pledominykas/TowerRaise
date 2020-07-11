Shader "Custom/CurrentLevelShader" {
	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_Color1("Color 1", Color) = (0,0,0,0)
		_Color2("Color 2", Color) = (0,0,0,0)
		_Frequency("Pulse frequency", Float) = 1
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 100

		CGPROGRAM
		#pragma surface surf Lambert noforwardadd

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		fixed4 _Color1;
		fixed4 _Color2;
		float _Frequency;
		void surf(Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb * lerp(_Color1, _Color2, (sin(_Time[1] * _Frequency) + 1)*0.5f);
			o.Alpha = c.a;
		}
		ENDCG
		}
		Fallback "Mobile/VertexLit"
}