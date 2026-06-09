using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Script_CalibrationCheck : MonoBehaviour
{

    [Header("キャリブレーション確認用ボタン")]
    [SerializeField] private Button CheckButton;

    [Header("キャリブレーション確認結果表示用テキスト")]
    [SerializeField] private TMP_Text ResultText;

    string routePath;

    // Start is called before the first frame update
    void Start()
    {
        ResultText.text = "NO";
        routePath = Path.Combine(Application.persistentDataPath,
            "Player_Data", Script_PlayerSelectManager.SelectedPlayerSlot + "_PlayerData",
            "Setting", "SpeedSetting.json");

    }

    public void OnCalibrationCheck()
    {
        if (File.Exists(routePath))
        {
            ResultText.text = "OK!";
        }
        else
        {
            ResultText.text = "NO";
        }
    }
}