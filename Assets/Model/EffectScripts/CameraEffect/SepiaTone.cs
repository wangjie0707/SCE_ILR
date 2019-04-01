using System;
using UnityEngine;

//黑白特效
namespace UnityStandardAssets.ImageEffects
{

    [ExecuteInEditMode]
    public class SepiaTone : ImageEffectBase
	{
        // Called by camera to apply image effect
        void OnRenderImage (RenderTexture source, RenderTexture destination)
		{
            Graphics.Blit (source, destination, material);
        }
    }
}
