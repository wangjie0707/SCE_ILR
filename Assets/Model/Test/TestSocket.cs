//===================================================
//
//===================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI;
using Myth;

public class TestSocket : MonoBehaviour {

	// Use this for initialization
	void Start () {
			
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyUp(KeyCode.A))
        {
            GameEntry.Socket.ConnectToMainSocket("192.168.1.111", 1038);
        }
        if (Input.GetKeyUp(KeyCode.B))
        {
            System_SendLocalTimeProto proto = new System_SendLocalTimeProto();
            GameEntry.Socket.SendMeg(proto.ToArray());
        }
    }
}
