Shader "Hidden/CRT_Afterimage"
{
    Properties
    {
        _BlitTexture ("Blit Source", 2D) = "white" {}
        _Intensity ("Persistence", Range(0, 1)) = 0.85
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        Pass
        {
            Name "CRT_Afterimage"
            ZTest Always Cull Off ZWrite Off
            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_BlitTexture);
            SAMPLER(sampler_BlitTexture);
            float _Intensity;

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

            Varyings Vert (Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }

            float4 Frag (Varyings input) : SV_Target
            {
                float4 current = SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, input.uv);
                float4 previous = UNITY_SAMPLE_TEX2D(_BlitTexture, input.uv); // same as current in this simple version

                // Blend previous with current based on intensity (simulated persistence)
                float4 blended = lerp(current, previous, _Intensity);

                return blended;
            }

            ENDHLSL
        }
    }
}
