Shader "Custom/SceneSplash" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
	   
	   LOD 200
	
	  pass{
		ZTest Always Cull Off ZWrite Off Lighting Off Fog { Mode off }
		CGPROGRAM

		#pragma vertex vert_img
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest 
		#include "UnityCG.cginc"

		sampler2D _MainTex;
		fixed _SplashStep;
		

		fixed4 frag (v2f_img i) : COLOR {
		
			fixed4 originColor = tex2D (_MainTex, i.uv);
			
			fixed4 lerpColor = fixed4(0,0,0,1);

			
			fixed4 outColor = lerp(lerpColor, originColor, _SplashStep) * _SplashStep;
			outColor.a = originColor.a;
			
			return outColor;
		
		}
		ENDCG
	  }
	} 
	FallBack off
}
