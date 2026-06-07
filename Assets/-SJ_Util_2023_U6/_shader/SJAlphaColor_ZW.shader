Shader "Custom/SJAlphaColor_ZW" 
{
	Properties 
	{
		_Color   ("Main Color " , Color)= (0,1,1,1)

	}

	SubShader 
	{
		Tags{ "Queue" = "Geometry" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		ZWrite On
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
