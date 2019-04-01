//===================================================
//
//===================================================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI;
using Myth;

public class TestEvent : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameEntry.Event.CommonEvent.AddEventListener(CommonEventId.RegComplete, OnRegComplete);
	}

    private void OnRegComplete(object userData)
    {
        Debug.Log(userData);
    }



    // Update is called once per frame
    void Update () {
        //if (Input.GetKeyUp(KeyCode.A))
        //{
        //    //GameEntry.Event.CommonEvent.Dispatch(CommonEventId.RegComplete, 123);
        //    GameEntry.Download.AddDownload(Application.dataPath + "/../ABC/def", "https://codeload.github.com/EllanJiang/StarForce/zip/master", (success) => { Debug.Log(success); });

        //}
        //if (Input.GetKeyUp(KeyCode.B))
        //{
        //    //GameEntry.Event.CommonEvent.Dispatch(CommonEventId.RegComplete, 123);
        //    GameEntry.Download.AddDownload(Application.dataPath + "/../ABC/abc", "https://youngqfbr-my.sharepoint.com/:t:/g/personal/shangshen_x1_tn/EWcO3tY85WhFpakLV578b-cBO0aj8upiVagHiUWlJhhdMQ");

        //}
        //if (Input.GetKeyUp(KeyCode.C))
        //{
        //    //GameEntry.Event.CommonEvent.Dispatch(CommonEventId.RegComplete, 123);
        //    GameEntry.Download.AddDownload(Application.dataPath + "/../ABC/efs", Application.dataPath + "/../BFBZ.zip");

        //}

        if (Input.GetKeyUp(KeyCode.A))
        {
            
        }
        if (Input.GetKeyUp(KeyCode.B))
        {
            
        }
    }

    private void OnDestroy()
    {
        Debug.Log("TestEvent OnDestroy");
        GameEntry.Event.CommonEvent.RemoveEventListener(CommonEventId.RegComplete, OnRegComplete);
    }
}
