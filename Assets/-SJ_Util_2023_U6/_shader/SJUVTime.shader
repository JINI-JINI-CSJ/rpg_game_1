Shader "Custom/SJUVTime"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _XVal ("XVal" , Float) = 1
        _YVal ("YVal" , Float) = 1
        _MColor ("Multy Color " , Color)= (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue" = "Transparent"  "RenderType" = "Transparent" "IgnoreProjector" = "True" }
        LOD 100

        //Lighting Off
        //Blend SrcAlpha OneMinusSrcAlpha


            CGPROGRAM
            //#pragma surface surf BlinnPhong  alpha:auto  noforwardadd novertexlights 
            #pragma surface surf FUNC_Color  alpha:fade  noforwardadd  


            half4 LightingFUNC_Color (SurfaceOutput s, half3 lightDir, half atten) {
                // half NdotL = dot (s.Normal, lightDir);
                half4 c;
                c.rgb = s.Albedo;
                //c.rgb = half3( 0, 1, 0);
                c.a = s.Alpha;
                return c;
            }            

            sampler2D _MainTex;
            fixed _XVal;
            fixed _YVal;
            fixed4 _MColor;

            struct Input {
                float2 uv_MainTex;
            };


            void surf (Input IN, inout SurfaceOutput o) {
                fixed ux = IN.uv_MainTex.x + _Time.y * _XVal;
                fixed uy = IN.uv_MainTex.y + _Time.y * _YVal;

                fixed4 c = tex2D(_MainTex, float2(ux,uy)) * _MColor;
                o.Albedo = c.rgb;
                o.Alpha = c.a;
            }


            ENDCG
        
    }
}
