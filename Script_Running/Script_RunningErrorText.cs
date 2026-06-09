using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_RunningErrorText : MonoBehaviour
{
    [Header("カメラ")]
    [SerializeField] private GameObject cameraObject;

    [Header("速度取得失敗中エラーテキスト")]
    [SerializeField] private GameObject GetSpeedErrorText;

    [Header("傾斜取得失敗中エラーテキスト")]
    [SerializeField] private GameObject GetInclineErrorText;

    [Header("切替間隔")]
    [SerializeField] private float interval;

    [Header("ゴールパネル")]
    [SerializeField] private GameObject goalPanel;

    private void Update()
    {
        if(!cameraObject.GetComponent<Script_RunningCameraManager>().isGetSpeed&&
            !cameraObject.GetComponent<Script_RunningCameraManager>().isGetIncline&&
            !goalPanel.activeSelf)
        {
            // 0 と 1 を交互に生成
            int phase = Mathf.FloorToInt(Time.time / interval) % 2;

            if (phase == 0)
            {
                GetSpeedErrorText.SetActive(false);
                GetInclineErrorText.SetActive(true);
            }
            else
            {
                GetSpeedErrorText.SetActive(true);
                GetInclineErrorText.SetActive(false);
            }
        }
    }

}
