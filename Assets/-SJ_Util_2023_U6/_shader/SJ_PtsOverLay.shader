// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Simplified Diffuse shader. Differences from regular Diffuse one:
// - no Main Color
// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

Shader "Custom/SJ_PtsOverLay" 
{
    Properties 
    {
        _MainTex ("Base (RGB)", 2D) = "black" {}
        _OverTex ("Over (RGB)", 2D) = "black" {}
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        _Center ("Center", Range(0,1)) = 0.5
    }

	SubShader
	{
		// No culling or depth
		Cull Off ZWrite On ZTest Always

		Pass 
        {  
		    CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _OverTex;
			float4 _OverTex_ST;

			fixed _Cutoff;
            fixed _Center;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, i.texcoord);
                fixed4 d = tex2D(_OverTex, i.texcoord);

                fixed dgray = saturate((d.r+d.g+d.b+c.r+c.g+c.b)/6); //

                fixed4 o;
                // if( dgray > _Center )
                // {
                //     //(2.0 * base * blend)
                //     //o.rgb = saturate(c*d*2);
                //     o.rgb = c+d;
                //     //o.rgb = 1;
                // }else{
                //     // (1.0 - 2.0 * (1.0 - base) * (1.0 - blend)
                //     //o.rgb = saturate(1-((1-c)*(1-d)*2));
                //     //o.rgb = 1-((1-c)*(1-d)*2);
                //     o.rgb = 0;
                // }

                o.rgb = lerp( saturate(1-((1-c)*(1-d)*2)),saturate(c*d*2),dgray);

                o.a = c.a;
				clip( o.a - _Cutoff);
				return o;
			}
		ENDCG
	    }

    // SubShader {
    //     Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector" = "True" }
    //     LOD 150

    // CGPROGRAM
    // #pragma surface surf Lambert 

    // sampler2D _MainTex;
    // sampler2D _OverTex;

    // struct Input {
    //     float2 uv_MainTex;
    //     float2 uv_OverTex;
    // };

    // void surf (Input IN, inout SurfaceOutput o) {

    //     fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
    //     fixed4 d = tex2D (_OverTex, IN.uv_OverTex);
    //     fixed dgray = saturate((d.r+d.g+d.b)/3); //
    //     if( dgray > 0.5 )
    //     {
    //         //(2.0 * base * blend)
    //         o.Albedo = saturate(c*d*2);
    //     }else{
    //         // (1.0 - 2.0 * (1.0 - base) * (1.0 - blend)
    //         o.Albedo = saturate(1-((1-c)*(1-d)*2));
    //     }
    //     o.Alpha = 0;

    //     // //o.Emission = lerp( saturate(1-((1-c)*(1-d)*2)),saturate(c*d*2),dgray);
    //     //o.Albedo = c.rgb;
    //     //o.Alpha = c.a;
    // }
    // ENDCG
    // }
    }
//Fallback "Custom/SJTexColor"
}
