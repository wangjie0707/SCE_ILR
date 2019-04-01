//===================================================
//
//===================================================

using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI;
using Myth;

public class TestPool : MonoBehaviour
{

    public GameObject sds;
    private GameObjectPool GameObjectPool;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


        if (Input.GetKeyUp(KeyCode.B))
        {
            GameEntry.Resource.LoadAsset("102000",  typeof(GameObject), 0,
                new LoadAssetCallbacks((assetName, asset, duration, userData) =>
                {
                    Debug.Log(assetName);
                    GameObject obj = (GameObject)asset;
                    GameObjectPool = new GameObjectPool(obj);
                    Debug.Log(duration);
                    Debug.Log(userData);
                }), 1);
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            GameObjectPool = new GameObjectPool(sds);
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            GameObject obj = GameObjectPool.Spawn();

            StartCoroutine(Despawn(obj));
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            GameObjectPool.Clear();
        }
    }



    private IEnumerator Despawn(GameObject instanc)
    {
        yield return new WaitForSeconds(20f);
        GameObjectPool.UnSpawn(instanc);
    }
}
