using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Script_ConnectTest : MonoBehaviour
{
    [Header("接続テスト用ボタン")]
    [SerializeField] private Button connectTestButton;

    [Header("接続テスト中に停止ボタンリスト")]
    [SerializeField] private List<Button> stopButtons = new List<Button>();

    [Header("ステータス表示用Text")]
    [SerializeField] private TMP_Text statusText;

    //接続テスト中かどうかの判定
    private bool isConnectTest = false;

    //IPアドレスとポート番号の参照用変数
    private Script_IP config;

    void Start()
    {
        //ステータステキストと接続テストボタンの初期設定
        statusText.text = "ステータスメッセージ";
        connectTestButton.GetComponentInChildren<TMP_Text>().text = "ConnectTest";
    }

    //ここをOnClickに設定する
    public void OnConnectTest()
    {
        if (isConnectTest == false)
        {
            //実行したら接続テストボタンを無効にする
            connectTestButton.interactable = false;

            //実行したら一部のボタンを無効化する
            foreach(Button btn in stopButtons)
            {
                btn.interactable = false;
            }
            StartCoroutine(ConnectTestCoroutine());
        }
    }

    //接続テスト用関数
    private IEnumerator ConnectTestCoroutine()
    {
        isConnectTest = true;

        //Resourcesフォルダをロードして指定のファイルを参照する
        config = Resources.Load<Script_IP>("IPConfig");
        string url = "http://" + config.ipaddress + ":" + config.port + "/connecttest";

        
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {

            statusText.text = "接続中";

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                statusText.text = "接続できています";
            }
            else
            {
                statusText.text = "エラー：" + www.error;
            }

            //接続テストが終わったらボタンを有効化にする
            connectTestButton.interactable = true;
            foreach (Button btn in stopButtons)
            {
                if (btn != null)
                {
                    btn.interactable = true;
                }
            }

            //フラグを戻す
            isConnectTest = false;
        }


    }
}
