
Shader "Shader Graphs/DissolveGlitchShader"
{
    Properties
    {
        _NoiseTex("Noise Texture", 2D) = "white" {}
        _Threshold("Threshold", Range(0, 1)) = 0.5
        _EdgeColor("Edge Color", Color) = (0, 1, 1, 1)
        _EdgeWidth("Edge Width", Range(0, 1)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);
            float _Threshold;
            float4 _EdgeColor;
            float _EdgeWidth;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float noise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, IN.uv).r;

                float edge = smoothstep(_Threshold, _Threshold + _EdgeWidth, noise);
                float clipValue = step(noise, _Threshold);

                clip(clipValue - 0.01);

                float3 baseColor = float3(0.1, 0.1, 0.1);
                float3 edgeGlow = _EdgeColor.rgb * edge;

                return float4(baseColor + edgeGlow, 1);
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
