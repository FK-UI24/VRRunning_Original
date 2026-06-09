using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Script_MoveToSetting : MonoBehaviour
{
    //呼ばれたらSettingファイルの確認をしてなかったら生成する
    public void checkSettingFolder()
    {
        string settingFolder = Path.Combine(Application.persistentDataPath,
            "Player_Data", Script_PlayerSelectManager.SelectedPlayerSlot + "_PlayerData",
            "Setting");

        if (!Directory.Exists(settingFolder))
        {
            Directory.CreateDirectory(settingFolder);
            Debug.Log("Settingフォルダを作った");
        }
        else
        {
            Debug.Log("Settingフォルダはある");
        }
    }
}
