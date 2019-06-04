Shader "Custom/CameraViewsMixerShader" {
	Properties {


		_SobelRenderTexture("Sobel Render Texture", 2D) = "white" {}
		_MainRenderTexture("Main Render Texture", 2D) = "white" {}

		_ScreenUV_X("Screen UV - X", Float) = 0
		_ScreenUV_Y("Screen UV - Y", Float) = 0

		_SourcePos_X("Source Pos - X", Float) = 0
		_SourcePos_Y("Source Pos - Y", Float) = 0

		_Radius("Radius", Float) = 0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		sampler2D _SobelRenderTexture;
		sampler2D _MainRenderTexture;
		float _ScreenUV_X;
		float _ScreenUV_Y;
		float _SourcePos_X;
		float _SourcePos_Y;
		float _Radius;


		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			float4 sobelColor = tex2D(_SobelRenderTexture, IN.uv_MainTex);// float2(_ScreenUV_X,_ScreenUV_Y));
			float4 mainColor = tex2D(_MainRenderTexture, IN.uv_MainTex);// float2(_ScreenUV_X, _ScreenUV_Y));


			float dx = length(_SourcePos_X - IN.worldPos.x);
			float dy = length(_SourcePos_Y - IN.worldPos.y);
			float dist = (dx * dx + dy * dy)*_Radius;

			float diff = length(dist);
			float fadeOff = 1.0f - diff / 5;

			o.Albedo = lerp(float3(1, 0, 0), float3(0, 0, 1), dist);
			o.Alpha = 1.0f;
			//o.Albedo = lerp()

		}
		ENDCG
	}
	FallBack "Diffuse"
}
