Shader "Custom/VolumetricFog"
{
  Properties
  {
    _MaxDistance("Max Distance", float) = 100
    _StepSize("StepSize", Range(0.1, 20)) = 1
    _DensityMultiplier("Density Multiplier", Range(0, 10)) = 0.02
    _FogColor("Fog Color", color) = (0.3, 0.4, 0.88)
  }

  SubShader
  {
    Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

    Pass
    {
      ZWrite Off
      ZTest Always
      Cull Back
      // Blend SrcAlpha OneMinusSrcAlpha
      // Blend DstColor Zero
      Blend One One

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

      float get_density ()
      {
        return _DensityMultiplier * 0.001;
      }

      half4 frag(Varyings IN) : SV_Target
      {
        
        float4 col = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, IN.texcoord);
        float depth = SampleSceneDepth(IN.texcoord);

        // Skip fog for very far distances
        if (depth >= 0.70)
        discard;
        
        float3 worldPos = ComputeWorldSpacePosition(IN.texcoord, depth, UNITY_MATRIX_I_VP);

        float3 entryPoint = _WorldSpaceCameraPos;
        float3 viewDir = worldPos - _WorldSpaceCameraPos;
        float viewLength = length(viewDir);
        float3 rayDir = normalize(viewDir);

        float distLimit = min(viewLength, _MaxDistance);
        float distTravelled = 0;
        float transmittance = 0;

        while(distTravelled < distLimit) {
          float density = get_density();
          if (density > 0) transmittance += density * _StepSize;
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

        return lerp(col, _FogColor, saturate(transmittance));
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