using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;

public class Script_RunningStartLoad : MonoBehaviour
{
    [Header("視点用のカメラオブジェクト")]
    [SerializeField] private GameObject Camera;

    [Header("ウェイポイントのオブジェクト")]
    [SerializeField] private GameObject wayPointObject;

    [Header("ラインレンダラー用の関数")]
    [SerializeField] private Script_Running_LineRenderer sc_lineRenderer;

    [Header("開発用ステージ名（もし前のシーンでデータが設定されていなかったらこれになる）")]
    [SerializeField] private string DebugStageName;

    //ルートファイルまでのパスを格納する用変数
    private string RouteJsonFile;

    //ウェイポイントオブジェクトを格納する用リスト
    //座標を保存するリストと対応させるために使用する
    private List<GameObject> wayPoints = new List<GameObject>();

    //ステージ名を格納する用変数
    private string stageName;


    void Awake()
    {
        StartLoad();
    }

    //Start関数で呼び出すロード用関数
    //ここでカメラをウェイポイントの１番に設定する
    //カメラの向きは2番目のウェイポイントにする
    //ウェイポイントオブジェクトを設置する
    //すべてのウェイポイントをつなぐ線を表示する
    void StartLoad()
    {
        if (Script_StageSelectManager.SelectedStageName == null)
        {
            Debug.Log("開発用ステージ名を使用");
            stageName = DebugStageName;
        }
        else
        {
            stageName = Script_StageSelectManager.SelectedStageName;
        }

            //ルートファイルまでのパスを格納する
            RouteJsonFile = Path.Combine(Application.persistentDataPath,
                "Player_Data", Script_PlayerSelectManager.SelectedPlayerSlot + "_PlayerData",
                "Route", "Route_" + stageName, "Route" + DataSelectManager.SelectedRouteSlot + ".json");

        //ファイルがあるかを確認する
        //順当にいけば、ファイルはあるので特にエラー文の表示などはしない
        if (!File.Exists(RouteJsonFile))
        {
            return;
        }

        //ファイルの中身を文字列として読み込む
        string jsonText=File.ReadAllText(RouteJsonFile);

        //正規表現で"(x,y,z)"の形を全部見つける
        MatchCollection matches = Regex.Matches(jsonText, @"\(([^)]+)\)");

        //ウェイポイントオブジェクトに対応する番号を数える用変数
        int count = 0;

        //1番のウェイポイントの座標を保存する用変数
        Vector3 startPos = Vector3.zero;

        //2番のウェイポイントオブジェクトを保存する用変数
        GameObject secondWayPoint = null;

        foreach (Match match in matches)
        {
            //"x,y,z"の部分を取得する
            //match.Groups[0]...マッチ全体"(x,y,z)"
            //match.Groups[1]...丸括弧の中身だけ"x,y,z"
            string[] parts = match.Groups[1].Value.Split(',');

            //floatに変換する
            float x = float.Parse(parts[0]);
            float y = float.Parse(parts[1]);
            float z = float.Parse(parts[2]);

            //Vector3への変換とウェイポイントの高さを調整する
            //ここで高さを-1.0fしているのはウェイポイントの位置をカメラの少し下の高さに設置したいからである
            //しかしこのままだとposはカメラ事態の高さにも使用しているので結局カメラとウェイポイント両方の高さが低くなるだけになってしまう
            //そのためこの関数の下部のカメラの初期位置を決めているところで、ここでマイナスした分をプラスして調整している
            Vector3 pos = new Vector3(x, y-1.25f, z);

            //オブジェクト生成
            GameObject waypoint = Instantiate(wayPointObject, pos, Quaternion.identity);
            wayPoints.Add(waypoint);

            //ウェイポイントのカウントを数える
            count += 1;

            //１番だったら座標を保存する
            //またウェイポイントとぶつかると音が鳴るため最初の１個目(Start)ではならないようにする
            //設置したウェイポイントの当たり判定をなくして、その後表示しないようにする
            if (count == 1)
            {
                startPos = pos;

                //最初のウェイポイントの当たり判定をなくして表示させない処理
                Collider col = waypoint.GetComponent<Collider>();
                col.enabled = false;
                waypoint.SetActive(false);
            }
            else if (count == 2)
            {
                secondWayPoint = waypoint;
            }

        }

        //ここでラインレンダラーでウェイポイントを結ぶ関数を呼び出す
        sc_lineRenderer.StartLoad_LineRender(wayPoints);


        //ここでカメラの位置と向きを変える
        //ここでの高さ調整についてはこの関数内のposの格納個所を確認するとわかる
        Camera.transform.position = new Vector3(startPos.x, startPos.y+1.25f, startPos.z);

        //向く方向はx,zは２番目のウェイポイントでいいが、y（高さ）はカメラの高さにそろえる
        Vector3 lookTarget =
            new Vector3(secondWayPoint.transform.position.x, Camera.transform.position.y,
            secondWayPoint.transform.position.z);
        Camera.transform.LookAt(lookTarget);
    }

}
