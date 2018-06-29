Shader "Custom/MyFoam" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_ScrollSpeed ("Scroll speed" , Range(0, 10) ) = 1
		_SubTex("Albedo2", 2D) = "white" {}

		//_Glossiness ("Smoothness", Range(0,1)) = 0.5
		//_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags {"RenderType"="Transparent" "Queue"="Transparent" }

//		Blend SrcAlpha OneMinusSrcAlpha	// Traditional transparency
//
//		Pass {
//			SetTexture [_MainTex] { combine texture }
//		}
		
		CGPROGRAM
		#pragma surface surf Lambert alpha:fade

		sampler2D _MainTex;
		sampler2D _SubTex;
		
		struct Input {
			float2 uv_MainTex;
			float2 uv_SubTex;
		};

		//half _Glossiness;
		//half _Metallic;
		fixed4 _Color;
		float _ScrollSpeed;

		void surf (Input IN, inout SurfaceOutput o) {
			//float t = _Time *_ScrollSpeed;
			//float2 uvNew = float2(IN.uv_MainTex.x, (IN.uv_MainTex.y+t));
			float4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = tex2D (_MainTex, IN.uv_MainTex). a * tex2D (_SubTex, IN.uv_SubTex).r ;
			//o.Alpha = c.b;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
