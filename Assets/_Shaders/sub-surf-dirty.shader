Shader "Custom/Dirty SubSurface" {

	Properties{
		_MainTex("Main Texture", 2D) = "white" {}
		_MainTex_ST("Texture Tiling and Offset", Vector) = (1,1,0,0)
		_BumpMap("Normal Map", 2D) = "bump" {}
		_BumpMap_ST("Normal Map Tiling and Offset", Vector) = (1,1,0,0)
		_BumpScale("Normal Map Scale", Range(0.0, 2.0)) = 1.0
		_myColor ("Example Color", Color) = (1,1,1,1)
		_myEmission("Example Emission", Color) = (1,1,1,1)
		_Distortion("Distortion", Range(0,1)) = 0.2
		_Power("Power", Range(0.1, 10.0)) = 1.0
		_Scale("Scale", Range(0.1, 10.0)) = 1.0
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
		LOD 100

		Pass {
			Name "ForwardLit"
			Tags { "LightMode"="UniversalForward" }

			HLSLPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile_fragment _ _SHADOWS_SOFT

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl"

			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);
			TEXTURE2D(_BumpMap);
			SAMPLER(sampler_BumpMap);

			struct Attributes {
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				float2 uv : TEXCOORD0;
			};

			struct Varyings {
				float4 positionCS : SV_POSITION;
				float3 positionWS : TEXCOORD0;
				float3 normalWS : TEXCOORD1;
				float3 tangentWS : TEXCOORD2;
				float3 binormalWS : TEXCOORD3;
				float2 uv : TEXCOORD4;
			};

			CBUFFER_START(UnityPerMaterial)
				float4 _MainTex_ST;
				float4 _BumpMap_ST;
				float4 _myColor;
				float4 _myEmission;
				float _BumpScale;
				float _Distortion;
				float _Power;
				float _Scale;
			CBUFFER_END

			Varyings vert(Attributes input) {
				Varyings output;
				output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
				output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
				output.normalWS = TransformObjectToWorldNormal(input.normalOS);
				output.tangentWS = TransformObjectToWorldDir(input.tangentOS.xyz);
				output.binormalWS = cross(output.normalWS, output.tangentWS) * input.tangentOS.w;
				output.uv = input.uv;
				return output;
			}

			float4 frag(Varyings input) : SV_TARGET {
				float3 normal = normalize(input.normalWS);
				float3 tangent = normalize(input.tangentWS);
				float3 binormal = normalize(input.binormalWS);
				
				float3 viewDir = normalize(_WorldSpaceCameraPos - input.positionWS);

				Light mainLight = GetMainLight(TransformWorldToShadowCoord(input.positionWS));
				float3 lightDir = normalize(mainLight.direction);

				// Sample texture with tiling and offset
				float2 uv = input.uv * _MainTex_ST.xy + _MainTex_ST.zw;
				float4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);

				// Sample and decode normal map
				float2 bumpUv = input.uv * _BumpMap_ST.xy + _BumpMap_ST.zw;
				float4 bumpTexture = SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, bumpUv);
				float3 bumpNormal = UnpackNormal(bumpTexture);
				bumpNormal *= float3(_BumpScale, _BumpScale, 1.0);
				
				// Convert normal map from tangent space to world space using TBN matrix
				float3x3 TBN = float3x3(tangent, binormal, normal);
				normal = normalize(mul(bumpNormal, TBN));

				// Main diffuse with texture
				float NdotL = max(0.0, dot(normal, lightDir));
				float3 diffuse = _myColor.rgb * texColor.rgb * NdotL * mainLight.color;

				// View-dependent subsurface scattering
				// Backlight transmission - how much light comes through from behind
				float3 backNormal = -normal;
				float backLight = max(0.0, dot(lightDir, backNormal));
				
				// Viewing angle dependent transmission
				// The more we look through the surface toward the light, the more we see transmission
				float viewThroughLight = max(0.0, dot(viewDir, -lightDir));
				
				// Distortion for thicker/thinner areas
				float3 distortedNormal = normalize(normal + viewDir * _Distortion);
				float distortedBackLight = max(0.0, dot(lightDir, -distortedNormal));
				
				// Combined subsurface effect: backlight modulated by viewing angle and texture
				float3 transmittedLight = backLight * viewThroughLight * distortedBackLight;
				diffuse += _myColor.rgb * texColor.rgb * transmittedLight * _Power * _Scale * mainLight.color;

				// Emission
				float3 finalColor = diffuse + _myEmission.rgb;

				return float4(finalColor, 1.0);
			}

			ENDHLSL
		}
	}
	
	Fallback "Universal Render Pipeline/Lit"
}