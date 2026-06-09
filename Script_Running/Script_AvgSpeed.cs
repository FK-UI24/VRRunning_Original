using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Script_AvgSpeed : MonoBehaviour
{
    [Header("カメラ")]
    [SerializeField] private GameObject cameraObject;

    [Header("走った距離(Running)")]
    [SerializeField] private GameObject distanceText;

    [Header("走った時間(Running)")]
    [SerializeField] private GameObject timeText;

    //1回だけやるフラグ
    private bool isAvgSpeed = false;

    private void Update()
    {
        //始まっていないなら測定しない
        if (!cameraObject.GetComponent<Script_RunningCameraManager>().runningStatus)
        {
            return;
        }


        if (!isAvgSpeed&&
            cameraObject.GetComponent<Script_RunningCameraManager>().goalStatus)
        {
            string[] parts = timeText.GetComponent<TMP_Text>().text.Split(":");

            float time = int.Parse(parts[0]) * 3600 + int.Parse(parts[1]) * 60 +
                int.Parse(parts[2]);

            int distance = int.Parse(distanceText.GetComponent<TMP_Text>().
                text.Replace("m", ""));

            float avgSpeed = (distance / time) * 3.6f;

            this.GetComponent<TMP_Text>().text = avgSpeed.ToString("F1")+"km/h";
            isAvgSpeed = true;
        }
    }
}
