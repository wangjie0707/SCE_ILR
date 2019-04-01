using ILHotfix;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class TestFix : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        ABC();
        ABC(2);
        ABC("start");
        ABC(4, "start");
        ABC("start", 5);

        int x =0;
        string y = string.Empty;
        ABC(ref x, out y);

        int t = ABC(10, 10);

        ABC<float>(3.1415f, "stringvalue");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F))
        {
            ABC();
            ABC(2);
            ABC("start");
            ABC(4, "start");
            ABC("start", 5);

            int x = 0;
            string y = string.Empty;
            ABC(ref x, out y);

            int t = ABC(10, 10);

            ABC<float>(3.1415f, "stringvalue");
        }
    }


    public void ABC()
    {
        Debug.Log("ABC");
    }


    private void ABC(int a)
    {
        Debug.Log(a);
    }

    private void ABC(string a)
    {
        Debug.Log(a);
    }


    private void ABC(string a, int b)
    {
        Debug.Log(a + "  +  " + b);
    }

    private void ABC(int a, string b)
    {
        Debug.Log(a + "  +  " + b);
    }

    private void ABC(ref int a, out string b)
    {
        a = 100;
        b = "hello world";
        Debug.Log(a + "  +  " + b);
    }

    private int ABC(int a, int b)
    {
        Debug.Log((a + b));
        return a + b;
    }

    private void ABC<T>(T a, string b)
    {
        Debug.Log( typeof(T).Name);
        Debug.Log(a + "  +  " + b);
    }
}



