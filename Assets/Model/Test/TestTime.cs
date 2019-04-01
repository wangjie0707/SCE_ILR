//===================================================
//
//===================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI;
using Myth;

public class TestTime : MonoBehaviour
{
    TimeAction action;
    // Use this for initialization
    void Start()
    {
        GameEntry.Pool.SetClassObjectResideCount<TimeAction>(3);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            //创建定时器
             action = GameEntry.Time.CreateTimeAction();
            Debug.Log("创建定时器");
            action.Init("ABC", 10, 5, 8, () =>
             {
                 Debug.Log("开始运行定时器");
             }, (int loop) =>
             {
                //Debug.Log("定时器运行中 剩余次数=" + loop);
            }, () =>
            {
                Debug.Log("定时器运行完毕");
            }).Run();
        }

        if (Input.GetKeyUp(KeyCode.B))
        {
            action.Pause();
        }

        if (Input.GetKeyUp(KeyCode.C))
        {
            action.RePause();
        }
    }
}
