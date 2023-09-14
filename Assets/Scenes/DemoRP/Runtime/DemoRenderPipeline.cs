using UnityEngine;
using UnityEngine.Rendering;

namespace Scenes.DemoRP.Runtime
{
    public class DemoRenderPipeline : RenderPipeline
    {
        private static readonly ShaderTagId ShaderTagId = new("DemoRPForward");
        private static readonly int
            DirLightColorId = Shader.PropertyToID("_DirectionalLightColor"),
            DirLightDirectionId = Shader.PropertyToID("_DirectionalLightDirection");
        
        private static readonly ShaderTagId DepthOnlyShaderTagId = new("DemoRPDepthOnly");
        private static readonly int
            DepthPrePassBufferId = Shader.PropertyToID("_DepthPrePassBuffer");

        private ScriptableRenderContext _context;
        private CullingResults _cullingResults;

        public DemoRenderPipeline() => GraphicsSettings.useScriptableRenderPipelineBatching = true;

        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            _context = context;

            foreach (Camera camera in cameras)
            {
                RenderCamera(camera);
            }
        }

        private void RenderCamera(Camera camera)
        {
            if (!Cull(camera))
            {
                return;
            }

            CommandBuffer cmd = CommandBufferPool.Get();
            string cameraName = camera.name;
            cmd.BeginSample(cameraName);
            ExecuteAndClearCmd(cmd);

            _context.SetupCameraProperties(camera);

            SetupLighting(cmd);

            DepthPrePass(camera, cmd);

            cmd.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
            ClearRenderTarget(camera, cmd);
            ExecuteAndClearCmd(cmd);

            DrawVisibleGeometry(camera);

            cmd.ReleaseTemporaryRT(DepthPrePassBufferId);
            cmd.EndSample(cameraName);
            ExecuteAndClearCmd(cmd);

            _context.Submit();
        }

        private void DepthPrePass(Camera camera, CommandBuffer cmd)
        {
            const string sampleName = "DepthPrePass";

            var sortingSettings = new SortingSettings(camera)
            {
                criteria = SortingCriteria.CommonOpaque,
            };
            var drawingSettings = new DrawingSettings(DepthOnlyShaderTagId, sortingSettings);
            var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);

            var desc =
                new RenderTextureDescriptor(camera.pixelWidth, camera.pixelHeight, RenderTextureFormat.Depth, 32);
            cmd.BeginSample(sampleName);
            cmd.GetTemporaryRT(DepthPrePassBufferId, desc);
            cmd.SetRenderTarget(DepthPrePassBufferId);
            cmd.ClearRenderTarget(true, false, Color.clear);
            ExecuteAndClearCmd(cmd);

            _context.DrawRenderers(
                _cullingResults,
                ref drawingSettings,
                ref filteringSettings
            );

            cmd.EndSample(sampleName);
            ExecuteAndClearCmd(cmd);
        }

        private static void SetupLighting(CommandBuffer cmd)
        {
            Light light = RenderSettings.sun;
            cmd.SetGlobalVector(DirLightColorId, light.color.linear * light.intensity);
            cmd.SetGlobalVector(DirLightDirectionId, -light.transform.forward);
        }

        private bool Cull(Camera camera)
        {
            if (!camera.TryGetCullingParameters(out ScriptableCullingParameters parameters))
            {
                return false;
            }

            _cullingResults = _context.Cull(ref parameters);
            return true;
        }

        private void DrawVisibleGeometry(Camera camera)
        {
            var sortingSettings = new SortingSettings(camera)
            {
                criteria = SortingCriteria.CommonOpaque,
            };
            var drawingSettings = new DrawingSettings(ShaderTagId, sortingSettings);
            var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);

            _context.DrawRenderers(
                _cullingResults,
                ref drawingSettings,
                ref filteringSettings
            );

            _context.DrawSkybox(camera);
        }

        private static void ClearRenderTarget(Camera camera, CommandBuffer cmd)
        {
            CameraClearFlags cameraClearFlags = camera.clearFlags;
            bool clearDepth = cameraClearFlags <= CameraClearFlags.Depth;
            bool clearColor;
            Color backgroundColor;

            if (camera.cameraType == CameraType.Preview)
            {
                clearColor = true;
                backgroundColor = Color.black;
                backgroundColor.r = backgroundColor.g = backgroundColor.b = 0.25f;
            }
            else
            {
                clearColor = cameraClearFlags == CameraClearFlags.Color || camera.cameraType != CameraType.Game;
                backgroundColor = clearColor ? camera.backgroundColor.linear : Color.clear;
            }

            cmd.ClearRenderTarget(clearDepth, clearColor, backgroundColor);
        }

        private void ExecuteAndClearCmd(CommandBuffer cmd)
        {
            _context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
        }
    }
}