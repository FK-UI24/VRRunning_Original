using UnityEngine;
using TMPro;

public class Script_RunningDistance_Scaled : MonoBehaviour
{
    [Header("計測対象オブジェクト")]
    [SerializeField] private GameObject targetObject;

    [Header("走行距離表示テキスト")]
    [SerializeField] private TMP_Text distanceText;

    //手動では7km/hで0.282になった（多少のずれあり）
    [Header("スケール補正係数\n増やすと距離の経過が速くなる\n減らすと距離の経過が遅くなる")]
    [SerializeField] private float scaleFactor = 1.0f;

    private Vector3 lastPosition;
    private float totalDistance;
    private bool isFirstUpdate = true;

    private void Start()
    {
        totalDistance = 0f;
    }

    private void Update()
    {
        //危険だからゴールした後も動き続けるようにする
        //しかし距離の更新はやめる
        if (targetObject.GetComponentInChildren<Script_RunningCameraManager>().runningStatus)
        {

            // 最初のフレームで初期位置を記録
            if (isFirstUpdate)
            {
                lastPosition = targetObject.transform.position;
                isFirstUpdate = false;
            }

            if (targetObject == null) return;

            Vector3 currentPosition = targetObject.transform.position;
            float distance = Vector3.Distance(currentPosition, lastPosition);

            // スケール補正係数を乗算して距離を補正
            totalDistance += distance * scaleFactor;

            // テキストを更新
            distanceText.text = totalDistance.ToString("F0") + "m";

            // 次のフレームのために位置を更新
            lastPosition = currentPosition;
        }
    }
}