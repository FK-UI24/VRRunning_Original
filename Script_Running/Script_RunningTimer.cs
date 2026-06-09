using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Script_RunningTimer : MonoBehaviour
{
    //経過時間
    private float elapsedTime = 0f;

    //計測中かどうか
    private bool isTimer = false;

    [Header("タイマ－テキスト")]
    [SerializeField] private TMP_Text timertext;

    [Header("カメラ")]
    [SerializeField] private GameObject cameraObject;

    private bool stopScreen = false;

    private void Update()
    {
        //もし停止中ならタイマーは動かさない
        stopScreen = cameraObject.GetComponentInChildren<Script_RunningCameraManager>().isStopRunning;

        if (stopScreen) return;

        if (isTimer)
        {
            elapsedTime += Time.deltaTime;

            int h = (int)(elapsedTime / 3600);
            int m = (int)(elapsedTime / 60) % 60;
            int s = (int)(elapsedTime % 60);

            timertext.text = string.Format("{0:00}:{1:00}:{2:00}", h, m, s);
        }
    }

    //これをStartボタンを押したら呼び出す
    public void StartTimer()
    {
        if (cameraObject.GetComponent<Script_RunningCameraManager>().setCalibrationFlag&&
            cameraObject.GetComponent<Script_RunningCameraManager>().getInclineFlag)
        {
            isTimer = true;
        }
    }

    //これをゴールをしたとき、またはPAUSE画面からランニングを終了したときに呼び出す
    public void StopTimer()
    {
        isTimer = false;
    }

}
