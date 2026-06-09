using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_StageSelectManager : MonoBehaviour
{
    //選択したステージ名を保存して外部から参照する用変数
    //何も決まっていないときはとりあえず「TEST」にする
    //シーンを切り替えても保持される
    //ただし再起動をしたり、自分で明示的に代入するとリセットされる
    public static string SelectedStageName;

    private void Start()
    {
        SelectedStageName = "TEST";
    }

    public void OnStageSelected(int SelectedStageSlotNumber)
    {
        //選択されたスロット番号に対応するステージ名を記録する
        if (SelectedStageSlotNumber == 1) SelectedStageName = "TEST";
        else if (SelectedStageSlotNumber == 2) SelectedStageName = "Numadu";
    }

}
