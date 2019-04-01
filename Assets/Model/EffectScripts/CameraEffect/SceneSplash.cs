using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

//场景由暗变亮
public class SceneSplash : ImageEffectBase {
	
	//Speed and amplitude of splash
	private float speed = 0.7f;
	
	private float _splashStep = 0.0f;
	
	void OnEnable()
	{
		_splashStep = 0.0f;
        Invoke("DisEnable", 2f);
    }

    void DisEnable()
    {
        enabled = false;
    }

    //Called by camera to apply image effect
    void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
            _splashStep += Time.deltaTime * speed;
		
		float splashValue = Mathf.Abs(Mathf.Sin( _splashStep));
		
		base.material.SetFloat ("_SplashStep", splashValue);
		Graphics.Blit (source, destination, material);
	}
	
	public float GetSplashValue()
	{
		return  Mathf.Abs(Mathf.Sin( _splashStep)); 
	}
}
