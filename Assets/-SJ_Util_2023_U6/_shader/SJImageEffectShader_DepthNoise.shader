Shader "Hidden/SJImageEffectShader_DepthNoise"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Start_Val ( "_Start_Val" , Float ) = 0.5
		_Length_Val ( "_Length_Val" , Float ) = 0.1
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float _Start_Val;
			float _Length_Val;

			fixed4 frag (v2f i) : SV_Target
			{
				//float sceneZ = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.vertex)).r);
				fixed4 col = tex2D(_MainTex, i.uv);

				//if( sceneZ > _Start_Val && sceneZ < _Start_Val + _Length_Val )
				//{
				//	col = fixed4(1,1,0,1);
				//}

				return col;
			}
			ENDCG
		}
	}
}
