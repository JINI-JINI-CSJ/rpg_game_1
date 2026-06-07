Shader "Custom/SJColor_NoZBuff" 
{
	Properties 
	{
		_Color   ("Multy Color " , Color)= (1,1,1,1)
	}
	
	// 1 pass
	SubShader 
	{
		Tags{ "Queue" = "Transparent"  "RenderType" = "Transparent" "IgnoreProjector" = "True" }
		LOD 100
	
		ZWrite		Off
		Lighting	Off
		Cull		Off
		ZTest		Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			Color[_Color]
			// Material
			// {
			// 	Diffuse[_Color]
			// 	Ambient[_Color]
			// }
			//Lighting On
		}
	}
	

}
