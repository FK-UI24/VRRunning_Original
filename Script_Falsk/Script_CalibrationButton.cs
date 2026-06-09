using NetTopologySuite.Index.Strtree;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Script_CalibrationButton : MonoBehaviour
{
    [Header("キャリブレーション用ボタン")]
    [SerializeField] private Button calibrationButton;

    [Header("キャリブレーション中に停止ボタンリスト")]
    [SerializeField] private List<Button> stopButtons = new List<Button>();

    [Header("既定速度")]
    [SerializeField] private float targetSpeed;

    [Header("閾値")]
    [SerializeField] private float stabilityThreshold;

    [Header("ステータス表示用Text")]
    [SerializeField] private TMP_Text statusText;

    [Header("ステータス更新間隔(これはあくまでUnity側での表示間隔。実際の間隔はサーバー側に記入。)")]
    [SerializeField] private float statusInterval;

    //キャリブレーション中かどうかの判定
    private bool isCalibrating = false;

    //キャリブレーションが正しく終わったかの判定用変数
    private bool isNormalEnd = false;

    //コルーチン保持用
    private Coroutine calibrationCoroutine;

    //IPアドレスとポート番号の参照用変数
    private Script_IP config;

    //この関数をOnClickに設定する
    public void OnCalibration()
    {
        //キャリブレーション中でなかったら
        if (isCalibrating == false)
        {
            //ステータステキストとキャリブレーションボタンの初期設定
            statusText.text = "接続中";
            calibrationButton.GetComponentInChildren<TMP_Text>().text = "Calibration";
            //速度情報保存用ファイルパス
            string settingFile = Path.Combine(Application.persistentDataPath,
                    "Player_Data", Script_PlayerSelectManager.SelectedPlayerSlot + "_PlayerData",
                    "Setting", "SpeedSetting.json");
            //ファイルの確認をしてなかったら生成する
            if (!File.Exists(settingFile))
            {
                File.Create(settingFile);
                Debug.Log("SpeedSettingファイルを作った");
            }
            else
            {
                Debug.Log("SpeedSettingファイルはある");
            }

            //実行したらボタンの文字を「Abort」に変更する
            calibrationButton.GetComponentInChildren<TMP_Text>().text = "Abort";
            //キャリブレーション中は一部のボタンを無効化する
            foreach (Button btn in stopButtons)
            {
                //ボタンがnullじゃなかったら
                if (btn != null)
                {
                    btn.interactable = false;
                }
            }
            //コルーチンを開始
            calibrationCoroutine = StartCoroutine(StartCalibration());
            StartCoroutine(CalibrationResult());

        }
        //もしキャリブレーション中なら
        else
        {
            //コルーチンが実行されていたら
            if (calibrationCoroutine != null)
            {
                //Flaskに停止要求を送る
                StartCoroutine(StopCalibrationRequest());
                //※StopCoroutineは呼ばない。サーバーの完了を待つ
            }
            statusText.text = "キャリブレーションを中断しました";
            isCalibrating = false;
            //ボタンの文字を戻す
            calibrationButton.GetComponentInChildren<TMP_Text>().text = "Calibration";
            //キャリブレーション中に無効化したボタンを有効化する
            foreach (Button btn in stopButtons)
            {
                if (btn != null)
                {
                    btn.interactable = true;
                }
            }
        }
    }

    //キャリブレーション用関数を呼び出す
    private IEnumerator StartCalibration()
    {
        isCalibrating = true;

        //フォーム作成
        WWWForm form = new WWWForm();
        form.AddField("target_speed", targetSpeed.ToString());
        form.AddField("stability_threshold", stabilityThreshold.ToString());

        //Resourcesフォルダをロードして指定のファイルを参照する
        config = Resources.Load<Script_IP>("IPConfig");
        string starturl = "http://" + config.ipaddress + ":" + config.port + "/start_calibration";

        //POST送信
        using (UnityWebRequest startReq = UnityWebRequest.Post(starturl, form))
        {
            //サーバーからの応答をまつ
            yield return startReq.SendWebRequest();

            //もしステータスコードが200でないなら
            if (startReq.result != UnityWebRequest.Result.Success)
            {
                //ボタンの文字を戻す
                calibrationButton.GetComponentInChildren<TMP_Text>().text = "Calibration";

                //キャリブレーション中に無効化したボタンを有効化する
                foreach (Button btn in stopButtons)
                {
                    if (btn != null)
                    {
                        btn.interactable = true;
                    }
                }

                statusText.text = "通信エラー：" + startReq.error;
                isCalibrating = false;
                yield break;
            }

            //キャリブレーション中の状態を定期取得する
            while (true)
            {
                //Resourcesフォルダをロードして指定のファイルを参照する
                config = Resources.Load<Script_IP>("IPConfig");
                string statusURL = "http://" + config.ipaddress + ":" + config.port + "/calibration_status";

                //GETで受け取る
                UnityWebRequest statusReq = UnityWebRequest.Get(statusURL);

                //サーバーからの応答を待つ
                yield return statusReq.SendWebRequest();

                //もしステータスコードが200なら
                if (statusReq.result == UnityWebRequest.Result.Success)
                {
                    //受け取った結果をステータステキストに代入する
                    string currentStatus = statusReq.downloadHandler.text;
                    statusText.text = currentStatus;
                    if (currentStatus.Contains("終了"))
                    {
                        //ボタンの文字を戻す
                        calibrationButton.GetComponentInChildren<TMP_Text>().text = "Calibration";
                        //キャリブレーション中は一部のボタン有効化する
                        foreach (Button btn in stopButtons)
                        {
                            if (btn != null)
                            {
                                btn.interactable = true;
                            }
                        }
                        isNormalEnd = true;
                        //キャリブレーション終了
                        break;
                    }
                    else if (currentStatus.Contains("中断"))
                    {
                        //ボタンの文字を戻す
                        calibrationButton.GetComponentInChildren<TMP_Text>().text = "Calibration";
                        //キャリブレーション中は一部のボタン有効化する
                        foreach (Button btn in stopButtons)
                        {
                            if (btn != null)
                            {
                                btn.interactable = true;
                            }
                        }

                        //キャリブレーション中断
                        break;
                    }
                }
                else
                {
                    //ボタンの文字を戻す
                    calibrationButton.GetComponentInChildren<TMP_Text>().text = "Calibration";
                    //キャリブレーション中に無効化したボタンを有効化する
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
            //終わったのでフラグとコルーチンを戻す
            isCalibrating = false;
            calibrationCoroutine = null;

        }

    }


    //キャリブレーション停止リクエスト
    private IEnumerator StopCalibrationRequest()
    {
        //Resourcesフォルダをロードして指定のファイルを参照する
        config = Resources.Load<Script_IP>("IPConfig");
        string stopurl = "http://" + config.ipaddress + ":" + config.port + "/stop_calibration";

        //フォーム作成
        WWWForm emptyForm = new WWWForm();
        //POST送信
        using (UnityWebRequest stopReq = UnityWebRequest.Post(stopurl, emptyForm))
        {
            yield return stopReq.SendWebRequest();

            //もしステータスコードが200でないなら
            if (stopReq.result != UnityWebRequest.Result.Success)
            {
                //ログにエラーを表示する
                Debug.Log("停止リクエスト失敗：" + stopReq.error);
            }
            else
            {
                //ログに成功を表示する
                Debug.Log("停止リクエスト成功：" + stopReq.downloadHandler.text);
            }

        }
    }


    //キャリブレーション結果を受け取るリクエスト
    private IEnumerator CalibrationResult()
    {
        //キャリブレーション中は待ち続ける
        while (isCalibrating)
        {
            yield return null;
        }
        if (isNormalEnd)
        {
            //Resourcesフォルダをロードして指定のファイルを参照する
            config = Resources.Load<Script_IP>("IPConfig");
            string resultURL = "http://" + config.ipaddress + ":" + config.port + "/calibration_result";

            using (UnityWebRequest resultReq = UnityWebRequest.Get(resultURL))
            {
                yield return resultReq.SendWebRequest();


                if (resultReq.result == UnityWebRequest.Result.Success)
                {
                    string avg_steps = resultReq.downloadHandler.text;
                    Debug.Log(targetSpeed + ":" + avg_steps);
                    //速度情報保存用ファイルパス
                    //順当に行くと既にファイルは存在してるので確認はしない
                    string settingFile = Path.Combine(Application.persistentDataPath,
                            "Player_Data", Script_PlayerSelectManager.SelectedPlayerSlot + "_PlayerData",
                            "Setting", "SpeedSetting.json");

                    //target_speedをキーにしたavg_stepsを値にしたjsonの定型文を作る
                    string saveJson = "{ \"" + targetSpeed + "\": " + avg_steps + " }";

                    //ファイルに書き込み（毎回完全上書きするので事前に中身の参照をしない）
                    File.WriteAllText(settingFile, saveJson);
                    Debug.Log("基準速度をキーとしたときの平均ステップ数を保存した");
                }
                else
                {
                    Debug.LogError(resultReq.error);
                }


                //正常に終了したフラグを戻す
                isNormalEnd = false;

            }
        }

    }

}
