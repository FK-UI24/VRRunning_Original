using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Script_CameraMove_Return : MonoBehaviour
{
    //特定の位置から特定の位置にカメラを移動させる

    //遷移元のカメラ位置
    [Header("遷移元のカメラ位置：x")]
    [SerializeField] private float SPx;
    [Header("遷移元のカメラ位置：y")]
    [SerializeField] private float SPy;
    [Header("遷移元のカメラ位置：z")]
    [SerializeField] private float SPz;

    [Header("遷移元のカメラ角度：x")]
    [SerializeField] private float SRx;
    [Header("遷移元のカメラ角度：y")]
    [SerializeField] private float SRy;
    [Header("遷移元のカメラ角度：z")]
    [SerializeField] private float SRz;



    //遷移先のカメラ位置
    [Header("遷移先のカメラ位置：x")]
    [SerializeField] private float GPx;
    [Header("遷移先のカメラ位置：y")]
    [SerializeField] private float GPy;
    [Header("遷移先のカメラ位置：z")]
    [SerializeField] private float GPz;

    [Header("遷移先のカメラ角度：x")]
    [SerializeField] private float GRx;
    [Header("遷移先のカメラ角度：y")]
    [SerializeField] private float GRy;
    [Header("遷移先のカメラ角度：z")]
    [SerializeField] private float GRz;

    [Header("何秒かけて移動させるか")]
    [SerializeField] private float moveDuration;

    private Vector3 startPos;
    private Vector3 goalPos;
    private Quaternion startRot;
    private Quaternion goalRot;

    private float elapsedTime = 0f;
    private bool isMoving = false;

    

    void Start()
    {
        
    }

    void Update()
    {
        if (isMoving)
        {
            //経過時間を加算
            elapsedTime += Time.deltaTime;

            //進行率（0～1）
            float t = Mathf.Clamp01(elapsedTime / moveDuration);

            //位置を補完
            transform.position = Vector3.Lerp(startPos, goalPos, t);

            //角度を補完
            transform.rotation = Quaternion.Slerp(startRot, goalRot, t);

            //移動完了判定
            if (t >= 1f)
            {
                isMoving = false;
            }
        }
    }

    //これを任意のタイミングで呼び出すと移動が始まる
    public void StartMove()
    {
        //Vector3とQuaternionに変換して格納する
        startPos = new Vector3(SPx, SPy, SPz);
        goalPos = new Vector3(GPx, GPy, GPz);

        startRot = Quaternion.Euler(SRx, SRy, SRz);
        goalRot = Quaternion.Euler(GRx, GRy, GRz);

        elapsedTime = 0f;
        isMoving = true;
    }
}
