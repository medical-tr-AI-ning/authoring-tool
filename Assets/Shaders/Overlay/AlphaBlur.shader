Shader "Custom/AlphaBlurMask"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurSize ("Blur Size", Float) = .00025
        _Height ("Max Height", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _BlurSize;
            float _Height;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            // Fragment Shader: Blurring only the Alpha channel
            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float2 blur = float2(_BlurSize, _BlurSize); // Blur size for both directions
                
                // Sampling the Alpha channel at various offsets around the pixel
                float alpha = tex2D(_MainTex, uv).a * 4.0; // Base pixel (center sample)

                alpha += tex2D(_MainTex, uv + float2(blur.x, 0.0)).a * 2.0;
                alpha += tex2D(_MainTex, uv - float2(blur.x, 0.0)).a * 2.0;
                alpha += tex2D(_MainTex, uv + float2(0.0, blur.y)).a * 2.0;
                alpha += tex2D(_MainTex, uv - float2(0.0, blur.y)).a * 2.0;

                alpha += tex2D(_MainTex, uv + float2(blur.x, blur.y)).a * 1.0;
                alpha += tex2D(_MainTex, uv - float2(blur.x, blur.y)).a * 1.0;
                alpha += tex2D(_MainTex, uv + float2(blur.x, -blur.y)).a * 1.0;
                alpha += tex2D(_MainTex, uv - float2(blur.x, -blur.y)).a * 1.0;

                // Normalize the alpha value
                alpha /= 16.0;
                alpha *= _Height;

                // Output as a grayscale value (black and white mask)
                return fixed4(alpha, alpha, alpha, 1); // RGB are the same, creating a grayscale
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
