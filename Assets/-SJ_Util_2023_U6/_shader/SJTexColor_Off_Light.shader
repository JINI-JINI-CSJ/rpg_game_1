Shader "Custom/SJTexColor_Off_Light" 
{
	Properties 
	{
		//_Shininess("Shininess", Range(0.01, 1)) = 0.7
		_MColor   ("Multy Color " , Color)= (1,1,1,1)
		_AColor   ("Add Color " , Color)= (0,0,0,0)
		_MainTex ("Base (RGB)", 2D) = "black" {}

	}
	
	// 2 pass
	SubShader 
	{
		Tags{ "RenderType" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 200
		
		Pass 
		{
			Lighting Off
			Fog{ Mode Off }
			Blend SrcAlpha OneMinusSrcAlpha
		
			Material
			{
				Diffuse(1,1,1,1)
				Ambient(1,1,1,1)
				//Emission(1,1,1,1)
			}

			SetTexture[_MainTex]
			{
				combine texture * primary
			}

			SetTexture [_MainTex] 
			{ 
				constantColor [_MColor]
				combine previous * constant
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
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 100
	
		Pass 
		{
			Lighting Off
			Blend SrcAlpha OneMinusSrcAlpha
			SetTexture [_MainTex] 
			{ 
				constantColor [_AColor]
				combine texture + constant
			} 
		}
	}
	

}
