using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_ErrorText : MonoBehaviour
{
    [Header("カメラ")]
    [SerializeField] private GameObject cameraObject;

    [Header("キャリブレーション結果送信エラーテキスト")]
    [SerializeField] private GameObject calibrationErrotext;

    [Header("傾斜取得失敗エラーテキスト")]
    [SerializeField] private GameObject inclineErrorText;

    [Header("切替間隔")]
    [SerializeField] private float interval;

    private void Update()
    {
        if (!cameraObject.GetComponent<Script_RunningCameraManager>().setCalibrationFlag &&
            !cameraObject.GetComponent<Script_RunningCameraManager>().getInclineFlag)
        {
            // 0 と 1 を交互に生成
            int phase = Mathf.FloorToInt(Time.time / interval) % 2;

            if (phase == 0)
            {
                calibrationErrotext.SetActive(false);
                inclineErrorText.SetActive(true);
            }
            else
            {
                calibrationErrotext.SetActive(true);
                inclineErrorText.SetActive(false);
            }
        }
    }
}
