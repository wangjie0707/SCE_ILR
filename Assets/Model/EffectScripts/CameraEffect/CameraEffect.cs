using UnityEngine;

//水波纹
//屏幕后处理特效一般都需要绑定在摄像机上  
[RequireComponent(typeof(Camera))]
public class CameraEffect : MonoBehaviour
{
    static private CameraEffect _instance;
    static public CameraEffect instance { get { return _instance; } }

    public Shader shader = null;
    private Material _material = null;
    public Material _Material
    {
        get
        {
            if (_material == null)
                _material = GenerateMaterial(shader);
            return _material;
        }
    }

    //根据shader创建用于屏幕特效的材质  
    protected Material GenerateMaterial(Shader shader)
    {
        if (shader == null)
            return null;
        //需要判断shader是否支持  
        if (shader.isSupported == false)
            return null;
        Material material = new Material(shader);
        material.hideFlags = HideFlags.DontSave;
        if (material)
            return material;
        return null;
    }

    /*
    //通过Range控制可以输入的参数的范围  
    [Range(0.0f, 3.0f)]
    public float brightness = 1.0f;//亮度  
    [Range(0.0f, 3.0f)]
    public float contrast = 1.0f;  //对比度  
    [Range(0.0f, 3.0f)]
    public float saturation = 1.0f;//饱和度  

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (_Material)
        {
            _material.SetFloat("_Brightness", brightness);
            _material.SetFloat("_Saturation", saturation);
            _material.SetFloat("_Contrast", contrast);
            //使用Material处理Texture，dest不一定是屏幕，后处理效果可以叠加的！  
            Graphics.Blit(src, dest, _material);
        }
        else
        {
            //直接绘制  
            Graphics.Blit(src, dest);
        }
    }
    */




    //距离系数  
    public float distanceFactor = 60.0f;
    //时间系数  
    public float timeFactor = -30.0f;
    //sin函数结果系数  
    public float totalFactor = 8.0f;

    //波纹宽度  
    public float waveWidth = 0.3f;
    //波纹扩散的速度  
    public float waveSpeed = 0.4f;

    private float waveStartTime;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (_Material)
        {
            //计算波纹移动的距离，根据enable到目前的时间*速度求解  
            float curWaveDistance = (Time.time - waveStartTime) * waveSpeed;
            _material.SetFloat("_curWaveDis", curWaveDistance);
            Graphics.Blit(src, dest, _material);
        }
        else
        {
            //直接绘制  
            //Graphics.Blit(src, dest);
            enabled = false;
        }
    }
    void SetAgr()
    {
        if (_Material)
        {
            _material.SetFloat("_distanceFactor", distanceFactor);
            _material.SetFloat("_timeFactor", timeFactor);
            _material.SetFloat("_totalFactor", totalFactor);
            _material.SetFloat("_waveWidth", waveWidth);
        }
    }

    void OnEnable()
    {
        if (_Material)
        {
            SetAgr();
            waveStartTime = Time.time;
            CancelInvoke();
            Invoke("DisEnable", 1f / waveSpeed);
        }
    }

    void DisEnable()
    {
        enabled = false;
    }
}