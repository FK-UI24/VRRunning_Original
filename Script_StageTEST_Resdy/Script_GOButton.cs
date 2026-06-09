using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Script_GOButton : MonoBehaviour
{
    [Header("接続確認結果テキスト")]
    [SerializeField] private TMP_Text Result;
    [Header("キャリブレーション確認結果テキスト")]
    [SerializeField] private TMP_Text calibrationText;

    // Update is called once per frame
    void Update()
    {
        if (Result.text == "OK!"&&calibrationText.text=="OK!")
        {
            this.gameObject.GetComponent<Button>().interactable = true;
        }
        else
        {
            this.gameObject.GetComponent<Button>().interactable = false;
        }
    }
}
