using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Script_AvgIncline : MonoBehaviour
{
    [Header("傾斜表示テキスト")]
    [SerializeField] private GameObject inclineText;

    [Header("カメラ")]
    [SerializeField] private GameObject cameraObject;

    [Header("平均傾斜表示テキスト")]
    [SerializeField] private GameObject avgInclineText;


    private float timer = 0f;

    //ゴール時に計算を１回だけやるフラグ
    private bool isIncline = false;

    //合計傾斜角度
    private float totalIncline = 0f;

    //何回傾斜を取得したかのカウンター
    private int count = 0;



    //タイマーで5秒ごとに傾斜を取得してゴール時にまとめて計算する
    private void Update()
    {
        //始まっていないなら測定しない
        if (!cameraObject.GetComponent<Script_RunningCameraManager>().runningStatus)
        {
            return;
        }


        timer += Time.deltaTime;

        if (timer >= 5f)
        {
            totalIncline += 
                float.Parse(inclineText.GetComponent<TMP_Text>().text.Replace("°",""));
            count++;
            timer = 0f;
        }

        if(!isIncline&&
            cameraObject.GetComponent<Script_RunningCameraManager>().goalStatus)
        {
            float averageIncline = 0f;
            //0除算（countが0の時に割り算するエラー）を防ぐ
            if (count > 0)
            {
                //平均を計算
                averageIncline = totalIncline / count;
            }

            avgInclineText.GetComponent<TMP_Text>().text = averageIncline.ToString("F1")+"°";
            isIncline = true;
        }

    }

}
