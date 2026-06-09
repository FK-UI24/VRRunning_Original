using Newtonsoft.Json;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReadyManagement : MonoBehaviour
{
    [Header("ステージ名表示用テキスト")]
    [SerializeField] private GameObject stageText;
    [Header("ルート番号表示用テキスト")]
    [SerializeField] private GameObject RouteNumText;
    [Header("ウェイポイント数を表示する用変数")]
    [SerializeField] private GameObject wayPointText;

    [Header("１秒間に回転する角度スライダー")]
    [SerializeField] private Slider SmoothSlider;
    [Header("角度表示用テキスト")]
    [SerializeField] private GameObject SmoothText;


    //トグルの結果を入れる変数
    public static bool UseBGM = false;
    public static bool UseCenterMarkar = false;
    public static bool UseFrameMarkar = false;
    public static bool UseSmooth = false;

    //１秒間に回転する角度を入れる変数
    public static int SmoothValue = 130;

    void Start()
    {
        stageText.GetComponentInChildren<TMP_Text>().text = Script_StageSelectManager.SelectedStageName;

        RouteNumText.GetComponentInChildren<TMP_Text>().text = "-Route " + DataSelectManager.SelectedRouteSlot + "-";

        string FilePath = Path.Combine(Application.persistentDataPath,
        "Player_Data", Script_PlayerSelectManager.SelectedPlayerSlot + "_PlayerData",
        "Route", "Route_" + Script_StageSelectManager.SelectedStageName,
        "Route" + DataSelectManager.SelectedRouteSlot + ".json");
        string json = File.ReadAllText(FilePath);
        string[] waypoints = JsonConvert.DeserializeObject<string[]>(json);
        wayPointText.GetComponentInChildren<TMP_Text>().text = "ウェイポイント数：" + waypoints.Length.ToString();

        SmoothText.GetComponentInChildren<TMP_Text>().text = ((int)SmoothSlider.value).ToString();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBGMToggle()
    {
        UseBGM = !UseBGM;
    }
    public void OnCenterMarkarToggle()
    {
        UseCenterMarkar = !UseCenterMarkar;
    }
    public void OnFrameToggle()
    {
        UseFrameMarkar = !UseFrameMarkar;
    }
    public void OnSmoothToggle()
    {
        UseSmooth = !UseSmooth;
    }

    public void changeSmoothSlider()
    {
        SmoothValue = (int)SmoothSlider.value;
        SmoothText.GetComponentInChildren<TMP_Text>().text = SmoothValue.ToString();
    }

    public void OnStart()
    {
        Debug.Log("\nBGM:" + UseBGM + "\nCenter:" + UseCenterMarkar + "\nFrame:" + UseFrameMarkar + "\nSmooth:" + UseSmooth);
    }
}
