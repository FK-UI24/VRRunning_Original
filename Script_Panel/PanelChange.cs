using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class PanelChange : MonoBehaviour
{
    //同一シーン内でのパネル切替

    AudioSource SE;

    //インスペクター側で設定する「表示したいパネル」用変数
    //ボタンを押したときに表示するパネルを指定する
    [Header("切り替えたいパネル（表示したいパネル）")]
    [SerializeField] private GameObject nextPanel;

    //インスペクター側で設定する「パネルの親オブジェクト」用変数
    //Canvasなどの親オブジェクトを指定して、その子オブジェクトである複数のパネルを管理する
    //Transformである理由...Transformは位置,回転,スケールだけでなく親子関係を管理しているため
    [Header("すべてのパネルが入っている親オブジェクト（直下にパネル以外が入っているのはダメ）")]
    [SerializeField] private Transform panelParent;

    void Start()
    {
        //同じオブジェクトにアタッチされているSEを取得する
        SE = GetComponent<AudioSource>();
    }

    void Update()
    {
        
    }

    //ボタンが押されたとき
    public void SwitchPanel()
    {
        StartCoroutine(SEtoSwitch());
    }

    IEnumerator SEtoSwitch()
    {
        //SEを鳴らして、なり終わるまで待つ
        SE.Play();
        yield return new WaitForSeconds(SE.clip.length);
        
        //panelParentの中身を１つずつpanelに格納してループする
        foreach(Transform panel in panelParent)
        {
            //panel(Transform).gameobjectって逆では？
            //これはTransformの上位のgameobjectを参照している
            //つまり上位→下位ではなく下位→上位で参照しているということ
            if (panel.gameObject == nextPanel)
            {
                panel.gameObject.SetActive(true);
            }

            else {
                panel.gameObject.SetActive(false);
            }
        }
    }
}
