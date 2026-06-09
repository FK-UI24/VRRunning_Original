using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Script_MapDisplay : MonoBehaviour
{
    //マップ表示のオンオフを保存してほかのスクリプトから参照する
    public static bool isMapDisplay;

    [Header("トグル")]
    [SerializeField] private Toggle toggle;

    private void Start()
    {
        toggle.isOn = false;
    }

    public void OnToggle(bool value)
    {
        toggle.isOn = value;
        isMapDisplay = toggle.isOn;
        Debug.Log("マップ表示を" + isMapDisplay + "にした");
    }

}
