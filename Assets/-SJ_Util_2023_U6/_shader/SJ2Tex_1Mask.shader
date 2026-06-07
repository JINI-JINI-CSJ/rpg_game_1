Shader "Custom/SJ2Tex_1Mask" {
	Properties 
	{
		_MColor     ("Multy Color " , Color)= (1,1,1,1)
		_MainTex	("Base (RGB)", 2D)	= "white" {}
		_AddTex		("Add (RGB)", 2D)	= "black" {}
		_MaskTex	("Mask (RGB)", 2D)	= "white" {}
	}
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }

		Cull Off
		Lighting Off
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha

		LOD 200
		
		//CGPROGRAM
		//#pragma surface surf Lambert
		//sampler2D _MainTex;
		//sampler2D _AddTex;
		//sampler2D _MaskTex;
		//struct Input {
		//    float2 uv_MainTex;
		//};
		//void surf (Input IN, inout SurfaceOutput o) 
		//{
		//    half4 mc		= tex2D (_MainTex, IN.uv_MainTex);
		//    half4 ac		= tex2D (_AddTex, IN.uv_MainTex);
		//    half4 mask_c	= tex2D (_MaskTex, IN.uv_MainTex);
			
		//    //mc.rgb = lerp( mc.rgb , ac.rgb , ac.a );

		//    o.Albedo = mc.rgb;
		//    o.Alpha = mc.a * mask_c.a;

		//    //o.Albedo = 1.0f;
		//    //o.Alpha = 1.0f;
		//}
		//ENDCG

		Pass 
		{
			SetTexture [_MainTex]	
			{
				constantColor [_MColor]
				combine texture * constant
			}
			SetTexture [_AddTex]	{combine texture lerp (texture) previous}
			SetTexture [_MaskTex]	{combine previous , texture * previous}
		}


	} 
	FallBack "Diffuse"
}
