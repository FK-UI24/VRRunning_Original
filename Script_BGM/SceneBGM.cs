using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneBGM : MonoBehaviour
{

    //BGMを再生・管理して
    //シーンをまたいでも音楽が止まらないかつ、同じBGMが重複再生されないように制御する

    //書くBGMを一意に識別するためのID
    //MainBGM→Mainシーン、BossBGM→ボス戦用
    [Header("BGMの識別子（例：MainBGM,BossBGMなど）")]
    [SerializeField] private string BGMID;

    //このBGMを再生するシーン名の一覧（インスペクター側で設定）
    //このリストに含まれないシーンに遷移したらBGMを止める
    [Header("BGMを再生するシーン一覧")]
    [SerializeField] private List<string> allowedScene=new List<string>();

    //BGMの再生状態を管理する性的セット（アプリ全体で共有）
    //既に再生中のBGMのIDを記録する
    //同じBGM（同じID）を複数シーンで使いまわすときに、重複再生を防止する
    private static HashSet<string> playingBGMIDs=new HashSet<string>();

    private void Awake()
    {
        //BGMが既に再生中であれば、この新しいインスタンスは不要なので削除
        if (playingBGMIDs.Contains(BGMID))
        {
            Destroy(gameObject);
            return;
        }

        //このBGMはまだ再生されていない→登録して破棄されないようにする
        DontDestroyOnLoad(gameObject);  //シーンをまたいでも破棄されなしようにする
        playingBGMIDs.Add(BGMID); //再生中のBGMとして記録
    }

    private void Update()
    {
        CheckSceneForBGM();
    }

    private void CheckSceneForBGM()
    {
        //現在のシーンを取得
        string currentScene = SceneManager.GetActiveScene().name;

        //現在のシーンが再生対象でなければBGMを停止して、自信を破棄
        if (allowedScene.Contains(currentScene) == false)
        {
            playingBGMIDs.Remove(BGMID); //このBGMの再生記録を解除
            Destroy(gameObject); //自信を破棄（＝BGMが止まる）
        }
    }
}
