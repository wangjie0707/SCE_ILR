//===================================================
//
//===================================================

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;



public class TestAsync : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.X))
        {
            Debug.Log("执行同步方法");
            TestMethod();
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            Debug.Log("执行异步方法");

            //关键词 Task  async await
            //第一种方式 常用方式
            //Task.Factory.StartNew(TestMethod);

            //第二种方式 
            //TestMethodAsync();

            TestMethodAsync2();
            Debug.Log("异步方法执行完毕=");
        }
    }

    public async Task<int> Test1()
    {
        int ret = 0;
        for (int i = 0; i < 100; i++)
        {
            ret = i;
            await Task.Delay(1);  //延迟1毫秒
        }
        return ret;
    }

    public async void TestMethodAsync2()
    {
        int result = await Test1();
        Debug.Log("结果=" + result);
    }

    public async void TestMethodAsync()
    {
        for (int i = 0; i < 5000; i++)
        {
            Debug.Log(i);
            await Task.Delay(1);  //延迟1毫秒
        }

    }

    private void TestMethod()
    {
        for (int i = 0; i < 5000; i++)
        {
            Debug.Log(i);
        }
    }
}
