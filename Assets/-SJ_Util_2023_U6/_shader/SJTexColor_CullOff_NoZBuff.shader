Shader "Custom/SJTexColor_CullOff_NoZBuff" 
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
		Tags{ "Queue" = "Transparent"  "RenderType" = "Transparent" "IgnoreProjector" = "True" }
		LOD 100
	
		Pass 
		{
			ZWrite		Off
			Lighting	Off
			Cull		Off
			ZTest		Always
			Blend SrcAlpha OneMinusSrcAlpha

			SetTexture [_MainTex] 
			{ 
				constantColor [_MColor]
				combine texture * constant
			} 

			SetTexture [_MainTex] 
			{ 
				constantColor [_AColor]
				combine previous + constant
			} 
		}
	}
	

}
