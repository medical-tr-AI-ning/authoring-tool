Shader "Custom/OverlayTexture"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {} // Die große Textur (Haut)
        _OverlayTex ("Overlay Texture", 2D) = "white" {} // Die kleine Textur (Naevi)
        _OverlayPosX ("Overlay X Position", Float) = 0 // X-Position (UV)
        _OverlayPosY ("Overlay Y Position", Float) = 0 // Y-Position (UV)
        _OverlayScale ("Overlay Scale", Float) = 1.0 // Skalierung der Naevi-Textur (wenn gewünscht)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            // Blending-Modus für Transparenz festlegen
            // Hier wird sichergestellt, dass die Alpha-Werte des Overlays nicht auf die MainTex angewendet werden
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

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
                float4 pos : SV_POSITION;
            };

            // Texturen und Parameter
            sampler2D _MainTex;
            sampler2D _OverlayTex;
            float _OverlayPosX;
            float _OverlayPosY;
            float _OverlayScale;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                // Lese den Haupttextur-Farbwert
                float4 mainTexColor = tex2D(_MainTex, i.uv);

                // Berechne die UV-Koordinaten der Overlay-Textur relativ zur Haupttextur
                float2 overlayUV = (i.uv - float2(_OverlayPosX, _OverlayPosY)) / _OverlayScale;

                // Überprüfe, ob die Overlay-Koordinaten innerhalb der UV-Grenzen der Overlay-Textur liegen
                if (overlayUV.x >= 0.0 && overlayUV.x <= 1.0 && overlayUV.y >= 0.0 && overlayUV.y <= 1.0)
                {
                    // Hole den Farbwert der Overlay-Textur an den berechneten UV-Koordinaten
                    float4 overlayTexColor = tex2D(_OverlayTex, overlayUV);

                    // Quadriere den Alpha-Wert, um die Transparenz an den Rändern weicher zu machen
                    overlayTexColor.a = overlayTexColor.a * overlayTexColor.a * overlayTexColor.a;

                    // Mische die Overlay-Textur mit der Haupttextur basierend auf der angepassten Alpha-Komponente,
                    // Stelle jedoch sicher, dass der Alpha-Wert der Haupttextur immer bei 1 bleibt
                    float3 blendedRGB = lerp(mainTexColor.rgb, overlayTexColor.rgb, overlayTexColor.a);

                    // Gib die gemischten RGB-Werte zurück und setze den Alpha-Wert der Haupttextur fest auf 1
                    return float4(blendedRGB, 1.0);
                }

                // Gib den Original-Farbwert der Haupttextur zurück, wenn außerhalb der Overlay-Textur, Alpha bleibt 1
                return float4(mainTexColor.rgb, 1.0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
