using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_StageSelectButtonManager : MonoBehaviour
{

    //宣言するだけだと、形だけで中身がない
    //そのためインスペクター側で中身事セットする
    [Header("「Script_StageSelectManager.cs」のオブジェクトをここにセットする")]
    [SerializeField] private Script_StageSelectManager stageSelectManager;

    [Header("ボタンと対応付けさせるスロット番号")]
    [SerializeField] private int slotNum;

    public void OnStageSlot()
    {
        stageSelectManager.OnStageSelected(slotNum);
    }

}
