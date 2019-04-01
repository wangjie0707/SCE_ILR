using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Myth;
using Myth;

public class TestDatatable : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.B))
        {
            GameEntry.DataTable.LoadDataTable<ChapterDBModel>("Assets/DownLoad/DataTable/Chapter.bytes", 0, 132);
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            ChapterDBModel chapterDBModel = GameEntry.DataTable.GetDataTable<ChapterDBModel>();
            ChapterEntity chapterEntity = chapterDBModel.Get(1);
            Debug.Log(chapterEntity.ChapterName);
            chapterEntity = chapterDBModel.Get(2);
            Debug.Log(chapterEntity.ChapterName);
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            GameEntry.DataTable.DestroyDataTable<ChapterDBModel>();
        }
    }
}
