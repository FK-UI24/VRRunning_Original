using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Script_ConnectCheck : MonoBehaviour
{
    //ラズパイと正しく接続できているかを確認するプログラム

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
        statusText.text = "NO";
        connectTestButton.GetComponentInChildren<TMP_Text>().text = "Connect Check";
    }

    //ここをOnClickに設定する
    public void OnConnectTest()
    {
        if (isConnectTest == false)
        {
            //実行したら接続テストボタンを無効にする
            connectTestButton.interactable = false;

            //実行したら一部のボタンを無効化する
            foreach (Button btn in stopButtons)
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
                statusText.text = "OK!";
            }
            else
            {
                statusText.text = "エラー";
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
