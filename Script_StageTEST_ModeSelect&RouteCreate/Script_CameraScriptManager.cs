using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_CameraScriptManager : MonoBehaviour
{
    //制御するスクリプトを指定
    [Header("特定のパネルのみで使用するスクリプト（実際の制御は別のオブジェクトから行っている）")]
    [SerializeField]private Script_CameraControll script_CameraControll;
    [SerializeField]private Script_WayPointDataManagement wayPointDataManagement;



    void Start()
    {
        script_CameraControll.enabled = false;
        wayPointDataManagement.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //ルートづくりの場合
    public void RouteCreater_OnScript()
    {
        script_CameraControll.enabled = true;
        wayPointDataManagement.enabled=true;

    }

    public void RouteCreater_OffScript()
    {
        script_CameraControll.enabled = false;
        wayPointDataManagement.enabled = false;
    }

    //ランニングを選択した場合
    public void Running_OnScript()
    {
        script_CameraControll.enabled = true;

    }
    public void Running_OffScript()
    {
        script_CameraControll.enabled = false;
    }
}
