Shader "Custom/SJTexColor_Opaque" 
{
	Properties 
	{
		_MColor   ("Multy Color " , Color)= (1,1,1,1)
		_AColor   ("Add Color " , Color)= (0,0,0,0)
		_MainTex ("Base (RGB)", 2D) = "black" {}
	}
	
	// 2 pass
	SubShader 
	{
		Tags{ "RenderType" = "Geometry" }
		LOD 200
		
		Pass 
		{
			Lighting Off

			
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
	
	// 1 pass
	SubShader 
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 100
	
		Pass 
		{
			Lighting Off

			SetTexture [_MainTex] 
			{ 
				constantColor [_AColor]
				combine texture + constant
			} 
		}
	}
	

}
