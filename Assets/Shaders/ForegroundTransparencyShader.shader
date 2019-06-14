// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/ForegroundShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		//_RingTex ("Texture", 2D) = "white" {}

		_ForegroundRenderTexture("Foreground Render Texture", 2D) = "white" {}

		_ScreenUV_X("Screen UV - X", Float) = 0
		_ScreenUV_Y("Screen UV - Y", Float) = 0

		_SourcePos_X("Source Pos - X", Float) = 0
		_SourcePos_Y("Source Pos - Y", Float) = 0

		_FaderOffset("Fader Vertical Offset", Range(0,1)) = 0
		_MaximumOpacity("Maximum Opacity", Range(0,1)) = 0



	}
	SubShader
	{

		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 100
		//Cull Off
		//ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			//sampler2D _RingTex;
			sampler2D _ForegroundRenderTexture;
			float4 _MainTex_ST;
			float _ScreenUV_X;
			float _ScreenUV_Y;
			float _SourcePos_X;
			float _SourcePos_Y;
			float _FaderOffset;
			float _MaximumOpacity;

			//float _AdditionalPositions[50];


			fixed4 OverlayBlend(fixed basePixel, fixed blendPixel)
			{
				if (basePixel < 0.5)
				{
					return 2.0f*(basePixel*blendPixel);
				}
				else
				{
					return (1.0f-2.0f*(1.0f-basePixel)*(1.0f-blendPixel));
				}
			}

			v2f vert(appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;
				return o;
			}

			fixed4 frag(v2f f) : SV_Target{
				fixed4 col = tex2D(_MainTex, f.uv);
				fixed4 foregroundColor = tex2D(_ForegroundRenderTexture, f.uv);

				fixed4 blended = col;
				blended.r = OverlayBlend(col.r, foregroundColor.r);
				blended.g = OverlayBlend(col.g, foregroundColor.g);
				blended.b = OverlayBlend(col.b, foregroundColor.b);
				
				if (f.vertex.y > _ScreenUV_Y *(1 - _FaderOffset) && foregroundColor.a > 0)
				{
					foregroundColor.a = _MaximumOpacity * ((f.vertex.y - _ScreenUV_Y * (1- _FaderOffset)) / (_ScreenUV_Y * (1 - _FaderOffset)));
				}

				//col *= foregroundColor;
				col = lerp(col, foregroundColor, foregroundColor.a);

				return col;
			}



			ENDCG
		}

	
	}
}
