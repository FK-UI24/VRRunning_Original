using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    //VideoPlayer用変数
    VideoPlayer video;

    //ボタンのサイズなどを決めるRectTransform用変数
    RectTransform Button_Size;

    //オリジナルのボタンサイズを扱う変数
    //Vector2とは2次元の漠とるを表す構造体（struct）である
    //2つのfloat値、xとyを持つ
    //Unityでは、位置、サイズ、速度、方向などの2Dの情報を扱うのに使用する
    Vector2 originalButton_Size;

    //インスペクター側で倍率設定する用変数
    public float BigButton;

    // Start is called before the first frame update
    void Start()
    {
        video = GetComponent<VideoPlayer>();

        if (video != null)
        {
            video.Play();
            video.Pause();
            video.frame = 0;
        }

        Button_Size = GetComponent<RectTransform>();


        //ボタンのオリジナルのサイズを格納
        //sizeDeltaとは、UnityのUI要素（RectTransform）の「幅と高さを直接指定するプロパティ」である
        originalButton_Size = Button_Size.sizeDelta;

    }

    //sizeDeltaとVector2の関係性
    //sizeDeltaはVector2型のプロパティ（変数みたいなもの）である



    //カーソルが乗っているときに、ビデオの再生、ボタンのサイズをオリジナルの1.1倍にする
    public void OnPointerEnter(PointerEventData eventdata)
    {
        Debug.Log("Ranningボタンにマウスが乗っている");
        video.Play();
        Button_Size.sizeDelta = originalButton_Size * BigButton;
    }

    //カーソルが乗っていないときに、ビデオの停止と最初から、ボタンのサイズをオリジナルのサイズにする
    public void OnPointerExit(PointerEventData eventdata)
    {
        Debug.Log("Ranningボタンからマウスが消えた");
        video.time = 0;
        video.Stop();
        Button_Size.sizeDelta = originalButton_Size;
    }


}
