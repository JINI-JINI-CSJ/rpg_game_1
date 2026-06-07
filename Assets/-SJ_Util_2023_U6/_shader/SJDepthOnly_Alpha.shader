Shader "Custom/SJDepthOnly_Alpha" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags {"Queue"="Transparent-1" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 200
		
		Pass 
		{
			ZWrite		On
			ColorMask	0
			AlphaTest Greater 0.5
            SetTexture [_MainTex] { combine texture }
		}
	} 
	FallBack "Diffuse"
}
