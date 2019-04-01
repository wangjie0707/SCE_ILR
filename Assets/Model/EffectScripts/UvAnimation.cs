using UnityEngine;
using System.Collections;

public class UvAnimation : MonoBehaviour
{
    public float xspeed = 0;
    public float yspeed = 0;
    private Material _material;
    public Material material
    {
        get
        {
            if (_material == null)
            {
                _material = GetComponent<Renderer>().materials[0];
            }
            return _material;
        }
    }

    private Vector2 v2;
    void OnEnable()
    {
        v2 = Vector2.zero;
    }
    void Update()
    {
        if (enabled)
        {
            v2.x += Time.fixedDeltaTime * xspeed;
            v2.y += Time.fixedDeltaTime * yspeed;
            material.mainTextureOffset = v2;
        }
    }


}
