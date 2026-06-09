using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Script_StopDrunk : MonoBehaviour
{
    //センターマーカー表示のオンオフを保存してほかのスクリプトから参照する
    public static bool isStopDrunk;

    [Header("トグル")]
    [SerializeField] private Toggle toggle;

    private void Start()
    {
        toggle.isOn = false;
    }

    public void OnToggle(bool value)
    {
        toggle.isOn = value;
        isStopDrunk = toggle.isOn;
        Debug.Log("センターマーカー表示を" + isStopDrunk + "にした");
    }

}
