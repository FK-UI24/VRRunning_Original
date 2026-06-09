using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Script_GoalCarorie : MonoBehaviour
{
    [Header("カメラ")]
    [SerializeField] private GameObject cameraObject;

    [Header("カロリー表示テキスト")]
    [SerializeField] private GameObject calorieText;

    //1回だけやるフラグ
    private bool isCalorie=false;

    // Update is called once per frame
    void Update()
    {
        if(!isCalorie&&
            cameraObject.GetComponentInChildren<Script_RunningCameraManager>().goalStatus)
        {
            this.GetComponent<TMP_Text>().text = calorieText.GetComponent<TMP_Text>().text;
            isCalorie =true;
        }
    }
}
