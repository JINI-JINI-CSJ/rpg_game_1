Shader "Custom/SJTexMulColor_Opaque" 
{
	Properties 
	{
		_Color   ("Multy Color " , Color)= (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "black" {}
	}
	
	// 1 pass
	SubShader 
	{
		Tags{ "Queue" = "Geometry" }
		LOD 100
	
		Pass 
		{
			Lighting Off

			SetTexture [_MainTex] 
			{ 
				constantColor [_Color]
				combine texture * constant
			} 
		}
	}
	

}
