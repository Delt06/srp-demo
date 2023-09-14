using UnityEngine;
using UnityEngine.Rendering;

namespace Scenes.DemoRP.Runtime
{
	public class DemoRenderPipeline : RenderPipeline
	{
		private static readonly ShaderTagId ShaderTagId = new("DemoRPForward");

		private ScriptableRenderContext _context;
		private CullingResults _cullingResults;

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

			cmd.SetRenderTarget(camera.targetTexture);
			ClearRenderTarget(camera, cmd);
			ExecuteAndClearCmd(cmd);

			DrawVisibleGeometry(camera);

			cmd.EndSample(cameraName);
			ExecuteAndClearCmd(cmd);

			_context.Submit();
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

		private void ExecuteAndClearCmd(CommandBuffer cmd)
		{
			_context.ExecuteCommandBuffer(cmd);
			cmd.Clear();
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
	}
}