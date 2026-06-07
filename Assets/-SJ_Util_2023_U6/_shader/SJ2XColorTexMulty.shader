Shader "Custom/SJ2XColorTexMulty" 
{
	Properties 
	{ 
		_Value("Value", Range(0,100)) = 1
		//_Color   ("Main Color " , Color)= (0,0,0,0)
		_MainTex("Base (RGB)", 2D) = "black" {}
	}

	SubShader 
	{
		Tags{ "Queue" = "Transparent+700" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		Cull Off
		Lighting Off
		Fog{ Mode Off }

		BlendOp	Add
		Blend	DstColor SrcAlpha
		//Blend Zero One ,One Zero
		//Blend Zero One , Zero One
		//Blend DstAlpha SrcAlpha
		//Blend Zero One , SrcAlpha DstAlpha
		Pass
		{
			SetTexture[_MainTex]
			{
				//constantColor[_Value]
				constantColor([_Value],[_Value],[_Value],1.0)
				combine texture * constant
			}
			//Color[_Color]
			//SetTexture[_MainTex]{ combine texture }
		}
	} 
	

	

}
