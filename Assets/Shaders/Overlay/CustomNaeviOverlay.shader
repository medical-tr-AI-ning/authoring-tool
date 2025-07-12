Shader "Custom/NaeviOverlay"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _MaskTex ("Mask Texture", 2D) = "white" {}
        _Threshold ("Threshold", Float) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 200

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            // User-defined properties
            sampler2D _MainTex;
            sampler2D _MaskTex;
            float _Threshold;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Get main texture color
                fixed4 mainColor = tex2D(_MainTex, i.uv);

                // Get mask value (grayscale)
                float maskValue = tex2D(_MaskTex, i.uv).r;

                // Only render parts where the mask value is above the threshold
                if (maskValue < _Threshold)
                {
                    return mainColor; // Fully opaque
                }
                else
                {
                    // Return transparent color, ensuring RGB is not pre-multiplied by Alpha
                    return fixed4(0, 0, 0, 0); // Fully transparent
                }
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
