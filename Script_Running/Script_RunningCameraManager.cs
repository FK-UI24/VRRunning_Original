using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Script_RunningCameraManager : MonoBehaviour
{

    [Header("開発モードを使用するか")]
    [SerializeField] private bool DebugMode;

    [Header("速度（開発モード）")]
    [SerializeField] private float DebugSpeed;

    [Header("速度取得間隔")]
    [SerializeField] private float getSpeedInterbal;

    [Header("傾斜取得間隔")]
    [SerializeField] private float getInclineInterbal;

    [Header("速度表示用テキスト")]
    [SerializeField] private GameObject speedText;

    [Header("傾斜表示用テキスト")]
    [SerializeField] private GameObject inclineText;

    [Header("「START」パネル")]
    [SerializeField] private GameObject startPanel;

    [Header("「Running」パネル")]
    [SerializeField] private GameObject runningPanel;

    [Header("「Goal」パネル")]
    [SerializeField] private GameObject goalPanel;

    [Header("「PAUSE」パネル")]
    [SerializeField] private GameObject pausePanel;

    [Header("開発用ステージ名（もし以前のシーンでデータが設定されていなかったらこれになる）")]
    [SerializeField] private string DebugStageName;

    [Header("Script_Running_LineRenderer.csをアタッチしたオブジェクト")]
    [SerializeField] private Script_Running_LineRenderer lineRenderer;

    [Header("Script_RunningTimer.csをアタッチしたオブジェクト")]
    [SerializeField] private Script_RunningTimer runningTimer;

    [Header("Script_GoalSE.csをアタッチしたオブジェクト")]
    [SerializeField] private GameObject goalSE;

    [Header("回転の基礎速度")]
    [SerializeField] private float baseRotationSpeed;

    [Header("回転の最大速度")]
    [SerializeField] private float maxRotationSpeed;

    [Header("回転の最小速度")]
    [SerializeField] private float minRotationSpeed;

    [Header("スタートパネルで速度/傾斜取得が失敗したときのテキスト")]
    [SerializeField] private GameObject[] StartErrorText;

    [Header("走行中に速度/傾斜取得が失敗したときのテキスト")]
    [SerializeField] private GameObject[] RunningErrorText;

    [Header("「START」パネル内の「START」ボタン")]
    [SerializeField] private Button STARTButoon;

    [Header("警告停止カウントダウンテキスト")]
    [SerializeField] private GameObject StopCountDown;

    [Header("警告停止テキスト")]
    [SerializeField] private GameObject StopText;

    [Header("速度取得と傾斜取得を停止する秒数")]
    [SerializeField] private float StopInterbal;

    [Header("停止中から走行を再開する時のカウントダウンテキスト")]
    [SerializeField] private GameObject reloadCountDown;

    [Header("停止中から走行開始までの秒数")]
    [SerializeField] private float reloadInterbal;

    [Header("PAUSEパネルのENDボタンを押したときに遷移するシーン名")]
    [SerializeField] private string nextScene;


    //走っている状態化のフラグ
    //走っているときはtrue,走っていないときはfalse
    public bool runningStatus = false;

    //ゴールしたかのフラグ
    //ゴールしたらtrue,ゴールしていないならfalse
    public bool goalStatus = false;

    //座標の到達をしてから座標を更新している間かの更新フラグ
    private bool updateStatus = false;

    //キャリブレーション結果をFalskサーバーにセットできたかどうかのフラグ
    public bool setCalibrationFlag = false;
    //Flaskサーバーから傾斜を取得できたかどうかのフラグ
    public bool getInclineFlag = false;

    //速度取得をしているかのフラグ
    public bool isGetSpeed = true;
    //傾斜取得をしているかのフラグ
    public bool isGetIncline = true;

    //画面の停止が実行しているかのフラグ
    public bool isStopRunning = false;
    //最初の速度/傾斜取得を待機したかのフラグ
    private bool firstGetWait = false;
    //エラーで停止し、再接続待機中かどうかのフラグ
    private bool isErrorStopped = false;
    //最初の速度データを無視するためのフラグ
    private bool ignoreFirstSpeedReading = true;

    private string routeJsonFilePath;
    private float speed;
    private Rigidbody CameraRb;
    private AudioSource[] SE;
    private Vector3 startPos, endPos;
    private List<Vector3> positionList = new List<Vector3>();
    private int passWayPointCount = 1;
    private string stageName;
    private Quaternion targetRotation;
    private float targetSpeed;
    private float calibrationStep;
    private Script_IP config;
    private string GetSpeedURL;
    private string GetInclineURL;

    private Coroutine getSpeedCoroutine;
    private Coroutine getInclineCoroutine;

    [Header("１秒あたりに曲がる角度")]
    [SerializeField] private float maxRotationDegreesPerSecond = 60f;
    [Header("水平回転のみかのフラグ")]
    [SerializeField] private bool horizontalOnly = false;

    //停止処理が呼ばれた理由を定義するenum
    private enum StopReason
    {
        Goal,
        Manual,
        ConnectionError,
        PostGoalError
    }

    void Start()
    {
        CameraRb = GetComponent<Rigidbody>();
        config = Resources.Load<Script_IP>("IPConfig");
        GetSpeedURL = "http://" + config.ipaddress + ":" + config.port + "/get_rotary_speed";
        GetInclineURL = "http://" + config.ipaddress + ":" + config.port + "/mpu6050_get_pitch_result";

        if (DebugMode)
        {
            speed = DebugSpeed;
            speedText.GetComponentInChildren<TMP_Text>().text = speed.ToString();
            setCalibrationFlag = true;
            getInclineFlag = true;
        }
        else
        {
            string settingFile = Path.Combine(Application.persistentDataPath, "Player_Data", Script_PlayerSelectManager.SelectedPlayerSlot + "_PlayerData", "Setting", "SpeedSetting.json");
            string json = File.ReadAllText(settingFile);
            JObject obj = JObject.Parse(json);
            targetSpeed = float.Parse(obj.Properties().First().Name);
            calibrationStep = obj.Properties().First().Value.Value<float>();
            StartCoroutine(SetCalibrationToFlask(targetSpeed, calibrationStep));
            StartCoroutine(CheckInclineFromFlask());
        }

        SE = GetComponents<AudioSource>();
        runningPanel.SetActive(false);
        goalPanel.SetActive(false);
        StartErrorText[0].SetActive(false);
        StartErrorText[1].SetActive(false);
        RunningErrorText[0].SetActive(false);
        RunningErrorText[1].SetActive(false);
        StopCountDown.SetActive(false);
        StopText.SetActive(false);
        if (reloadCountDown != null) reloadCountDown.SetActive(false);
        firstPositionLoad();
    }

    void ReTry()
    {
        STARTButoon.interactable = false;
        StartCoroutine(AttemptReconnection());
    }

    IEnumerator SetCalibrationToFlask(float targetSpeed, float calibrationStep)
    {
        string url = "http://" + config.ipaddress + ":" + config.port + "/set_rotary_speed";
        WWWForm form = new WWWForm();
        form.AddField("ref_speed", targetSpeed.ToString());
        form.AddField("ref_steps", calibrationStep.ToString());
        using (UnityWebRequest setSpeedReq = UnityWebRequest.Post(url, form))
        {
            yield return setSpeedReq.SendWebRequest();
            if (setSpeedReq.result != UnityWebRequest.Result.Success)
            {
                setCalibrationFlag = false;
                StartErrorText[0].GetComponentInChildren<TMP_Text>().text = "キャリブレーション結果送信_通信エラー：" + setSpeedReq.error;
            }
            else
            {
                setCalibrationFlag = true;
            }
        }
    }

    IEnumerator CheckInclineFromFlask()
    {
        string startURL = "http://" + config.ipaddress + ":" + config.port + "/start_mpu6050_get_pitch";
        using (UnityWebRequest startInclineReq = UnityWebRequest.Post(startURL, new WWWForm()))
        {
            yield return startInclineReq.SendWebRequest();
            if (startInclineReq.result != UnityWebRequest.Result.Success)
            {
                getInclineFlag = false;
                StartErrorText[1].GetComponentInChildren<TMP_Text>().text = "MPU6050_通信エラー：" + startInclineReq.error;
            }
            else
            {
                getInclineFlag = true;
            }
        }
    }

    void Update()
    {
        if (isErrorStopped && Input.GetKeyDown(KeyCode.R))
        {
            isErrorStopped = false;
            if (StopCountDown != null) StopCountDown.SetActive(false);
            if (StopText != null) StopText.SetActive(false);
            ReTry();
        }

        if (!runningStatus && !isStopRunning && !goalStatus)
        {
            if (!setCalibrationFlag || !getInclineFlag)
            {
                STARTButoon.GetComponentInChildren<TMP_Text>().text = "RETRY";
                StartErrorText[0].SetActive(!setCalibrationFlag);
                StartErrorText[1].SetActive(!getInclineFlag);
            }
            else
            {
                StartErrorText[0].SetActive(false);
                StartErrorText[1].SetActive(false);
                STARTButoon.GetComponentInChildren<TMP_Text>().text = "START";
            }
        }

        // --- ゴール後のエラー検知を先に実行する ---
        if (goalStatus && !isStopRunning && !DebugMode)
        {
            if (!isGetSpeed || !isGetIncline)
            {
                //ゴール後のエラーとして停止シーケンスを開始する
                StartCoroutine(StopScreen(StopReason.PostGoalError));
            }
            //ゴール後は走行中エラーテキストは表示しない
            RunningErrorText[0].SetActive(false);
            RunningErrorText[1].SetActive(false);
        }
        // --- 走行中のエラー検知と停止トリガー ---
        else if (runningStatus && firstGetWait && !isStopRunning && !DebugMode)
        {
            if (!isGetSpeed || !isGetIncline)
            {
                StartCoroutine(StopScreen(StopReason.ConnectionError));
            }
            // 走行中のエラーテキスト表示
            RunningErrorText[0].SetActive(!isGetSpeed);
            RunningErrorText[1].SetActive(!isGetIncline);
        }
        // --- 走行中でない場合のエラーテキスト非表示 ---
        else if (!isStopRunning)
        {
            RunningErrorText[0].SetActive(false);
            RunningErrorText[1].SetActive(false);
        }

        //正常に走っているとき
        if ((runningStatus || goalStatus) && !isStopRunning)
        {
            //速度を変えて、表示も変える
            CameraRb.linearVelocity = transform.forward * speed;
            speedText.GetComponentInChildren<TMP_Text>().text = speed.ToString("F0");


            if (!goalStatus)
            {
                Vector3 direction = (endPos - transform.position).normalized;
                if (direction != Vector3.zero)
                {
                    if (horizontalOnly)
                    {
                        direction.y = 0;
                        if (direction.sqrMagnitude < 0.001f) direction = transform.forward;
                    }
                    targetRotation = Quaternion.LookRotation(direction);
                }
                float maxStep = maxRotationDegreesPerSecond * Time.deltaTime;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxStep);
                if (Vector3.Distance(transform.position, endPos) < 0.5f && !updateStatus)
                {
                    updateStatus = true;
                    positionUpdate();
                }
            }
        }
        else if (isStopRunning)
        {
            speedText.GetComponentInChildren<TMP_Text>().text = speed.ToString("F0");
        }

        if (goalStatus && updateStatus)
        {
            runningTimer.StopTimer();
            goalSE.GetComponent<Script_GoalSE>().playSE();
            goalPanel.SetActive(true);
            if (pausePanel != null) pausePanel.SetActive(false);
            Debug.Log("ゴール！クールダウン走行を開始します。");
            updateStatus = false;
        }
    }
    IEnumerator FirstPanelOFFSE()
    {
        SE[0].Play();
        yield return new WaitForSeconds(SE[0].clip.length);
        startPanel.SetActive(false);
        runningPanel.SetActive(true);
        runningStatus = true;
        ignoreFirstSpeedReading = true;
        if (!DebugMode)
        {
            yield return StartCoroutine(SetCalibrationToFlask(targetSpeed, calibrationStep));
            if (setCalibrationFlag)
            {
                getSpeedCoroutine = StartCoroutine(GetSpeedFromFlask());
                getInclineCoroutine = StartCoroutine(GetInclineFromFlask());
            }
        }
        StartCoroutine(FirstResWait());
    }

    IEnumerator RetrySEtoSTART()
    {
        SE[1].Play();
        yield return new WaitForSeconds(SE[1].clip.length);
        ReTry();
    }

    public void OnYourMarkSet_GO()
    {
        if (setCalibrationFlag && getInclineFlag)
        {
            StartCoroutine(FirstPanelOFFSE());
        }
        else
        {
            if (STARTButoon.interactable)
            {
                StartCoroutine(RetrySEtoSTART());
            }
        }
    }

    private void firstPositionLoad()
    {
        stageName = (Script_StageSelectManager.SelectedStageName == null) ? DebugStageName : Script_StageSelectManager.SelectedStageName;
        routeJsonFilePath = Path.Combine(Application.persistentDataPath, "Player_Data", Script_PlayerSelectManager.SelectedPlayerSlot + "_PlayerData", "Route", "Route_" + stageName, "Route" + DataSelectManager.SelectedRouteSlot + ".json");
        if (!File.Exists(routeJsonFilePath)) return;
        string jsonText = File.ReadAllText(routeJsonFilePath);
        MatchCollection matches = Regex.Matches(jsonText, @"\(([^)]+)\)");
        int count = 0;
        foreach (Match match in matches)
        {
            string[] parts = match.Groups[1].Value.Split(',');
            positionList.Add(new Vector3(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2])));
            if (count == 0) startPos = positionList[0];
            if (count == 1) endPos = positionList[1];
            count++;
        }
    }

    private void positionUpdate()
    {
        passWayPointCount++;
        if (passWayPointCount >= positionList.Count)
        {
            goalStatus = true;
            return;
        }
        startPos = endPos;
        endPos = positionList[passWayPointCount];
        updateStatus = false;
        lineRenderer.lineIndexUpdate();
    }

    IEnumerator GetSpeedFromFlask()
    {
        //走行中またはゴール後もデータ取得を続ける
        while (runningStatus || goalStatus)
        {
            using (UnityWebRequest getSpeedReq = UnityWebRequest.Get(GetSpeedURL))
            {
                yield return getSpeedReq.SendWebRequest();
                if (getSpeedReq.result == UnityWebRequest.Result.Success)
                {
                    if (ignoreFirstSpeedReading)
                    {
                        ignoreFirstSpeedReading = false;
                    }
                    else
                    {
                        //走行中もゴール後も、取得した速度をspeed変数に反映する
                        speed = float.Parse(getSpeedReq.downloadHandler.text);
                    }
                    isGetSpeed = true;
                }
                else
                {
                    isGetSpeed = false;
                }
            }
            yield return new WaitForSeconds(getSpeedInterbal);
        }
    }

    //走行中に定期的に傾斜を取得するコルーチン
    IEnumerator GetInclineFromFlask()
    {
        //走行中またはゴール後もデータ取得を続ける
        while (runningStatus || goalStatus)
        {
            //サーバーにGETリクエストを送信する
            using (UnityWebRequest getInclineReq = UnityWebRequest.Get(GetInclineURL))
            {
                //応答を待つ
                yield return getInclineReq.SendWebRequest();
                //通信に成功した場合
                if (getInclineReq.result == UnityWebRequest.Result.Success)
                {
                    //サーバーからの応答をfloat型に変換する
                    float inclineValue;
                    //安全な変換を試みる
                    if (float.TryParse(getInclineReq.downloadHandler.text, out inclineValue))
                    {
                        //取得した値を最も近い5の倍数に丸める
                        float roundedIncline = Mathf.Round(inclineValue / 5.0f) * 5.0f;
                        //傾斜が取得できているのでフラグをtrueにする
                        isGetIncline = true;
                        //UIテキストを更新する
                        inclineText.GetComponentInChildren<TMP_Text>().text = roundedIncline.ToString("F0") + "°";
                    }
                    else
                    {
                        //変換に失敗した場合（"None"など）
                        isGetIncline = false;
                        //表示を0°にする
                        inclineText.GetComponentInChildren<TMP_Text>().text = "0°";
                    }
                }
                //通信に失敗した場合
                else
                {
                    //エラーをログに出力する
                    Debug.LogError("傾斜の値の取得に失敗：" + getInclineReq.error);
                    //傾斜が取得できていないのでフラグをfalseにする
                    isGetIncline = false;
                    //表示を0°にする
                    inclineText.GetComponentInChildren<TMP_Text>().text = "0°";
                }
            }
            //指定した間隔だけ待機する
            yield return new WaitForSeconds(getInclineInterbal);
        }
    }

    IEnumerator EndSpeedFromFlask()
    {
        string url = "http://" + config.ipaddress + ":" + config.port + "/stop_rotary_speed";
        using (UnityWebRequest endSpeedReq = UnityWebRequest.Post(url, new WWWForm()))
        {
            yield return endSpeedReq.SendWebRequest();
            if (endSpeedReq.result == UnityWebRequest.Result.Success) isGetSpeed = false;
        }
    }

    IEnumerator EndInclineFromFlask()
    {
        string url = "http://" + config.ipaddress + ":" + config.port + "/stop_mpu6050_get_pitch";
        using (UnityWebRequest endInclineReq = UnityWebRequest.Post(url, new WWWForm()))
        {
            yield return endInclineReq.SendWebRequest();
            if (endInclineReq.result == UnityWebRequest.Result.Success) isGetIncline = false;
        }
    }

    public void OnEnd()
    {
        if (isStopRunning) return;
        StartCoroutine(OnEndStopScreen());
    }

    IEnumerator OnEndStopScreen()
    {
        SE[1].Play();
        yield return new WaitForSeconds(SE[1].clip.length);
        if (pausePanel != null) pausePanel.SetActive(false);
        yield return StartCoroutine(StopScreen(StopReason.Manual));
    }

    /// 停止時にカウントダウンをしながら滑らかに画面を停止させる関数
    IEnumerator StopScreen(StopReason reason)
    {
        //複数回実行されないようにフラグで管理する
        if (isStopRunning) yield break;
        isStopRunning = true;

        //ゴール後のエラーの場合、最初にゴールパネルを非表示にする
        if (reason == StopReason.PostGoalError)
        {
            if (goalPanel != null) goalPanel.SetActive(false);
        }

        //走行状態とゴール状態を止める（これによりデータ取得ループが止まる）
        runningStatus = false;
        goalStatus = false;

        if (RunningErrorText != null && RunningErrorText.Length > 1)
        {
            RunningErrorText[0].SetActive(false);
            RunningErrorText[1].SetActive(false);
        }

        Debug.Log($"画面停止シーケンスを開始します... 理由: {reason}");

        //停止テキストを表示する（ゴール時以外）
        if (reason != StopReason.Goal && StopText != null)
        {
            StopText.SetActive(true);
        }
        //カウントダウンテキストを表示する
        if (StopCountDown != null) StopCountDown.SetActive(true);

        // カウントダウンUIを初期状態（数値表示用）にリセットする
        if (StopCountDown != null)
        {
            var countdownTMP = StopCountDown.GetComponentInChildren<TMP_Text>();
            if (countdownTMP != null)
            {
                //フォントサイズを大きい方に戻す
                countdownTMP.fontSize = 414.35f;
                //テキストを最初の秒数に設定
                countdownTMP.text = StopInterbal.ToString("F0");
            }
        }

        //3秒間待機する（エラー時と手動停止時のみ）
        if (reason == StopReason.ConnectionError || reason == StopReason.Manual || reason == StopReason.PostGoalError)
        {
            yield return new WaitForSeconds(3.0f);
        }

        //減速開始前の速度を記録する
        float startSpeed = speed;
        //傾斜表示を0にする
        inclineText.GetComponentInChildren<TMP_Text>().text = "0°";
        //経過時間を初期化する
        float elapsedTime = 0f;

        //カウントダウンと減速を同時に行う
        while (elapsedTime < StopInterbal)
        {
            //経過時間を加算する
            elapsedTime += Time.deltaTime;
            //残り時間を計算する
            float remainingTime = StopInterbal - elapsedTime;
            //カウントダウンUIを更新する
            if (StopCountDown != null)
            {
                StopCountDown.GetComponentInChildren<TMP_Text>().text = Mathf.CeilToInt(remainingTime).ToString();
            }
            //経過時間の割合(0.0~1.0)を計算する
            float t = elapsedTime / StopInterbal;
            //速度を滑らかに0に近づける
            speed = Mathf.Lerp(startSpeed, 0f, t);
            //計算した速度をカメラの物理挙動に反映させる
            CameraRb.linearVelocity = transform.forward * speed;
            //次のフレームまで待機する
            yield return null;
        }

        //停止をログに出力する
        Debug.Log("画面を停止しました");
        //速度と物理挙動を完全に0にする
        speed = 0f;
        CameraRb.linearVelocity = Vector3.zero;

        //すべての通信とコルーチンを停止する
        StopAllCoroutinesAndNotifyServer();
        //タイマーを止める
        if (reason != StopReason.Goal) //ゴール時はすでにタイマーが止まっているので、それ以外で止める
        {
            if (runningTimer != null) runningTimer.StopTimer();
        }

        //停止理由に応じた最終処理
        if (reason == StopReason.Goal)
        {
            //このルートはゴール処理がUpdateに移動したため、通常は通らない
        }
        else if (reason == StopReason.ConnectionError)
        {
            //通信エラーの場合：「R」リロードの表示に切り替えて待機
            Debug.Log("通信エラーのため停止しました。'R'キーで再接続を試みてください。");
            //警告UIにメッセージを表示する
            if (StopText != null) StopText.SetActive(true);
            if (StopCountDown != null)
            {
                var countdownTMP = StopCountDown.GetComponentInChildren<TMP_Text>();
                if (countdownTMP != null)
                {
                    countdownTMP.fontSize = 150f;
                    countdownTMP.text = "「R」\nリロード";
                }
            }
            //エラー停止状態フラグを立てる
            isErrorStopped = true;
        }
        else if (reason == StopReason.PostGoalError)
        {
            //ゴール後のエラーの場合
            Debug.Log("クールダウン中に通信エラーが発生しました。");
            //停止UIは非表示にする
            if (StopCountDown != null) StopCountDown.SetActive(false);
            if (StopText != null) StopText.SetActive(false);
            //停止後にゴールパネルを再表示する
            if (goalPanel != null) goalPanel.SetActive(true);
            //Rキーでのリロードは無効のまま（isErrorStoppedをtrueにしない）
        }
        else if (reason == StopReason.Manual)
        {
            //手動停止の場合は、指定された次のシーンに遷移する
            SceneManager.LoadScene(nextScene);
        }
    }
    IEnumerator AttemptReconnection()
    {
        Debug.Log("再接続処理を開始します...");
        Coroutine calibCoroutine = StartCoroutine(SetCalibrationToFlask(targetSpeed, calibrationStep));
        Coroutine inclineCoroutine = StartCoroutine(CheckInclineFromFlask());
        yield return calibCoroutine;
        yield return inclineCoroutine;

        if (setCalibrationFlag && getInclineFlag)
        {
            Debug.Log("再接続に成功しました。");
            StartErrorText[0].SetActive(false);
            StartErrorText[1].SetActive(false);
            if (isStopRunning)
            {
                Debug.Log("走行を再開します。");
                StartCoroutine(RestartCountdown());
            }
            else
            {
                STARTButoon.interactable = true;
                Debug.Log("スタート準備が完了しました。");
            }
        }
        else
        {
            Debug.Log("再接続に失敗しました。");
            if (isStopRunning)
            {
                isErrorStopped = true;
                if (StopText != null) StopText.SetActive(true);
                if (StopCountDown != null)
                {
                    StopCountDown.SetActive(true);
                    var countdownTMP = StopCountDown.GetComponentInChildren<TMP_Text>();
                    if (countdownTMP != null)
                    {
                        countdownTMP.fontSize = 150f;
                        countdownTMP.text = "「R」\nリロード";
                    }
                }
            }
            else
            {
                STARTButoon.interactable = true;
                Debug.Log("スタート前の接続に失敗しました。RETRYボタンで再試行してください。");
            }
        }
    }

    IEnumerator RestartCountdown()
    {
        if (reloadCountDown != null) reloadCountDown.SetActive(true);
        for (float t = reloadInterbal; t > 0; t -= 1.0f)
        {
            if (reloadCountDown != null)
            {
                reloadCountDown.GetComponentInChildren<TMP_Text>().text = Mathf.CeilToInt(t).ToString();
            }
            yield return new WaitForSeconds(1.0f);
        }
        if (reloadCountDown != null) reloadCountDown.SetActive(false);

        isStopRunning = false;
        runningStatus = true;
        isGetSpeed = true;
        isGetIncline = true;
        ignoreFirstSpeedReading = true;

        if (!DebugMode)
        {
            yield return StartCoroutine(SetCalibrationToFlask(targetSpeed, calibrationStep));
            if (setCalibrationFlag)
            {
                getSpeedCoroutine = StartCoroutine(GetSpeedFromFlask());
                getInclineCoroutine = StartCoroutine(GetInclineFromFlask());
            }
        }
    }

    private void StopAllCoroutinesAndNotifyServer()
    {
        StartCoroutine(EndSpeedFromFlask());
        StartCoroutine(EndInclineFromFlask());
        if (getSpeedCoroutine != null)
        {
            StopCoroutine(getSpeedCoroutine);
            getSpeedCoroutine = null;
        }
        if (getInclineCoroutine != null)
        {
            StopCoroutine(getInclineCoroutine);
            getInclineCoroutine = null;
        }
    }

    IEnumerator FirstResWait()
    {
        yield return new WaitForSeconds(0.5f);
        firstGetWait = true;
    }
}