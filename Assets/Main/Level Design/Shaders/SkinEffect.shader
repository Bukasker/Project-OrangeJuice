Shader "Hidden/SkinEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SkinTint ("Skin Tint Color", Color) = (1, 1, 1, 1) // Kolor sk�ry
        _SkinColor ("Skin Color", Color) = (1, 0.8, 0.6, 1) // Kolor sk�ry, na kt�rym dzia�a efekt
        _ColorTolerance ("Color Tolerance", Range(0, 1)) = 0.1 // Tolerancja koloru
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        ZTest Always

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
            fixed4 _SkinTint;
            fixed4 _SkinColor;
            float _ColorTolerance;

            // Funkcja do por�wnania koloru piksela z zadan� warto�ci�
            bool IsColorSimilar(fixed4 pixelColor, fixed4 targetColor, float tolerance)
            {
                // Obliczamy r�nic� mi�dzy kolorami
                float diff = distance(pixelColor.rgb, targetColor.rgb);
                return diff < tolerance; // Je�li r�nica jest mniejsza ni� tolerancja, kolory s� podobne
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // Sprawdzamy, czy kolor piksela jest podobny do koloru sk�ry
                if (IsColorSimilar(col, _SkinColor, _ColorTolerance))
                {
                    // Zastosuj SkinTint tylko na podobnych kolorach
                    col.rgb = lerp(col.rgb, col.rgb * _SkinTint.rgb, 1.0);
                }

                // Uwzgl�dnienie przezroczysto�ci
                col.a *= _SkinTint.a;

                return col;
            }
            ENDCG
        }
    }
}
