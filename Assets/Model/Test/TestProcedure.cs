//===================================================
//
//===================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI;
using Myth;

public class TestProcedure : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {


    }

    private IEnumerator Test1(VarInt a)
    {
        //保留 如果使用同步方法 可以不保留
        a.Retain();
        yield return new WaitForSeconds(5);
        Debug.Log("a=" + a.Value);

        Debug.Log("在协程中释放");
        //释放了
        a.Release();
    }

    // Update is called once per frame
    void Update()
    {

        //==================================================

        //if (Input.GetKeyUp(KeyCode.A))
        //{
        //    //分配一个对象 Alloc和Release 对应    Alloc在最开始
        //    VarInt a = VarInt.Alloc(10);
        //    int x = a;

        //    StartCoroutine(Test1(a));
        //    Debug.Log("直接释放");
        //    //释放了  结束时候一定要释放
        //    a.Release();


        //}
        //============================================================


        //if (Input.GetKeyUp(KeyCode.B))
        //{
        //    GameEntry.Procedure.SetData("name", "UnityEngine.UI");
        //    GameEntry.Procedure.SetData("code", 12);
        //    GameEntry.Procedure.ChangeState(ProcedureState.CheckVersion);
        //}

        //if (Input.GetKeyUp(KeyCode.C))
        //{
        //    GameEntry.Procedure.ChangeState(ProcedureState.EnterGame);
        //}

        //if (Input.GetKeyUp(KeyCode.D))
        //{
        //    GameEntry.Procedure.ChangeState(ProcedureState.Launch);
        //}




        // ================================

        //if (Input.GetKeyUp(KeyCode.E))
        //{
        //    ChapterDBModel dr = new ChapterDBModel();
        //    dr.LoadData();

        //    List<ChapterEntity> lst = dr.GetList();
        //    int len = lst.Count;
        //    for (int i = 0; i < len; i++)
        //    {
        //        ChapterEntity entity = lst[i];
        //        Debug.Log("Id=" + entity.Id);
        //        Debug.Log("Name=" + entity.ChapterName);
        //    }
        //}

        //===================================================

        //if (Input.GetKeyUp(KeyCode.B))
        //{
        //    GameEntry.Procedure.ChangeState(ProcedureState.Preload);
        //}
        //if (Input.GetKeyUp(KeyCode.C))
        //{
        //    GameEntry.DataTable.DataTableManager.LoadDataTable();
        //}
    }
}

