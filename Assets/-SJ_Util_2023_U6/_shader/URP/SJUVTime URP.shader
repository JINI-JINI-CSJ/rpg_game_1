Shader "Custom/SJUVTime_URP"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _XVal ("XVal", Float) = 1
        _YVal ("YVal", Float) = 1
        _MColor ("Multy Color", Color) = (1,1,1,1)
    }
    
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _XVal;
            float _YVal;
            float4 _MColor;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // UV 시간 애니메이션 적용
                float2 animatedUV = IN.uv + float2(_Time.y * _XVal, _Time.y * _YVal);
                
                // 텍스처 샘플링 및 컬러 곱하기
                half4 texColor = tex2D(_MainTex, animatedUV) * _MColor;
                
                return texColor;
            }
            ENDHLSL
        }
    }
}
