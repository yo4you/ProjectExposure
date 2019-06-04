// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/EdgeShader"
{
	Properties
	{
		//S_MainTex ("Texture", 2D) = "white" {}
		_SpecColor("Specular Material Color", Color) = (1,1,1,1)
		_Color("Main Color", Color) = (1,1,1,1)
		_Shininess("Shininess", Float) = 10

	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue"="Geometry" "LightMode"= "ForwardBase" }
		LOD 100

		Pass
		{

			Stencil
			{
				Ref 2
				Comp always
				Pass replace

			}

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			uniform float4 _LightColor0;

			fixed4 _Color;
			fixed4 _SpecColor;
			float _Shininess;


			// make fog work
			struct appdata {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};
			struct v2f {
				float4 pos : SV_POSITION;
				float4 posWorld : TEXCOORD0;
				float3 normalDir : TEXCOORD1;
			};
			v2f vert(appdata v) {
				v2f o;

				float4x4 modelMatrix = unity_ObjectToWorld;
				float4x4 modelMatrixInverse = unity_WorldToObject;

				o.posWorld = mul(modelMatrix, v.vertex);
				o.normalDir = normalize(
					mul(float4(v.normal, 0.0), modelMatrixInverse).xyz);
				o.pos = UnityObjectToClipPos(v.vertex);

				return o;

				
			}
			float4 frag(v2f input) : COLOR {
					
				float3 normalDirection = normalize(input.normalDir);

				float3 viewDirection = normalize(_WorldSpaceCameraPos - input.posWorld.xyz);
				float3 lightDirection;
				float attenuation;


				if (0.0 == _WorldSpaceLightPos0.w) // directional light?
				{
					attenuation = 1.0; // no attenuation
					lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				}
				else
				{
					float3 vertexToLightSource = _WorldSpaceLightPos0.xyz - input.posWorld.xyz;
					float distance = length(vertexToLightSource);
					attenuation = 1.0; // linear attenuation 
					lightDirection = normalize(vertexToLightSource);
				}
			

				float3 ambientLighting = UNITY_LIGHTMODEL_AMBIENT.rgb * _Color.rgb;

				float3 diffuseReflection = attenuation * _LightColor0.rgb * _Color.rgb * max(0.0, dot(normalDirection, lightDirection));

				float3 specularReflection;
			
				if (dot(normalDirection, lightDirection) < 0.0)
				// light source on the wrong side?
				{
					specularReflection = float3(0.0, 0.0, 0.0);
					// no specular reflection
				}
				else // light source on the right side
				{
					 specularReflection = attenuation * _LightColor0.rgb
						* _SpecColor.rgb * pow(max(0.0, dot(
						reflect(-lightDirection, normalDirection),
						viewDirection)), _Shininess);
				}

				return float4(ambientLighting + diffuseReflection + specularReflection, 1.0);
			}
			ENDCG
		}
	}
}
