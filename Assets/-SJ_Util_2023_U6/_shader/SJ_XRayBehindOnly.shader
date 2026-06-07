Shader "Custom/SJ_XRayBehindOnly"
{
Properties {
        _Color ("Color", Color) = (0, 1, 1, 0.5)
        _Bias ("Depth Bias", Float) = 0.0001
    }

    SubShader {
        Tags { "Queue"="Transparent+10" "RenderType"="Transparent" }
        Pass {
            ZTest Greater
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "UnityCG.cginc"

            fixed4 _Color;
            float _Bias;

            struct appdata {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float depth : DEPTH;
            };

            v2f vert(appdata v) {
                v2f o;
                float4 clipPos = UnityObjectToClipPos(v.vertex);
                o.pos = clipPos;
                o.depth = clipPos.z / clipPos.w + _Bias; // Apply bias
                return o;
            }

            fixed4 frag(v2f i, out float oDepth : SV_Depth) : SV_Target {
                oDepth = i.depth;
                return _Color;
            }
            ENDCG
        }
    }
}
