using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class DataSelectManager : MonoBehaviour
{
    //選択されているプレイヤーナンバー格納用変数
    private int SelectedPlayerSlot;

    [Header("開発用プレイヤースロットナンバーを使用するかどうか")]
    [SerializeField] private bool useValue = false;

    [Header("開発用プレイヤースロットナンバー")]
    [SerializeField] private int DebugSlotNumber = 0;

    //外部から参照する用変数（現在選択中のスロット番号を保持）
    //何も決まっていないときは-1
    //シーンを切り替えても保持される
    //ただし再起動をしたり、自分で明示的に代入するとリセットされる
    public static int SelectedRouteSlot;


    void Start()
    {

        SelectedRouteSlot = -1;

        //ここで開発用プレイヤースロットナンバーか
        //既に選択されているプレイヤースロットナンバーのどちらか格納用変数に入れる
        if (useValue == false)
        {
            SelectedPlayerSlot = Script_PlayerSelectManager.SelectedPlayerSlot;
            Debug.Log("選択されたスロットナンバーを使用");
        }
        else
        {
            SelectedPlayerSlot = DebugSlotNumber;
            Debug.Log("デバッグ用スロットナンバーを使用");
        }
    }

    void Update()
    {
        
    }

    public void OnSlotSelected(int SelectedRouteSlotNumber)
    {
        //選択されたスロット番号を記録する
        SelectedRouteSlot = SelectedRouteSlotNumber;


        //0_PlayerDataフォルダまでのパスを作成する
        string PlayerDataForlder = Path.Combine(Application.persistentDataPath, "Player_Data", $"{SelectedPlayerSlot}_PlayerData");

        //0_PlayerDataフォルダがなかったらデバッグ文を表示
        //順当にくれば、ここで0_PlayerDataフォルダがないことはありえない
        if (!Directory.Exists(PlayerDataForlder))
        {
            Debug.Log(PlayerDataForlder + "は存在しない\nまたこの状況はデバッグ以外では発生しない");
        }
        else
        {
            Debug.Log(SelectedPlayerSlot + "_PlayerDataフォルダまで存在する");
        }

        //Routeフォルダまでのパスを作成する
        string RouteForder = Path.Combine(PlayerDataForlder, "Route");

        //Routeフォルダがあるかを確認する
        //なかったら作成する
        if (Directory.Exists(RouteForder))
        {
            Debug.Log("Routeフォルダは存在する");
        }
        else 
        { 
            Directory.CreateDirectory(RouteForder);
            Debug.Log("Routeフォルダを生成した");
        }

        //「Route_(ステージ名)」フォルダまでのパスを作成する
        string StageRouteForder = Path.Combine(RouteForder, "Route_"+ Script_StageSelectManager.SelectedStageName);

        //「Route_(ステージ名)」フォルダがあるかを確認する
        //なかったら作成する
        if (Directory.Exists(StageRouteForder))
        {
            Debug.Log("Route_" + Script_StageSelectManager.SelectedStageName + "フォルダは存在する");
        }
        else
        {
            Directory.CreateDirectory(StageRouteForder);
            Debug.Log("Route_" + Script_StageSelectManager.SelectedStageName + "フォルダを生成した");
        }

        //「Route(SelectedRouteSlotNumber).json」ファイルまでのパスを作成する
        string SelectedSlotNumberFile =
            Path.Combine(StageRouteForder, "Route" + SelectedRouteSlotNumber + ".json");

        //引数のスロット番号のルート.JSONファイルがあるかを確認する
        //なかったら作成する
        if (File.Exists(SelectedSlotNumberFile))
        {
            Debug.Log("Route" + SelectedRouteSlotNumber + ".jsonファイルは存在する");
        }
        else
        {
            File.WriteAllText(SelectedSlotNumberFile,"");
            Debug.Log("Route" + SelectedRouteSlotNumber + ".jsonファイルを生成した");
        }

        //最終的に選ばれたルートスロットを表示する
        Debug.Log("今選択されているルートスロットは" + SelectedRouteSlot + "番");
    }
}
