// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/ScreenSpaceDecals"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		LOD 100


		Pass
		{
			ZWrite Off
			Lighting Off
			Cull Back
			Blend SrcAlpha OneMinusSrcAlpha


			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 ScreenPos : TEXCOORD0;
				float4 recPos : TEXCOORD1;
				float4 position : SV_POSITION;
			};

			uniform sampler2D _CameraDepthTexture;
			uniform float4x4 _invMVP;
			float4 _MainTex_ST;
			sampler2D _MainTex;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.position = UnityObjectToClipPos(v.vertex);
				o.recPos = UnityObjectToClipPos(v.vertex);
				o.ScreenPos = ComputeScreenPos(o.position);
				return o;
			}
			
			fixed4 frag (v2f i) : Color
			{
				fixed4 color = fixed4(1.0,0,0,0);
				half depth = tex2D(_CameraDepthTexture,i.ScreenPos.xy / i.ScreenPos.w).r;
				float4 prjPos = float4 (i.recPos.xy / i.recPos.w,depth,1.0);
				float4 objPos = mul(_invMVP,prjPos);
				objPos /= objPos.w;
				if (objPos.x < -0.5 || objPos.x > 0.5 || objPos.y < -0.5 || objPos.y > 0.5 || objPos.z < -0.5 || objPos.z > 0.5)
					clip(-1.0);
				color = tex2D(_MainTex,_MainTex_ST.xy*(objPos.xz + 0.5) + _MainTex_ST.zw);
				
				//return tex2D(_CameraDepthTexture, i.ScreenPos.xy / i.ScreenPos.w);
				return color;
			}
			ENDCG
		}
	}
}
