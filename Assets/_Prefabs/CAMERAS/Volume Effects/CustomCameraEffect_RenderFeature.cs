using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule; // Required for the new mode

public class CustomCameraEffect_RenderFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public Material material;
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    }

    public Settings settings = new Settings();
    private CustomPostProcessPass m_RenderPass;

    public override void Create()
    {
        m_RenderPass = new CustomPostProcessPass(settings.material);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.material == null) return;
        m_RenderPass.renderPassEvent = settings.renderPassEvent;
        renderer.EnqueuePass(m_RenderPass);
    }

    class CustomPostProcessPass : ScriptableRenderPass
    {
        private Material m_Material;

        // Container structure that holds resources required by the GPU timeline
        private class PassData
        {
            public Material material;
            public float intensity;
            public Color tintColor;
            public float distance;
            public TextureHandle sourceTexture;
        }

        public CustomPostProcessPass(Material material)
        {
            m_Material = material;
            
            // MANDATORY IN NEW URP: Tells the engine we will be intercepting and modifying the camera target
            requiresIntermediateTexture = true; 
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            // 1. Fetch Volume data safely on the CPU
            var stack = VolumeManager.instance.stack;
            var customVolume = stack.GetComponent<CustomPostProcessVolume>();
            if (customVolume == null || !customVolume.IsActive()) return;

            // 2. Grab resource links from the frame tracking buffers
            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

            TextureHandle cameraColorTarget = resourceData.activeColorTexture;
            if (!cameraColorTarget.IsValid() || m_Material == null) return;

            // 3. Create a Temporary Texture Descriptor based on the current camera
            RenderTextureDescriptor desc = cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0; // We only need color data for this blit
            
            // 4. Allocate the Temporary Texture inside the Render Graph
            TextureHandle tempTexture = UniversalRenderer.CreateRenderGraphTexture(renderGraph, desc, "CustomPostProcess_Temp", false);

            // ==========================================
            // PASS 1: Blit from Camera -> Temp (Apply Material)
            // ==========================================
            using (var builder = renderGraph.AddRasterRenderPass<PassData>("Custom Post Process - Apply", out var passData))
            {
                passData.material = m_Material;
                passData.intensity = customVolume.intensity.value;
                passData.tintColor = customVolume.tintColor.value;
                passData.distance = customVolume.distance.value;
                passData.sourceTexture = cameraColorTarget;

                builder.UseTexture(cameraColorTarget);          // Set Camera as Input
                builder.SetRenderAttachment(tempTexture, 0);    // Set Temp as Output

                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    data.material.SetFloat("_Intensity", data.intensity);
                    data.material.SetColor("_TintColor", data.tintColor);
                    data.material.SetFloat("_Distance", data.distance);
                    
                    // Execute material blit
                    Blitter.BlitTexture(context.cmd, data.sourceTexture, Vector2.one, data.material, 0);
                });
            }

            // ==========================================
            // PASS 2: Blit from Temp -> Camera (Resolve Back)
            // ==========================================
            using (var builder = renderGraph.AddRasterRenderPass<PassData>("Custom Post Process - Resolve", out var passData))
            {
                passData.sourceTexture = tempTexture;

                builder.UseTexture(tempTexture);                   // Set Temp as Input
                builder.SetRenderAttachment(cameraColorTarget, 0); // Set Camera as Output

                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    // Material-less Blit to simply copy the pixels back to the screen
                    Blitter.BlitTexture(context.cmd, data.sourceTexture, Vector2.one, 0.0f, false);
                });
            }
        }

        // Left completely blank, but prevents errors if Compatibility Mode is ever toggled back on
        [System.Obsolete]
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
        }
    }
}