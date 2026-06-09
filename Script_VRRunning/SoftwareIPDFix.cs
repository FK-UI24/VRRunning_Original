using System.Collections;
using UnityEngine;
/// <summary>
/// Quest 2 の最小IPD設定(58mm)よりも、ユーザーの実際のIPDが狭い場合に発生する
/// 映像のズレ（右目の見切れなど）をソフトウェア側で補正するスクリプト。
/// OVRManagerが毎フレーム TrackingSpace の位置をリセットするのを前提とし、
/// LateUpdate() で強制的に補正用のオフセット（ズレ）を上書きします。
/// </summary>
public class SoftwareIPDFix : MonoBehaviour
{
    [Header("目標の仮想IPD（メートル単位）")]
    [Tooltip("58mm未満の値を設定します (例: 56mm = 0.056)")]
    [SerializeField] private float desiredVirtualIPD = 0.056f; // 56mmを例として設定

    private Camera vrCamera;

    // 処理を軽くするため、待機オブジェクトをキャッシュする
    private WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

    void Start()
    {
        vrCamera = GetComponent<Camera>();
        if (vrCamera == null)
        {
            Debug.LogError("Cameraコンポーネントが見つかりません。" +
                           "このスクリプトは CenterEyeAnchor にアタッチしてください。", this);
            return;
        }

        // コルーチンを開始する
        StartCoroutine(ApplyIPDAtEndOfFrame());
    }

    private IEnumerator ApplyIPDAtEndOfFrame()
    {
        // このスクリプトが有効な間、ずっとループし続ける
        while (enabled)
        {
            // 全てのUpdate()とLateUpdate()が終わり、
            // カメラの描画が完了する直前まで待機する
            yield return waitForEndOfFrame;

            // 全ての処理の「最後」にIPDを上書きする
            vrCamera.stereoSeparation = desiredVirtualIPD;
        }
    }
}