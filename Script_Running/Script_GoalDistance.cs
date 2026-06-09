using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Script_GoalDistance : MonoBehaviour
{
    [Header("走行距離")]
    [SerializeField] private GameObject runningDistance;

    [Header("カメラ")]
    [SerializeField] private GameObject cameraObject;

    //走行距離を1回だけ代入するフラグ
    private bool isDistance = false;

    private void Update()
    {
        if (!isDistance && 
            cameraObject.GetComponentInChildren<Script_RunningCameraManager>().goalStatus)
        {
            this.GetComponent<TMP_Text>().text=runningDistance.GetComponent<TMP_Text>().text;
            isDistance = true;
        }
    }
}
