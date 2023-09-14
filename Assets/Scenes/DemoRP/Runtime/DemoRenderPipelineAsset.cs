using UnityEngine;
using UnityEngine.Rendering;

namespace Scenes.DemoRP.Runtime
{
	[CreateAssetMenu]
	public class DemoRenderPipelineAsset : RenderPipelineAsset
	{
		protected override RenderPipeline CreatePipeline()
		{
			return new DemoRenderPipeline();
		}
	}
}