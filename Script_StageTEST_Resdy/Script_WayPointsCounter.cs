using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;

public class Script_WayPointsCounter : MonoBehaviour
{
    //選ばれたルートのウェイポイントの数を返す

    [Header("ウェイポイント数を表示する用変数")]
    [SerializeField] private TMP_Text wayPointsCounter;

    // Start is called before the first frame update
    void Start()
    {
        string FilePath = Path.Combine(Application.persistentDataPath,
                "Player_Data", Script_PlayerSelectManager.SelectedPlayerSlot + "_PlayerData",
                "Route", "Route_" + Script_StageSelectManager.SelectedStageName, 
                "Route" + DataSelectManager.SelectedRouteSlot + ".json");

        string json=File.ReadAllText(FilePath);
        string[] waypoints=JsonConvert.DeserializeObject<string[]>(json);

        wayPointsCounter.text="ウェイポイント数："+waypoints.Length.ToString();
    }
}
