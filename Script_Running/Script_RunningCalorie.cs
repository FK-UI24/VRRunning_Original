using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class Script_RunningCalorie : MonoBehaviour
{
    [Header("現在の消費カロリーを表示するテキスト")]
    [SerializeField] private GameObject calorieText;

    [Header("速度表示テキスト")]
    [SerializeField] private GameObject speedText;

    [Header("傾斜表示テキスト")]
    [SerializeField] private GameObject inclineText;

    [Header("カメラ")]
    [SerializeField] private GameObject cameraObject;

    //総カロリー
    private float totalCarorie = 0f;

    //プレイヤーの体重
    private float weight;

    private void Start()
    {
        //まずはパスを作る
        string path = Path.Combine(Application.persistentDataPath, "Player_Data",
            Script_PlayerSelectManager.SelectedPlayerSlot + "_PlayerData",
            "Basic_Information.json");
        //もしファイルがなかったら適当な体重を入れる
        //これが起きるのは開発時だけだから特に問題なし
        if (!File.Exists(path))
        {
            weight = 60;
            Debug.Log("体重データがないので60kgとする");
        }
        else
        {
            string json=File.ReadAllText(path);
            JObject obj=JObject.Parse(json);
            weight = (float)obj["weight"];
            Debug.Log("体重は" + weight + "kg");
        }
    }

    private void Update()
    {
        if (cameraObject.GetComponent<Script_RunningCameraManager>().runningStatus)
        {
            //今表示されている速度をfloatにする
            float speed = 
                float.Parse(speedText.GetComponent<TMP_Text>().text);
            //今表示されている傾斜から"°"を覗いてfloatにする
            float incline =
                float.Parse(inclineText.GetComponent<TMP_Text>().text.Replace("°", ""));

            //速度から基礎METsを取得する
            float baseMETs = MetsList(speed);

            //傾斜から追加METsを取得する
            float inclineBonusMETs = GetInclineBonusMets(speed, incline);

            //合計METsを計算する
            float totalMETs = baseMETs + inclineBonusMETs;

            //今のフレームで消費したカロリー計算をする
            //消費カロリー(kcal)=METs*体重(kg)*運動時間*1.05
            //運動時間(h)=deltaTime(秒)/3600
            float frameCalorie = totalMETs * weight * (Time.deltaTime / 3600f) * 1.05f;

            //総消費カロリーに加算する
            totalCarorie += frameCalorie;

            //テキストを更新する
            calorieText.GetComponent<TMP_Text>().text= totalCarorie.ToString("F1")+"kcal";
        }
    }

    private float GetInclineBonusMets(float currentSpeedKmh, float inclineDegrees)
    {
        //動いていない、または下り坂の場合は追加METsは0
        if (currentSpeedKmh <= 0.1f || inclineDegrees <= 0)
        {
            return 0f;
        }

        //速度を 時速(km/h) から 分速(m/min) に変換する
        float speedMetersPerMinute = currentSpeedKmh * 1000f / 60f;
        //傾斜を 角度(°) から 勾配(%)の小数表現に変換する
        float grade = Mathf.Tan(inclineDegrees * Mathf.Deg2Rad);

        //傾斜によって追加される酸素摂取量(VO2)を計算
        float inclineVo2;
        if (currentSpeedKmh < 5.0f)
        {
            //歩行時の傾斜成分
            inclineVo2 = 1.8f * speedMetersPerMinute * grade;
        }
        else
        {
            //走行時の傾斜成分
            inclineVo2 = 0.9f * speedMetersPerMinute * grade;
        }

        //追加の酸素摂取量をMETsに変換して返す (追加VO2 / 3.5)
        return inclineVo2 / 3.5f;
    }

    //速度とMETsの対応を片っ端から準備する
    //範囲外の箇所とかは線形補完を使っておおよその値を返すようにしている
    private float MetsList(float currentSpeedKmh)
    {
        // METs表のデータに基づく分岐処理
        if (currentSpeedKmh <= 0.5) return 0.0f;
        if (currentSpeedKmh <= 1.6f) return 2.0f;
        if (currentSpeedKmh < 4.2f) return LinearInterpolate(currentSpeedKmh, 1.6f, 2.0f, 4.2f, 3.3f);
        if (currentSpeedKmh <= 6.0f) return 3.3f;
        if (currentSpeedKmh < 6.5f) return LinearInterpolate(currentSpeedKmh, 6.0f, 3.3f, 6.5f, 6.5f);
        if (currentSpeedKmh <= 6.8f) return 6.5f;
        if (currentSpeedKmh < 7.0f) return LinearInterpolate(currentSpeedKmh, 6.8f, 6.5f, 7.0f, 7.8f);
        if (currentSpeedKmh <= 7.8f) return 7.8f;
        if (currentSpeedKmh < 8.1f) return LinearInterpolate(currentSpeedKmh, 7.8f, 7.8f, 8.1f, 8.5f);
        if (currentSpeedKmh <= 8.4f) return 8.5f;
        if (currentSpeedKmh < 8.9f) return LinearInterpolate(currentSpeedKmh, 8.4f, 8.5f, 8.9f, 9.0f);
        if (currentSpeedKmh <= 9.4f) return 9.0f;
        if (currentSpeedKmh < 9.7f) return LinearInterpolate(currentSpeedKmh, 9.4f, 9.0f, 9.7f, 9.3f);
        if (currentSpeedKmh <= 10.2f) return 9.3f;
        if (currentSpeedKmh < 10.9f) return LinearInterpolate(currentSpeedKmh, 10.2f, 9.3f, 10.9f, 10.5f);
        if (currentSpeedKmh < 11.3f) return LinearInterpolate(currentSpeedKmh, 10.9f, 10.5f, 11.3f, 11.0f);
        if (currentSpeedKmh < 12.2f) return LinearInterpolate(currentSpeedKmh, 11.3f, 11.0f, 12.2f, 11.8f);
        if (currentSpeedKmh < 13.0f) return LinearInterpolate(currentSpeedKmh, 12.2f, 11.8f, 13.0f, 12.0f);
        if (currentSpeedKmh < 13.9f) return LinearInterpolate(currentSpeedKmh, 13.0f, 12.0f, 13.9f, 12.5f);
        if (currentSpeedKmh < 14.6f) return LinearInterpolate(currentSpeedKmh, 13.9f, 12.5f, 14.6f, 13.0f);
        if (currentSpeedKmh < 15.1f) return LinearInterpolate(currentSpeedKmh, 14.6f, 13.0f, 15.1f, 14.8f);
        if (currentSpeedKmh <= 15.6f) return 14.8f;
        if (currentSpeedKmh < 16.2f) return LinearInterpolate(currentSpeedKmh, 15.6f, 14.8f, 16.2f, 14.8f); 
        if (currentSpeedKmh < 17.8f) return LinearInterpolate(currentSpeedKmh, 16.2f, 14.8f, 17.8f, 16.8f);
        if (currentSpeedKmh < 19.4f) return LinearInterpolate(currentSpeedKmh, 17.8f, 16.8f, 19.4f, 18.5f);
        if (currentSpeedKmh < 21.1f) return LinearInterpolate(currentSpeedKmh, 19.4f, 18.5f, 21.1f, 19.8f);
        if (currentSpeedKmh < 22.7f) return LinearInterpolate(currentSpeedKmh, 21.1f, 19.8f, 22.7f, 23.0f);

        // METs表の上限を超える場合は、最高値で固定
        return 23.0f;
    }

    //2点間の値を線形補間するための補助関数
    //求めたい点のX座標 (現在の速度)
    //始点のX座標 (区間の下の速度)
    //始点のY座標 (区間の下のMETs)
    //終点のX座標 (区間の上の速度)
    //終点のY座標 (区間の上のMETs)
    //補間されたY座標 (計算されたMETs)
    private float LinearInterpolate(float x, float x0, float y0, float x1, float y1)
    {
        // 0除算を防ぐ
        if ((x1 - x0) == 0)
        {
            return y0;
        }
        // 線形補間の計算式
        return y0 + (x - x0) * (y1 - y0) / (x1 - x0);
    }

}
