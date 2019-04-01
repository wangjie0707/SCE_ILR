using UnityEngine;
using System.Collections;

public class AniTranPosY : MonoBehaviour
{
    public float distance = 0.5f;
    public float randValue = 0.1f;
    public float freq = 1.0f;

    public float rotationSpeedX = 0;
    public float rotationSpeedY = 0;
    public float rotationSpeedZ = 0;

    private Vector3 chuShiPos;
    private float zuiDaPosY;
    private float zuiXiaoPosY;
    private bool IsUp = false;
    private float _randon = 0.0f;
    private Vector3 _pt = Vector3.zero;
    private Transform trs;
    //private  Vector3 rotationVector = Vector3(rotationSpeedX,rotationSpeedY,rotationSpeedZ);

    void Start()
    {
        trs = this.transform;
        distance = distance + Random.Range(0.0f, randValue);
        _randon = Random.value;
        if (_randon < 0.5)
        {
            IsUp = true;
        }
        else
        {
            IsUp = false;
        }
        chuShiPos = this.transform.localPosition;
        zuiDaPosY = chuShiPos.y + distance;
        zuiXiaoPosY = chuShiPos.y - distance;
    }

    void Update()
    {
        //transform.Rotate (rotationSpeedX * Time.deltaTime, 0, 0);
        //transform.Rotate (0, rotationSpeedY * Time.deltaTime, 0);
        //transform.Rotate (0, 0, rotationSpeedZ * Time.deltaTime);
        if (!IsUp)
        {
            //this.transform.Translate(Vector3.up * Time.deltaTime * Freq);
            _pt = trs.localPosition;
            _pt.y += Vector3.up.y * Time.deltaTime * freq;
            trs.localPosition = _pt;
            if (_pt.y >= zuiDaPosY)
            {
                IsUp = true;
            }
        }
        else
        {
            //this.transform.Translate(-Vector3.up * Time.deltaTime * Freq);
            _pt = trs.localPosition;
            _pt.y += -Vector3.up.y * Time.deltaTime * freq;
            trs.localPosition = _pt;
            if (_pt.y <= zuiXiaoPosY)
            {
                IsUp = false;
            }
        }
    }

}
