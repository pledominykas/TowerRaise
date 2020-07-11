Shader "Custom/GradientShader" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_ColorTop("Top Color", Color) = (1,1,1,1)
		_ColorBot("Bot Color", Color) = (1,1,1,1)
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 100

		CGPROGRAM
		#pragma surface surf Lambert noforwardadd vertex:vert

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float4 texcoord;
		};

		void vert(inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.texcoord = v.texcoord;
		}

		fixed4 _ColorTop;
		fixed4 _ColorBot;

		void surf(Input IN, inout SurfaceOutput o) {
			fixed4 c = lerp(_ColorBot, _ColorTop, IN.texcoord.y);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
		}
Fallback "Mobile/VertexLit"
}