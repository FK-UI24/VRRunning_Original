using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Script_RouteNum : MonoBehaviour
{
    //ルート番号を返すプログラム

    [Header("ルート番号表示用テキスト")]
    [SerializeField] private TMP_Text RouteNum;

    private void Start()
    {
        RouteNum.text = "-Route " + DataSelectManager.SelectedRouteSlot + "-";
    }
}
