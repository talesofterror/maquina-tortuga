Shader "Custom/HumanSkinSSS"
{
    Properties
    {
        _BaseMap("Base Map", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (1,1,1,1)
        _BumpMap("Normal Map", 2D) = "bump" {}
        _Smoothness("Smoothness", Range(0.0, 1.0)) = 0.5
        
        [Header(Subsurface Scattering)]
        _SSSColor("SSS Color", Color) = (1, 0.2, 0.1, 1)
        _SSSThicknessMap("Thickness Map", 2D) = "white" {}
        _SSSScale("SSS Scale", Range(0, 5)) = 1.0
        _SSSDistortion("SSS Distortion", Range(0, 1)) = 0.1
        _SSSPower("SSS Power", Range(0.1, 10)) = 1.0
        _WrappedDiffuse("Wrapped Diffuse", Range(0, 1)) = 0.5
        _TriplanarBlendSharpness("Triplanar Blend Sharpness", Range(1, 64)) = 4.0
        [Toggle] _ReceiveShadows("Receive Shadows", Float) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" "Queue"="Geometry" }
        LOD 300

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float2 uv : TEXCOORD2;
                float3 viewDirWS : TEXCOORD3;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _BaseColor;
                float _Smoothness;
                float4 _SSSColor;
                float _SSSScale;
                float _SSSDistortion;
                float _SSSPower;
                float _WrappedDiffuse;
                float _TriplanarBlendSharpness;
                float _ReceiveShadows;
            CBUFFER_END

            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
            TEXTURE2D(_BumpMap); SAMPLER(sampler_BumpMap);
            TEXTURE2D(_SSSThicknessMap); SAMPLER(sampler_SSSThicknessMap);

            float4 TriplanarSample(Texture2D tex, SamplerState smp, float3 positionWS, float3 normalWS, float tileScale, float blendSharpness)
            {
                float3 blendWeights = pow(abs(normalWS), blendSharpness);
                blendWeights /= (blendWeights.x + blendWeights.y + blendWeights.z);

                float2 uvX = positionWS.zy * tileScale;
                float2 uvY = positionWS.xz * tileScale;
                float2 uvZ = positionWS.xy * tileScale;

                float4 colX = SAMPLE_TEXTURE2D(tex, smp, uvX);
                float4 colY = SAMPLE_TEXTURE2D(tex, smp, uvY);
                float4 colZ = SAMPLE_TEXTURE2D(tex, smp, uvZ);

                return colX * blendWeights.x + colY * blendWeights.y + colZ * blendWeights.z;
            }

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.viewDirWS = GetWorldSpaceViewDir(output.positionWS);
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                return output;
            }

            float3 CalculateSSS(Light light, float3 normalWS, float3 viewDirWS, float thickness, float3 sssColor)
            {
                float3 lightDir = light.direction;
                float shadowAtten = lerp(1.0, light.shadowAttenuation, _ReceiveShadows);
                float3 lightColor = light.color * light.distanceAttenuation * shadowAtten;

                // Translucency (Back Scattering)
                float3 halfDir = normalize(lightDir + normalWS * _SSSDistortion);
                float VdotH = pow(saturate(dot(viewDirWS, -halfDir)), _SSSPower);
                float3 translucency = VdotH * _SSSScale * sssColor * thickness * lightColor;

                // Wrapped Diffuse (Front Scattering)
                float NdotL = dot(normalWS, lightDir);
                float wrappedNdotL = saturate((NdotL + _WrappedDiffuse) / (1.0 + _WrappedDiffuse));
                float3 wrappedLighting = wrappedNdotL * lightColor * sssColor * 0.5; // Tint shadows with SSS color

                return translucency + wrappedLighting;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float3 viewDirWS = normalize(input.viewDirWS);
                float3 normalWS = normalize(input.normalWS);
                
                // Sample Textures (Triplanar)
                float tileScale = _BaseMap_ST.x;
                float4 baseMapSample = TriplanarSample(_BaseMap, sampler_BaseMap, input.positionWS, normalWS, tileScale, _TriplanarBlendSharpness);
                float3 albedo = baseMapSample.rgb * _BaseColor.rgb;
                float thickness = TriplanarSample(_SSSThicknessMap, sampler_SSSThicknessMap, input.positionWS, normalWS, tileScale, _TriplanarBlendSharpness).r;

                // Main Light
                Light mainLight = GetMainLight(TransformWorldToShadowCoord(input.positionWS));
                float mainShadowAtten = lerp(1.0, mainLight.shadowAttenuation, _ReceiveShadows);
                float3 mainLightColor = mainLight.color * mainLight.distanceAttenuation * mainShadowAtten;
                
                // Standard Diffuse
                float NdotL = saturate(dot(normalWS, mainLight.direction));
                float3 diffuse = albedo * mainLightColor * NdotL;

                // SSS
                float3 sss = CalculateSSS(mainLight, normalWS, viewDirWS, thickness, _SSSColor.rgb);

                // Additional Lights
                int pixelLightCount = GetAdditionalLightsCount();
                for (int i = 0; i < pixelLightCount; ++i)
                {
                    Light light = GetAdditionalLight(i, input.positionWS);
                    float NdotL_add = saturate(dot(normalWS, light.direction));
                    float addShadowAtten = lerp(1.0, light.shadowAttenuation, _ReceiveShadows);
                    float3 lightColor_add = light.color * light.distanceAttenuation * addShadowAtten;
                    
                    diffuse += albedo * lightColor_add * NdotL_add;
                    sss += CalculateSSS(light, normalWS, viewDirWS, thickness, _SSSColor.rgb);
                }

                // Combine
                float3 finalColor = diffuse + sss;
                
                // Simple Ambient
                finalColor += albedo * 0.1; // Fake ambient

                return half4(finalColor, 1.0);
            }
            ENDHLSL
        }
        
        // Shadow Caster Pass
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            ZWrite On
            ZTest LEqual
            ColorMask 0

            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float3 _LightDirection;

            Varyings ShadowPassVertex(Attributes input)
            {
                Varyings output;
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);

                output.positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));
                output.uv = input.uv;
                return output;
            }

            half4 ShadowPassFragment(Varyings input) : SV_Target
            {
                return 0;
            }
            ENDHLSL
        }
    }
}
