using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Script_MPU6050GetPitch : MonoBehaviour
{
    [Header("計測用ボタン")]
    [SerializeField] private Button measureButton;

    [Header("計測中に停止ボタンリスト")]
    [SerializeField] private List<Button> stopButtons = new List<Button>();

    [Header("ステータス表示用Text")]
    [SerializeField] private TMP_Text statusText;

    [Header("ステータス更新間隔(これはあくまでUnity側での表示間隔。実際の間隔はサーバー側に記入。)")]
    [SerializeField] private float statusInterval;

    //計測中かの判定
    private bool isMeasure = false;

    //コルーチン保持用
    private Coroutine measureCoroutine;

    //IPアドレスとポート番号の参照用変数
    private Script_IP config;

    //この関数をOnclickに設定する
    public void OnMeasure()
    {
        //計測中でなかったら
        if (isMeasure == false)
        {
            //ステータステキストと計測ボタンの初期設定
            statusText.text = "接続中";
            measureButton.GetComponentInChildren<TMP_Text>().text = "Measure Incline";

           //実行したらボタンの文字を「Abort」に変更する
           measureButton.GetComponentInChildren<TMP_Text>().text = "Abort";

            //計測中は一部のボタンを無効化する
            foreach (Button btn in stopButtons)
            {
                //ボタンがnullじゃなかったら
                if (btn != null)
                {
                    btn.interactable = false;
                }
            }

            //コルーチンを開始
            measureCoroutine = StartCoroutine(StartMeasure());
            //ここに結果を取得するコルーチンを入れると取得できる
            //Script_CalibrationButton参照
        }
        //もし計測中なら
        else
        {
            if (measureCoroutine != null)
            {
                //Flaskに終了要求を送る
                StartCoroutine(StopMeasure());
                //既に動いているコルーチンを停止する
                StopCoroutine(measureCoroutine);
                //既に動いていたコルーチンをnullにする
            }
            //ステータスを更新する
            statusText.text = "計測を終了しました";
            //計測中のフラグを戻す
            isMeasure = false;
            //ボタンの文字を戻す
            measureButton.GetComponentInChildren<TMP_Text>().text = "Measure Incline";
            //計測中に無効化したボタンを有効化する
            foreach (Button btn in stopButtons)
            {
                if (btn != null)
                {
                    btn.interactable = true;
                }
            }
        }
    }

    //計測開始用関数を呼び出す
    private IEnumerator StartMeasure()
    {
        //計測開始フラグを立てる「
        isMeasure = true;

        //フォーム作成
        WWWForm form = new WWWForm();

        //Resourcesフォルダをロードして指定のファイルを参照する
        config = Resources.Load<Script_IP>("IPConfig");
        string startURL = "http://" + config.ipaddress + ":" + config.port + "/start_mpu6050_get_pitch";

        //POST送信
        using (UnityWebRequest startReq = UnityWebRequest.Post(startURL, form))
        {
            //サーバーからの応答を待つ
            yield return startReq.SendWebRequest();

            //もしステータスコードが200でないなら
            if (startReq.result != UnityWebRequest.Result.Success)
            {
                //ボタンの文字を戻す
                measureButton.GetComponentInChildren<TMP_Text>().text = "Measure Incline";
                //計測中に無効化したボタンを有効化する
                foreach (Button btn in stopButtons)
                {
                    if (btn != null)
                    {
                        btn.interactable = true;
                    }
                }
                //ステータス更新
                statusText.text = "通信エラー：" + startReq.error;
                isMeasure = false;
                //終了
                yield break;
            }

            //計測中の値を定期取得する
            while (true)
            {
                //Resourcesフォルダをロードして指定のファイルを参照する
                config = Resources.Load<Script_IP>("IPConfig");
                string statusURL = "http://" + config.ipaddress + ":" + config.port + "/mpu6050_get_pitch_status";

                //GETで受け取る
                using (UnityWebRequest statusReq = UnityWebRequest.Get(statusURL))
                {
                    //サーバーからの応答を待つ
                    yield return statusReq.SendWebRequest();

                    //もしステータスコードが200なら
                    if (statusReq.result == UnityWebRequest.Result.Success)
                    {
                        //受け取った結果をステータステキストに代入する
                        string cuurentStatus = statusReq.downloadHandler.text;
                        statusText.text = cuurentStatus;
                    }
                    else
                    {
                        //ボタンの文字を戻す
                        measureButton.GetComponentInChildren<TMP_Text>().text = "Measure Incline";
                        //計測中に無効化したボタンを有効化する
                        foreach (Button btn in stopButtons)
                        {
                            if (btn != null)
                            {
                                btn.interactable = true;
                            }
                        }
                        //ステータス更新
                        statusText.text = "通信エラー：" + statusReq.error;
                    }
                    //設定した時間停止
                    yield return new WaitForSeconds(statusInterval);

                }

            }
        }
    }

    //計測終了リクエスト
    private IEnumerator StopMeasure()
    {
        //Resourcesフォルダをロードして指定のファイルを参照する
        config = Resources.Load<Script_IP>("IPConfig");
        string stopURL = "http://" + config.ipaddress + ":" + config.port + "/stop_mpu6050_get_pitch";

        //フォーム作成
        WWWForm emptyForm=new WWWForm();
        //POST送信
        using(UnityWebRequest stopReq = UnityWebRequest.Post(stopURL, emptyForm))
        {

            //サーバーからの応答を待つ
            yield return stopReq.SendWebRequest();

            //もしステータスコードが200でないなら
            if (stopReq.result != UnityWebRequest.Result.Success)
            {
                //ログにエラーを表示する
                Debug.Log("終了リクエスト失敗：" + stopReq.error);
            }
            else
            {
                //ログに成功を表示する
                Debug.Log("終了リクエスト成功：" + stopReq.downloadHandler.text);
            }

        }
    }
}
