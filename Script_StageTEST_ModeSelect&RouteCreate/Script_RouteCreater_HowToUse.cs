using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Script_RouteCreater_HowToUse : MonoBehaviour
{
    //これ自体は空のオブジェクトにアタッチする

    [Header("ルート作成パネル")]
    [SerializeField] private CanvasGroup RouteCreatepanels;

    [Header("使い方パネル")]
    [SerializeField] private GameObject HowPanel;

    [Header("カメラ")]
    [SerializeField] private GameObject Camera;

    //カメラにアタッチされているスクリプトを格納する変数
    private MonoBehaviour[] cameraMono;

    [Header("使い方の内容のゲームオブジェクト\n例：Text_HowToUse_Content_1")]
    [SerializeField] private List<GameObject> HowContentList = new List<GameObject>();

    [Header("ボタン用のSE")]
    [SerializeField] private AudioSource SE;

    [Header("次に進む用ボタン")]
    [SerializeField] private Button nextButton;

    [Header("前に戻る用ボタン")]
    [SerializeField] private Button prevButton;

    //HowToUseのContent数の最大値を格納する用変数
    private int MaxContentCount = 0;

    //今表示しているContent番号を格納する用変数
    private int NowContentCount;

    private void Start()
    {
        //内容数の最大値を格納する
        MaxContentCount = HowContentList.Count;

        //今表示しているContent番号を格納する
        NowContentCount = 0;

        //いったん内容は非表示にする
        foreach (GameObject g in HowContentList)
        {
            g.SetActive(false);
        }

        //最初の番号の内容を表示する
        HowContentList[0].SetActive(true);

        //カメラのスクリプトをまとめてリストに格納する
        cameraMono = Camera.GetComponents<MonoBehaviour>();

        //最初は非表示
        HowPanel.SetActive(false);

        //もし内容が１つだけなら最初から次へボタンを押せないようにする
        if (MaxContentCount == 1)
        {
            nextButton.interactable = false;
        }

        //最初は前に移るボタンを押せないようにする
        prevButton.interactable = false;


    }

    //押されたら使い方パネルを表示する関数を呼び出す用関数
    public void OnONHow()
    {
        StartCoroutine(C_OnONHow());
    }

    //呼ばれたら使い方パネルを表示する用関数
    IEnumerator C_OnONHow()
    {
        SE.Play();
        yield return new WaitForSeconds(SE.clip.length);
        //ルート作成パネルの無効化
        RouteCreatepanels.interactable = false;
        RouteCreatepanels.blocksRaycasts = false;
        RouteCreatepanels.alpha = 1.0f;

        //カメラの無効化
        foreach (MonoBehaviour m in cameraMono)
        {
            m.enabled = false;
        }

        //押されたら毎回内容番号と表示内容をリセットする
        NowContentCount = 0;
        //いったん内容は非表示にする
        foreach (GameObject g in HowContentList)
        {
            g.SetActive(false);
        }
        //もし内容が１つだけなら最初から次へボタンを押せないようにする
        if (MaxContentCount == 1)
        {
            nextButton.interactable = false;
        }
        else
        {
            nextButton.interactable=true;
        }

        //最初は前に移るボタンを押せないようにする
        prevButton.interactable = false;


        //最初の番号の内容を表示する
        HowContentList[0].SetActive(true);


        //使い方パネルの有効化
        HowPanel.SetActive(true);
    }


    //押されたら使い方パネルを非表示にする関数を呼び出す用関数
    public void OnOFFHow()
    {
        StartCoroutine (C_OnOFFHow());
    }

    //呼ばれたら使い方パネルを非表示にする用関数
    IEnumerator C_OnOFFHow()
    {
        SE.Play();
        yield return new WaitForSeconds(SE.clip.length);
        //ルート作成パネルの有効化
        RouteCreatepanels.interactable = true;
        RouteCreatepanels.blocksRaycasts = true;
        RouteCreatepanels.alpha = 1.0f;

        //カメラの有効化
        foreach (MonoBehaviour m in cameraMono)
        {
            m.enabled = true;
        }

        //使い方パネルの無効化
        HowPanel.SetActive(false);
    }


    //押されたら次の内容に移る関数を呼び出す用関数
    public void nextHow()
    {
        StartCoroutine(C_nextHow());
    }

    //呼ばれたら次の内容に移る用関数
    IEnumerator C_nextHow()
    {
        SE.Play();
        yield return new WaitForSeconds (SE.clip.length);


        //今表示している内容カウントを１増やす
        NowContentCount += 1;

        //カウントに対応する番号の内容を表示して、１つ前のは消す
        HowContentList[NowContentCount].SetActive(true);
        HowContentList[NowContentCount-1].SetActive(false);

        prevButton.interactable = true;


        //もしこの時点で最大なら次にボタンを押せなくする
        if (NowContentCount+1  == MaxContentCount)
        {
            nextButton.interactable = false;
        }
    }

    //押されたら前の内容に移る関数を呼び出す用関数
    public void prevHow()
    {
        StartCoroutine(C_prevHow());
    }

    //呼ばれたら前の内容に移る用関数
    IEnumerator C_prevHow()
    {
        SE.Play();
        yield return new WaitForSeconds (SE.clip.length);

        //今表示している内容カウントを１減らす
        NowContentCount -= 1;

        //カウントに対応する内容を表示して、１つ次のは消す
        HowContentList[NowContentCount].SetActive (true);　
        HowContentList[NowContentCount+1].SetActive(false);

        nextButton.interactable = true;


        //もしこの時点で０ならボタンを押せなくする
        if (NowContentCount == 0)
        {
            prevButton.interactable = false;
        }
    }


}
