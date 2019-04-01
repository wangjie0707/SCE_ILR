
//屏幕边缘黑 中心空 突出人物时用
namespace Colorful
{
	using UnityEngine;
    
	[ExecuteInEditMode]
	public class FastVignette : BaseEffect
	{
		public enum ColorMode
		{
			Classic,
			Desaturate,
			Colored
		}

		[Tooltip("Vignette type.")]
		public ColorMode Mode = ColorMode.Classic;

		[ColorUsage(false), Tooltip("The color to use in the vignette area.")]
		public Color Color = Color.red;

		[Tooltip("Center point.")]
		public Vector2 Center = new Vector2(0.5f, 0.5f);

		[Range(-100f, 100f), Tooltip("Smoothness of the vignette effect.")]
		public float Sharpness = 10f;

		[Range(0f, 100f), Tooltip("Amount of vignetting on screen.")]
		public float Darkness = 30f;

		protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			Material.SetVector("_Params", new Vector4(Center.x, Center.y, Sharpness * 0.01f, Darkness * 0.02f));
			Material.SetColor("_Color", Color);
			Graphics.Blit(source, destination, Material, (int)Mode);
		}
	}
}
