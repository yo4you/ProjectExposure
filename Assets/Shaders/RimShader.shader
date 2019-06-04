// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/RimShader"
{
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}

		_RimColor("Rim Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_RimPower("Rim Power", Range(0.01, 10)) = 3.0
	}
	SubShader{
		Pass{
			Tags {"RenderType" = "Opaque" }
			//"Queue" = "Geometry" "LightMode" = "ForwardBase" }
			// pass for ambient light and first light source

			Stencil {
				Ref 1
				Comp equal
				Pass Keep
			}
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct v2f {
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
					float3 normal : TEXCOORD1;
					float3 viewDir : TEXCOORD2;
				};

				sampler2D _MainTex;
				float4 _RimColor;
				float _RimPower;

				float4 _MainTex_ST;

				v2f vert(appdata_tan v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.normal = normalize(v.normal);
					o.viewDir = normalize(ObjSpaceViewDir(v.vertex));
					return o;
				}

				half4 frag(v2f i) : COLOR
				{
					//Rim Outline
					half Rim = 1 - saturate(dot(normalize(i.viewDir), i.normal));
					half4 RimOut = _RimColor * pow(Rim, _RimPower);
					return RimOut;
				}
				ENDCG
		}

	}
}
