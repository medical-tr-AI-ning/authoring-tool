Shader "Custom/MelanomaShader"
{
    Properties
    {
        _Melanoma ("Melanoma Texture", 2D) = "white" {} // Input-Textur
        _Segmentation_Mask ("Segmentation Mask", 2D) = "white" {} // Segmentierungsmaske
        _BlurredRectangle ("Blurred Rectangle Texture", 2D) = "white" {} // Statische BlurredRectangle-Textur
        _Scale ("Scale", Float) = 1.0 // Skalierung der Textur
        _Brightness ("Brightness", Float) = 0.0 // Helligkeit
        _Contrast ("Contrast", Float) = 1.0 // Kontrast
        _Tint ("Tint Color", Color) = (1,1,1,1) // Einfärbung
        _Spread ("Spread", Float) = 1.0
        _Blur ("Blur", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

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
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            // Texture Samplers
            sampler2D _Melanoma;
            sampler2D _Segmentation_Mask;
            sampler2D _BlurredRectangle;

            float _Scale;
            float _Brightness;
            float _Contrast;
            float4 _Tint;
            float _Spread;
            float _Blur;

            // Vorarbeit: Tiling und Offset Berechnung
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                // Wir wollen die Textur auf die Mitte skalieren, ohne Kacheln.
                // Statt _Scale verwenden wir 1.0 / _Scale, um das umgekehrte Verhalten zu bekommen
                float scaleFactor = 1.0 / _Scale;  // Umkehrung der Skalierung
                float offsetFactor = (1.0 - scaleFactor) / 2.0;

                // Berechne die UVs so, dass die Texturen immer nur einmal erscheinen, und skaliere korrekt
                o.uv = v.uv * scaleFactor + float2(offsetFactor, offsetFactor);
                
                return o;
            }

            void Unity_Contrast_float(float3 In, float Contrast, out float3 Out)
            {
                float midpoint = pow(0.5, 2.2);
                Out = (In - midpoint) * Contrast + midpoint;
            }

            void Unity_Blend_Overlay_float4(float4 Base, float4 Blend, float Opacity, out float4 Out)
            {
                float4 result1 = 1.0 - 2.0 * (1.0 - Base) * (1.0 - Blend);
                float4 result2 = 2.0 * Base * Blend;
                float4 zeroOrOne = step(Base, 0.5);
                Out = result2 * zeroOrOne + (1 - zeroOrOne) * result1;
                Out = lerp(Base, Out, Opacity);
            }

            float4 frag (v2f i) : SV_Target
            {
                // Clamp UVs for all texture samplers to prevent tiling
                float2 clampedUV = clamp(i.uv, 0.0, 1.0);

                // BaseColor-Teil
                // 1. Melanoma-Textur samplen mit den skalierten und geklammerten UVs
                float4 melanomaColor = tex2D(_Melanoma, clampedUV);

                // 2. Brightness addieren
                melanomaColor.rgb += _Brightness;

                // 3. Contrast anwenden (nur auf RGB, Alpha bleibt unverändert)
                Unity_Contrast_float(melanomaColor.rgb, _Contrast, melanomaColor.rgb);

                // 4. Blend-Node: Mit Tint-Farbe mischen
                float4 blendedColor;
                Unity_Blend_Overlay_float4(melanomaColor, _Tint, 0.2, blendedColor);
                // Alpha-Teil
                // 1. Samplen der Segmentation Mask mit den gleichen UV-Koordinaten
                float4 segmentationMask = tex2D(_Segmentation_Mask, clampedUV);

                // 2. Samplen der Blurred Rectangle-Textur mit den gleichen UV-Koordinaten
                float4 blurredRectangle = tex2D(_BlurredRectangle, clampedUV);

                // 3. Multiplizieren der beiden Alpha-Werte
                float finalAlpha = segmentationMask.a * blurredRectangle.r;

                // Rückgabe: BaseColor mit berechnetem Alpha-Wert
                return float4(blendedColor.rgb, finalAlpha);
            }

            ENDCG
        }
    }

    FallBack "Diffuse"
}
