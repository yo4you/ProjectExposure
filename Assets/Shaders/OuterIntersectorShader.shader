Shader "Unlit/OuterIntersectorShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" "Queue" = "Geometry+2"}
			LOD 100

			Pass
			{
				Stencil
				{
					Ref 2
					Comp Equal
					Pass keep
					ZFail decrWrap
				}
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag


				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
				};

				struct v2f
				{
					float4 vertex : SV_POSITION;
				};

				fixed4 _Color;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					return _Color;
				}
				ENDCG
			}


		}
}
