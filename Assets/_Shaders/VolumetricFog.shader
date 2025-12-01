Shader "Custom/VolumetricFog"
{
  Properties
  {
    _MaxDistance("Max Distance", float) = 100
    _StepSize("StepSize", Range(0.1, 20)) = 1
    _DensityMultiplier("Density Multiplier", Range(0, 10)) = 0.02
    _FogColor("Fog Color", color) = (0.3, 0.4, 0.88)
    _NoiseOffset("Noise Offset", float) = 0

    _FogNoise("Fog Noise", 3D) = "white" {}
    _DensityThreshhold("Density Threshhold", Range(0, 1)) = 0.1
    _NoiseTiling("Noise Tiling", float) = 1
  }

  SubShader
  {
    Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

    Pass
    {
      ZWrite Off
      ZTest Always
      Cull Back
      Blend SrcAlpha OneMinusSrcAlpha
      // Blend DstColor Zero
      // Blend One One

      HLSLPROGRAM

      #pragma vertex Vert
      #pragma fragment frag

      #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
      #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
      #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

      float _MaxDistance;
      float _StepSize;
      float _DensityMultiplier;
      float4 _FogColor;
      float _NoiseOffset;
      TEXTURE3D(_FogNoise);
      float _DensityThreshhold;
      float _NoiseTiling;

      float get_density (float3 worldPos)
      {
        float4 noise = _FogNoise.SampleLevel(sampler_TrilinearRepeat, worldPos * 0.01 * _NoiseTiling, 0);
        float density = dot(noise, noise);
        density = saturate(density - (_DensityThreshhold * 0.5)) * _DensityMultiplier;
        return density * 0.1;
      }

      half4 frag(Varyings IN) : SV_Target
      {

        float4 col = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, IN.texcoord);
        float depth = SampleSceneDepth(IN.texcoord);

        // // Skip fog for very far distances
        // if (depth >= 0.70)
        // discard;

        float3 worldPos = ComputeWorldSpacePosition(IN.texcoord, depth, UNITY_MATRIX_I_VP);

        float3 entryPoint = _WorldSpaceCameraPos;
        float3 viewDir = worldPos - _WorldSpaceCameraPos;
        float viewLength = length(viewDir);
        float3 rayDir = normalize(viewDir);

        float2 pixelCoords = IN.texcoord * _BlitTexture_TexelSize.zw;
        float distLimit = min(viewLength, _MaxDistance);
        float distTravelled = InterleavedGradientNoise(pixelCoords, (int)(_Time.y / max(HALF_EPS, unity_DeltaTime.x))) * _NoiseOffset;
        float transmittance = 1;

        while(distTravelled < distLimit) {
          float3 rayPos = entryPoint + rayDir * distTravelled;

          float density = get_density(rayPos);
          if (density > 0) transmittance *= exp(- density * _StepSize);
          distTravelled += _StepSize;
        }

        // return float4(1, 1, 1, saturate(transmittance));
        // return float4(_FogColor.rgb, saturate(transmittance));
        // return float4(_FogColor.rgb, transmittance);

        // float finalDensity = saturate(transmittance); // The total accumulated density / absorption
        // float3 fogContribution = _FogColor.rgb * finalDensity;
        // return float4(fogContribution, 1.0); // Return the final color contribution

        // pink and yellow gradient squares and depths
        // return float4(frac(worldPos), 1);

        return lerp(col, _FogColor, 1.0 - saturate(transmittance));
      }

      ENDHLSL
    }
  }
}

/*

Pass
{
  HLSLPROGRAM

  #pragma vertex Vert
  #pragma fragment frag

  #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
  #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
  #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

  half4 frag(Varyings IN) : SV_Target
  {
    float depth = SampleSceneDepth(IN.texcoord);
    float3 worldPos = ComputeWorldSpacePosition(IN.texcoord, depth, UNITY_MATRIX_I_VP);
    return float4(frac(worldPos), 1);
  }

  */