Shader "Custom/SJTexColor_2Pass2Side" 
{
	Properties 
	{
		_MColor   ("Multy Color " , Color)= (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "black" {}
		_BackTex ("Base (RGB)", 2D) = "black" {}
	}
	

	SubShader 
	{
		Tags{ "RenderType" = "Geometry" }
		LOD 100
	
		Pass 
		{
			ZWrite On
			Lighting On
			Cull Back
			SetTexture [_MainTex] 
			{ 
				constantColor [_MColor]
				combine texture * constant
			}

		}

		Pass
		{
			ZWrite On
			Lighting On
			Cull Front
			SetTexture[_BackTex]
			{
				constantColor[_MColor]
				combine texture * constant
			}

		}

	}
	

}
