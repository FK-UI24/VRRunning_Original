using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Text.RegularExpressions;
using TMPro;


//このスクリプトはメインカメラに付けるが、これを使用するシーンではパネルの切替で
//シーンのように見せているため、カメラが１つしかない状況では想定外の挙動を起こす
//そのため、ほかのスクリプトから特定のパネルになったときにオンにするようにする
//このスクリプト内には特定のボタンを押されることで開始する関数があるが
//今回の場合、そのボタンを表示・非表示を切り替えることで対応しているため
//そこは特に大丈夫(´∇`)bｸﾞｯ

public class Script_WayPointDataManagement : MonoBehaviour
{

    [Header("ウェイポイントのオブジェクト")]
    [SerializeField] private GameObject wayPointObject;

    //ウェイポイントオブジェクトを格納する用リスト
    //座標を保存するリストと対応させるために使用する
    private List<GameObject> wayPoints = new List<GameObject>();

    [Header("使用するステージ名の入力（フォルダとファイルの検索で使用する）")]
    [SerializeField] private string StageName;

    //一時的に座標を保存する用のリスト
    private List<Vector3> clickedPositions=new List<Vector3>();


    [Header("開発用プレイヤー番号とステージ名とファイル番号を使うか")]
    [SerializeField] private bool debugCheck;

    [Header("開発用プレイヤー番号")]
    [SerializeField] private int debugPlayerSlot;

    [Header("開発用ステージ名")]
    [SerializeField] private string debugStageName;

    [Header("開発用JSONファイル番号")]
    [SerializeField] private int debugJsonFileSlot;

    //レイヤーを分けないとクリックした箇所の右下あたりにウェイポイントが設置されてしまうようになった。
    //バージョンを変えたことの影響か？
    [Header("ウェイポイントを設置するオブジェクトのレイヤー名")]
    [SerializeField] private LayerMask layer;


    //0_PlayerDataフォルダまでのパスを格納する用変数
    string PlayerDataFolder;

    //Routeフォルダまでのパスを格納する用変数
    string RouteFolder;

    //「Route_(ステージ名)」フォルダまでのパスを格納する用変数
    string StageRouteFolder;

    //「Route0.json」ファイルまでのパスを格納する用変数
    string RouteJsonFile;

    //LineRenderer用変数
    private LineRenderer lineRenderer;

    //ウェイポイント設置時のSE用変数
    AudioSource SE;

    private void Awake()
    {
        //このゲームオブジェクトのLineRendererコンポーネントを取得する
        //LineRendererは、複数の点（座標）を順番につないで線を描くコンポーネント
        lineRenderer = gameObject.GetComponent<LineRenderer>();

        //もしLineRendererコンポーネントがなかったら、新たに追加する
        if (lineRenderer == null)
        {
            lineRenderer=gameObject.AddComponent<LineRenderer>();
        }

        //positionCountは線に使う点の数を指定するプロパティである
        lineRenderer.positionCount = 0;

        //線の太さを設定する（最初と最後で同じ幅にする）
        lineRenderer.startWidth = 0.5f;
        lineRenderer.endWidth = 0.5f;

        //マテリアルをStandardシェーダーで生成
        //これによりどこから見ても一定の幅に見える
        lineRenderer.material = new Material(Shader.Find("Standard"));

        //線の色を黄色にする
        lineRenderer.material.color = Color.yellow;

        //Emission（自己発光）を有効化して、くすんだ色にならないようにする
        lineRenderer.material.EnableKeyword("_EMISSION");

        //Emissionの色も黄色に設定する
        lineRenderer.material.SetColor("_EmissionColor", Color.yellow);

        //線による影の投影を無効化
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        //線がほかのオブジェクトから影の影響を受けないようにする
        lineRenderer.receiveShadows = false;

        //SEを格納する
        SE=GetComponent<AudioSource>();
    }

    void Start()
    {
        //開発用の変数を使う場合の一連のフォルダ,ファイルの確認と生成
        if (debugCheck == true)
        {
            PlayerDataFolder = Path.Combine(Application.persistentDataPath, "Player_Data", $"{debugPlayerSlot}_PlayerData");
            if (!Directory.Exists(PlayerDataFolder))
            {
                Directory.CreateDirectory(PlayerDataFolder);
            }

            RouteFolder = Path.Combine(PlayerDataFolder, "Route");
            if (!Directory.Exists(RouteFolder))
            {
                Directory.CreateDirectory(RouteFolder);
            }

            StageRouteFolder = Path.Combine(RouteFolder, "Route_" + debugStageName);
            if (!Directory.Exists(StageRouteFolder))
            {
                Directory.CreateDirectory(StageRouteFolder);
            }

            RouteJsonFile = Path.Combine(StageRouteFolder, "Route" + debugJsonFileSlot + ".json");
            if (!File.Exists(RouteJsonFile))
            {
                //ファイルだけ作成する
                //usingを使わないとファイルハンドルが解放されずに例外が起こる
                using (File.Create(RouteJsonFile)) { }
            }
        }

        //開発用の変数を使わない場合の一連のフォルダ,ファイルの確認と生成
        //順当にくれば、ここで「Route0.json」ファイルがないことはありえない
        else
        {
            RouteJsonFile = Path.Combine(Application.persistentDataPath,
                "Player_Data", Script_PlayerSelectManager.SelectedPlayerSlot + "_PlayerData",
                "Route", "Route_" + StageName, "Route" + DataSelectManager.SelectedRouteSlot + ".json");

            if (!File.Exists(RouteJsonFile))
            {
                Debug.LogError("これがでているということは、途中のシーンから始めたか...\n" +
                    "あるいはなにか変なことをしたか?\n" +
                    "それともSelectedRouteSlotが正しく参照できていないかだな...\n" +
                    "アタッチしたオブジェクトにステージ名いれた?");
            }
        }

        //最後にロードして前回の記録があれば引き継ぐ
        LoadPositionsFromJson();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ListSet();
        }
    }


    //リストに座標をセットする用の関数
    //これはカメラ側から呼び出す
    void ListSet()
    {
        //マウス位置からカメラに向かってRay（見えない線）を飛ばす
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //Raycast（線）にヒットした情報を格納するための変数
        RaycastHit hit;

        //Rayがオブジェクトに当たったかを判定する
        if (Physics.Raycast(ray, out hit,Mathf.Infinity,layer))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.red, 3f);
            Debug.Log($"Ray origin:{ray.origin} → hit:{hit.point}");

            //当たった場所の座標を取得する
            Vector3 clickPosition = hit.point;

            //clickPosiutionの座標を「リストに保存する」
            clickedPositions.Add(clickPosition);
            Debug.Log(clickPosition + "をリストに追加！！！");

            //ウェイポイントの高さを調整
            clickPosition.y += 2.5f;

            //選択したときのSEを鳴らす
            SE.Play();

            //インスペクター側から設定したオブジェクトを置く
            GameObject waypoint = Instantiate(wayPointObject, clickPosition, Quaternion.identity);

            //生成したクローンをリストに追加する
            wayPoints.Add(waypoint);

            //wayPointの子オブジェクトのTMPを取得して、リストに対応する番号を入れる
            TextMeshProUGUI waypointCounter =waypoint.GetComponentInChildren<TextMeshProUGUI>();
            waypointCounter.text = wayPoints.Count.ToString();

            //線の更新
            UpdateLineRenderer();


        }

    }

    //リストから最後の座標を削除する関数
    //これはボタンから間接的に呼び出す
    public void ListRemove()
    {
        //リストに保存されている座標が０個より多い場合
        if (clickedPositions.Count > 0)
        {
            //削除されるリストの要素を格納する
            Vector3 RemovePosition = clickedPositions[clickedPositions.Count - 1];

            //RemovAtで指定したインデックス番号の削除をする
            //clickedPositions.Countはリストの要素の数なので
            //そこから１を引くことで最新の要素を指定できる
            clickedPositions.RemoveAt(clickedPositions.Count - 1);
            Debug.Log(RemovePosition + "を削除した！！！");

            //対応するウェイポイントを削除する
            GameObject waypoint = wayPoints[wayPoints.Count - 1];
            wayPoints.RemoveAt(wayPoints.Count - 1);
            Destroy(waypoint);

            //線の更新
            UpdateLineRenderer();

        }
        //リストに１つも座標がないときの場合
        else
        {
            Debug.Log("削除する座標がないよ");
        }
    }

    //リストの内容を指定したJsonファイルに保存する関数
    //単純にキーなしで座標のみを保存していく
    //これはボタンから間接的に呼び出す
    public void SavePositionToJson()
    {
        //StringBuilderを使って効率的に文字列を組み立てる準備をする
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        //JSON配列の開始記号「[」を追加する
        //改行を入れて見やすくする
        sb.Append("[\n");

        //clickPositionsリストの要素を１つずつ取り出してループする
        for (int i = 0; i < clickedPositions.Count; i++)
        {
            Vector3 v=clickedPositions[i];

            //JSONの文字列配列の要素として座標を文字列化して、"(x,y,z)"の形式で書き込む
            //例：(1.0,2.0,3.0)のようになる
            //文字列は""で囲む必要があるので、$"..."の中に含める
            //ここで保存する座標を調整している
            //→クリックした場所の座標のままだと高さが0f基準になる
            //そのため+2.0fして高さを調整している
            sb.Append($"  \"({v.x},{v.y+2.5f},{v.z})\"");

            //最後の要素以外にはコンマと改行を追加してJSON配列の区切りにする
            if (i < clickedPositions.Count - 1)
            {
                sb.Append(",\n");
            }
        }

        //JSON配列の終了記号を追加する
        //改行も入れて整形する
        sb.Append("\n]");

        //完成したJSON文字列をファイルに完全上書きする
        //毎回完全上書きなので、
        //ロード関数を作って１番最初にJSONファイルの中身を取り出しておかないとリセットになる
        File.WriteAllText(RouteJsonFile, sb.ToString());

        Debug.Log("リストを" + RouteJsonFile + "に保存したよ");
    }

    //ロード用関数
    //Start関数の最後で実行する
    public void LoadPositionsFromJson()
    {
        RouteJsonFile = Path.Combine(Application.persistentDataPath,
            "Player_Data", Script_PlayerSelectManager.SelectedPlayerSlot + "_PlayerData",
            "Route", "Route_" + StageName, "Route" + DataSelectManager.SelectedRouteSlot + ".json");


        //既存の座標リストとオブジェクトリストとオブジェクトを念のためクリアする
        clickedPositions.Clear();
        foreach(GameObject obj in wayPoints)
        {
            Destroy(obj);
        }
        wayPoints.Clear();

        //JSONファイルの存在を確認する
        //順当に行けばファイルはある
        //あるいは開発用のインスペクター側からの設定がミスってる
        //これを使用するStart関数内で、これより前にその確認を行っているので
        //特にエラー文などは表示しない
        if (!File.Exists(RouteJsonFile))
        {
            return;
        }

        //ファイルの中身を文字列として読み込む
        string jsonText=File.ReadAllText(RouteJsonFile);

        // 正規表現で "(x,y,z)" の形を全部見つける
        MatchCollection matches = Regex.Matches(jsonText, @"\(([^)]+)\)");

        foreach (Match match in matches)
        {
            // "x,y,z" の部分を取得
            //match.Groups[0]...マッチ全体 "(x,y,z)"
            //match.Groups[1]...丸括弧の中身だけ "x,y,z"
            string[] parts = match.Groups[1].Value.Split(',');

            // floatに変換
            float x = float.Parse(parts[0]);
            float y = float.Parse(parts[1]);
            float z = float.Parse(parts[2]);

            //Vector3への変換とウェイポイントの高さを調整する
            Vector3 pos = new Vector3(x, y , z);

            // リストに追加
            clickedPositions.Add(pos);

            // オブジェクト生成
            GameObject waypoint = Instantiate(wayPointObject, pos, Quaternion.identity);
            wayPoints.Add(waypoint);


            //wayPointの子オブジェクトのTMPを取得して、リストに対応する番号を入れる
            TextMeshProUGUI waypointCounter = waypoint.GetComponentInChildren<TextMeshProUGUI>();
            waypointCounter.text = wayPoints.Count.ToString();


        }
        Debug.Log("座標のロード完了！！！");

        //線の更新
        UpdateLineRenderer();

    }

    //LineRendererを更新してウェイポイントに順番につなぐ
    private void UpdateLineRenderer()
    {
        if (wayPoints.Count < 2)
        {
            //2点未満なら線なし
            lineRenderer.positionCount = 0;
            return;
        }

        //線を書くのに使う点の数はウェイポイントの数-1であるので、リストのインデックス数と同じである
        lineRenderer.positionCount=wayPoints.Count;
        for(int i = 0; i < wayPoints.Count; i++)
        {
            //SetPosition(何番目の点を設定するか,設定する座標)
            lineRenderer.SetPosition(i, wayPoints[i].transform.position);
        }
    }
}
