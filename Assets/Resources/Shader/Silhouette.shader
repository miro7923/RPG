Shader "DK/Silhouette/Diffuse"
{
    Properties
    {
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        [Space]
        _SilhouetteColor ("Silhouette Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        // Pass 1 ----------------------------------------------------------------
        Zwrite On        
        Stencil
        {
            Ref 2            // 스텐실 버퍼에 Ref 2 기록.
            Pass Replace  // Stencil, Z Test 모두 성공한 부분에 2 기록.
        }

        CGPROGRAM
        #pragma surface surf Lambert
        #pragma target 3.0

        sampler2D _MainTex;
        fixed4 _Color;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }

        ENDCG
        // ------------------------------------------------------------------------

        // Pass 2 ----------------------------------------------------------------
        ZWrite Off
        ZTest Greater // 다른 물체에 가려진 부분에 단색 실루엣 렌더링.

        Stencil
        {
            Ref 2
            Comp NotEqual //다른 실루엣이 그려진 부분에 덮어쓰지 않기.
                                  // Pass 1에서 렌더링 성공한 부분에는 그리지 않도록 한다.
        }

        CGPROGRAM
        #pragma surface surf nolight alpha:fade noforwardadd nolightmap noambient novertexlights noshadow

        struct Input
        {
            float4 color : COLOR;
        };

        float4 _SilhouetteColor;

        void surf(Input IN, inout SurfaceOutput o)
        {
            o.Emission = _SilhouetteColor.rgb;
            o.Alpha = _SilhouetteColor.a;
        }

        float4 Lightingnolight(SurfaceOutput s, float3 lightDir, float atten)
        {
            return float4(s.Emission, s.Alpha);
        }
        ENDCG
        // ------------------------------------------------------------------------
    }
    FallBack "Diffuse"
}
