Shader "Custom/SJAlphaColor" 
{
	Properties 
	{
		_Color   ("Main Color " , Color)= (0,1,1,1)

	}

	SubShader 
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		Cull Off
		Lighting Off
		Fog{ Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			Material
			{
				Diffuse[_Color]
				Ambient[_Color]
			}

			// use fixed function per-vertex lighting
			Lighting On
		}
	} 
	

	

}
