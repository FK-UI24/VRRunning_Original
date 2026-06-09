using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_MapCameraManager : MonoBehaviour
{
    [Header("メインカメラ")]
    [SerializeField] private GameObject MainCamera;

    [Header("マップカメラの高さ")]
    [SerializeField] private float height;

    private void Update()
    {
        //座標は高さ以外すべてメインカメラと合わせる
        //高さだけは調整できるようにする
        transform.position = new Vector3(MainCamera.transform.position.x,
            MainCamera.transform.position.y + height, MainCamera.transform.position.z);

        //マップカメラの角度はxは常に下向きでyはメインカメラと合わせる
        transform.rotation = Quaternion.Euler(90f, MainCamera.transform.eulerAngles.y, 0f);
    }

}
