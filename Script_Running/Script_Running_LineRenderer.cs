
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Running_LineRenderer : MonoBehaviour
{
    //LineRenderer用変数
    private LineRenderer lineRenderer;

    [Header("線の更新にカメラが必要なのでカメラ")]
    [SerializeField] private Camera Camera;

    private void Awake()
    {
        //このゲームオブジェクトのLineRendererコンポーネントを取得する
        //LineRendererは、複数の点（座標）を順番につないで線を描くコンポーネント
        lineRenderer = gameObject.GetComponent<LineRenderer>();

        //もしLineRendererコンポーネントがなかったら、新たに追加する
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
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

        //線の角を丸める
        lineRenderer.numCapVertices = 10;

        //頂点を丸める
        lineRenderer.numCornerVertices = 10;
    }

    //この関数を「Script_RunningStartLoad.cs」内の「StartLoad」内で実行する
    public void StartLoad_LineRender(List<GameObject> wayPoints)
    {
        //ウェイポイントを線で結ぶ
        if (wayPoints.Count < 2)
        {
            //2点未満なら線なし
            lineRenderer.positionCount = 0;
            return;
        }

        //線を書くのに使う点の数はウェイポイントの数-1であるので、リストのインデックス数と同じである
        lineRenderer.positionCount = wayPoints.Count;
        for (int i = 0; i < wayPoints.Count; i++)
        {

            //いったんまとめる
            Vector3 pos = new Vector3(wayPoints[i].transform.position.x,
                wayPoints[i].transform.position.y, wayPoints[i].transform.position.z);

            //SetPosition(何番目の点を設定するか,設定する座標)
            lineRenderer.SetPosition(i, pos);
        }
    }

    //毎フレーム更新して通ったラインをどんどん短くする
    //基本的にLineRendererは１本の線なのでそれをどんどん短くしていくイメージ
    //実際の実装方法はカメラの位置を線の始点に置き換えていく
    private void Update()
    {
        //まずはカメラの座標をもとにしているので
        //高さだけを「Script_StartLoad.cs」のウェイポイントの高さに合わせる
        Vector3 pos = new Vector3(Camera.transform.position.x, Camera.transform.position.y-1.0f,
            Camera.transform.position.z);

        //ここで始点をカメラ位置にする
        lineRenderer.SetPosition(0, pos);
    }

    //これを「Script_RunningCameraManager.cs」でウェイポイントにたどり着いたときに
    //呼び出して線の頂点を更新する
    //これによってウェイポイントに達しても始点を常にカメラの位置に更新できる
    public void lineIndexUpdate()
    {
        // 今のlineRendererの頂点数（positionCount）を取得する
        int count = lineRenderer.positionCount;

        //先頭から何個詰めるか（今回は１個）
        int removeCount = 1;

        // 削除数が現在の頂点数以上なら、線を消しておわり
        if (removeCount >= count)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        // 新しい頂点配列を用意する（古い頂点数からremoveCountを引いた長さ）
        Vector3[] newPositions = new Vector3[count - removeCount];

        //oldPositionsの（i+removeCount）をnewPositions[i]に詰める
        //つまり、先頭removeCount個ぶんを飛ばして残りを前に詰める処理
        for (int i = 0; i < newPositions.Length; i++)
        {
            newPositions[i] = lineRenderer.GetPosition(i + removeCount);
        }

        //lineRendererの頂点数を新しい長さに設定する
        lineRenderer.positionCount = newPositions.Length;

        //配列で一括設定
        lineRenderer.SetPositions(newPositions);
    }

}
