Shader "Custom/SJTexColor_CullOff_1" 
{
	Properties 
	{
		_MColor   ("Multy Color " , Color)= (1,1,1,1)
		_AColor   ("Add Color " , Color)= (0,0,0,0)
		_MainTex ("Base (RGB)", 2D) = "black" {}
	}
	
	// 1 pass
	SubShader 
	{
		Tags{ "Queue" = "Transparent"  "RenderType" = "Transparent" }
		LOD 100
	
		Pass 
		{
			ZWrite On
			Lighting Off
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha
			SetTexture [_MainTex] 
			{ 
				constantColor [_AColor]
				combine texture + constant
			} 
		}
	}
	

}
