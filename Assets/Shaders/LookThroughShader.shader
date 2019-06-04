Shader "Unlit/LookThroughShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader{
			Tags { "RenderType" = "Transparent" }
			Stencil {
				Ref 1
				Comp always
				Pass replace
			}

			CGPROGRAM
			#pragma surface surf Lambert alpha

			struct Input {
				fixed3 Albedo;
			};

			void surf(Input IN, inout SurfaceOutput o) {
				o.Albedo = fixed3(1, 0, 1);
				o.Alpha = 1;
			}
			ENDCG
	}
	FallBack "Diffuse"
}
