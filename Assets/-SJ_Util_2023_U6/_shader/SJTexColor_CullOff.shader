Shader "Custom/SJTexColor_CullOff" 
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
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 200
		
		Pass 
		{
			Lighting Off
			Cull Off
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			//Blend SrcAlpha One
			
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
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 100
	
		Pass 
		{
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
