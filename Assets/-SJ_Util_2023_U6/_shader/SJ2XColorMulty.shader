Shader "Custom/SJ2XColorMulty" 
{
	Properties 
	{
		_Color   ("Main Color" , Color)= (0,0,0,0)
	}

	SubShader 
	{
		Tags{ "Queue" = "Transparent+700" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		Cull Off
		Lighting Off
		Fog{ Mode Off }

		Blend SrcAlpha OneMinusSrcAlpha
		Pass
		{
			Color[_Color]
		}
	} 
	

	

}
