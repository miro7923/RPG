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
            Ref 2            // ���ٽ� ���ۿ� Ref 2 ���.
            Pass Replace  // Stencil, Z Test ��� ������ �κп� 2 ���.
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
        ZTest Greater // �ٸ� ��ü�� ������ �κп� �ܻ� �Ƿ翧 ������.

        Stencil
        {
            Ref 2
            Comp NotEqual //�ٸ� �Ƿ翧�� �׷��� �κп� ����� �ʱ�.
                                  // Pass 1���� ������ ������ �κп��� �׸��� �ʵ��� �Ѵ�.
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
