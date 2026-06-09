using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Script_GoalTimer : MonoBehaviour
{
    [Header("ランニングタイマー")]
    [SerializeField] private GameObject runningTimer;

    [Header("Goalパネル")]
    [SerializeField] private GameObject goalPanel;

    //タイマーを1回だけ代入するフラグ
    private bool isTimer = false;

    private void Update()
    {
        if (!isTimer && goalPanel.activeSelf)
        {
            this.GetComponent<TMP_Text>().text=runningTimer.GetComponent<TMP_Text>().text;
        }
    }

}
