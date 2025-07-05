using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class CRT_AfterimageEffectRendererFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public Material material;
        [Range(0f, 1f)] public float intensity = 0.85f;
    }

    public Settings settings = new Settings();

    private CRT_AfterimageRenderPass pass;

    public override void Create()
    {
        pass = new CRT_AfterimageRenderPass(settings);
        pass.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.material == null)
            return;

        renderer.EnqueuePass(pass);
    }

    class CRT_AfterimageRenderPass : ScriptableRenderPass
    {
        private Material material;
        private float intensity;

        static readonly int IntensityID = Shader.PropertyToID("_Intensity");

        public CRT_AfterimageRenderPass(Settings settings)
        {
            this.material = settings.material;
            this.intensity = settings.intensity;
        }

        class PassData
        {
            public Material material;
            public float intensity;
            public TextureHandle source;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            TextureHandle cameraColor = resourceData.cameraColor;

            var desc = renderGraph.GetTextureDesc(cameraColor);
            desc.name = "CRT_Afterimage_Temp";

            TextureHandle intermediateHandle = renderGraph.CreateTexture(desc);

            using (var builder = renderGraph.AddRasterRenderPass<PassData>("CRT Afterimage", out var passData))
            {
                passData.material = material;
                passData.intensity = intensity;
                passData.source = cameraColor;

                builder.UseTexture(cameraColor, AccessFlags.Read); // input
                builder.SetRenderAttachment(intermediateHandle, 0, AccessFlags.Write); // output

                builder.SetRenderFunc((PassData data, RasterGraphContext ctx) =>
                {
                    ctx.cmd.SetGlobalTexture("_BlitTexture", data.source);
                    data.material.SetFloat(IntensityID, data.intensity);
                    CoreUtils.DrawFullScreen(ctx.cmd, data.material, shaderPassId: 0);
                });

                resourceData.cameraColor = intermediateHandle;
            }
        }
    }
}
