﻿Shader "Unlit/PortalTransitionShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		//_RingTex ("Texture", 2D) = "white" {}

		_SobelRenderTexture("Sobel Render Texture", 2D) = "white" {}
		_MainRenderTexture("Main Render Texture", 2D) = "white" {}
		_CollectablesRenderTexture("Collectables Texture", 2D) = "white" {}

		_ScreenUV_X("Screen UV - X", Float) = 0
		_ScreenUV_Y("Screen UV - Y", Float) = 0

		_SourcePos_X("Source Pos - X", Float) = 0
		_SourcePos_Y("Source Pos - Y", Float) = 0

		_Radius("Radius", Float) = 0

	}
	SubShader
	{
		Tags { "Queue" = "Overlay" "RenderType" = "Overlay" }
		LOD 100

		//Blend SrcAlpha One
		//Cull Off
		//ZWrite Off

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
			sampler2D _SobelRenderTexture;
			sampler2D _MainRenderTexture;
			sampler2D _CollectablesRenderTexture;
			float4 _MainTex_ST;
			float _ScreenUV_X;
			float _ScreenUV_Y;	
			float _SourcePos_X;
			float _SourcePos_Y;
			float _Radius;

			//float _AdditionalPositions[50];

			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);


				return o;
			}
			float CalculateFadeOff(v2f i, float PosX, float PosY, float radius)
			{
				float dx = length(PosX - i.vertex.x);
				float dy = length(PosY - i.vertex.y);
				float dist = (dx * dx + dy * dy) / radius;

				float diff = length(dist);
				return 1.0f - diff / 5;
			}
			
			float4 frag (v2f i) : SV_Target
			{
				float4 sobelColor = tex2D(_SobelRenderTexture, i.uv);// float2(_ScreenUV_X,_ScreenUV_Y));
				float4 mainColor = tex2D(_MainRenderTexture, i.uv);// float2(_ScreenUV_X, _ScreenUV_Y));
				float4 collectablesColor = tex2D(_CollectablesRenderTexture, i.uv);// float2(_ScreenUV_X, _ScreenUV_Y));


				float2 pointPos = float2(i.vertex.x, i.vertex.y) / float2(_ScreenUV_X , _ScreenUV_Y);
				float2 mouse = float2(_SourcePos_X, _SourcePos_Y) / float2(_ScreenUV_X, _ScreenUV_Y);

				float2 dist = mouse - pointPos;
				//float2 dist = float2(_SourcePos_X - i.vertex.x, _SourcePos_Y - i.vertex.y);
				dist.x *= (_ScreenUV_X / _ScreenUV_Y);
				//float dx = length((_SourcePos_X - i.vertex.x) * (_ScreenUV_X/_ScreenUV_Y));
				//float dy = length(_SourcePos_Y - i.vertex.y);
				//float dist = (dx * dx + dy * dy);

				float diff = length(dist);
				float fadeOff = 1.0f - diff / _Radius;
				
				float4 playerColor = lerp(float4(mainColor.r, mainColor.g, mainColor.b, 1.0f), float4(sobelColor.r, sobelColor.g, sobelColor.b, 1), fadeOff);
				float4 elementColor = lerp(float4(mainColor.r, mainColor.g, mainColor.b, 1.0f), float4(sobelColor.r, sobelColor.g, sobelColor.b, 1), CalculateFadeOff(i, 200, 200, 500));


				float4 res = (playerColor);// +elementColor)*0.5f;

				return res;
			}




			ENDCG
		}
	
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

			float4 _MainTex_ST;
			sampler2D _CollectablesRenderTexture;


			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				return o;
			}


			float4 frag(v2f i) : SV_Target
			{
				float4 collectablesColor = tex2D(_CollectablesRenderTexture, i.uv);

				if (collectablesColor.r == 0 && collectablesColor.g==0 && collectablesColor.b==0)
				{
					discard;
				}

				return collectablesColor;
			}




			ENDCG
		}
	
	}
}