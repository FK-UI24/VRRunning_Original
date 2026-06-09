using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FalskTest : MonoBehaviour
{
    //IPアドレスとポート番号の参照用変数
    private Script_IP config;

    private IEnumerator Start()
    {
        //Resourcesフォルダをロードして指定のファイルを参照する
        config = Resources.Load<Script_IP>("IPConfig");
        string url = "http://" + config.ipaddress + ":" + config.port + "/";
        
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Response：" + www.downloadHandler.text);
            }
            else
            {
                Debug.Log("Error：" + www.error);
            }
        }
    }
}
