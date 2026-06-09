using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Script_DeleteButtonManager : MonoBehaviour
{

    public GameObject MainPanel;
    public GameObject DeletePanel;

    AudioSource SE;


    // Start is called before the first frame update
    void Start()
    {
        DeletePanel.SetActive(false);

        SE=GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnDeleteButton()
    {
        StartCoroutine(DeletetoDeletePanel());
    }

    public void OnYesButton()
    {
        //ここで{slotNumber}_PlayerDataフォルダまでのパスを一気に作る
        string floderPath = Path.Combine(Application.persistentDataPath,
            "Player_Data", $"{Script_PlayerSelectManager.SelectedPlayerSlot}_PlayerData");

        //第一引数のパスのフォルダを削除する
        //第二引数がtrueの場合、中身ごと削除。falseの場合中身が空なら削除する
        Directory.Delete( floderPath, true );
        Debug.Log("データ削除完了。プレイヤー選択画面へ");

        StartCoroutine(DeletePaneltoPlayerSelect());
    }
    public void OnNoButton()
    {
        StartCoroutine(DeletePaneltoMain());
    }

    IEnumerator DeletetoDeletePanel()
    {
        SE.Play();
        yield return new WaitForSeconds(SE.clip.length);
        DeletePanel.SetActive(true);
        MainPanel.SetActive(false);
    }

    IEnumerator DeletePaneltoPlayerSelect()
    {
        SE.Play();
        yield return new WaitForSeconds(SE.clip.length);
        SceneManager.LoadScene("PlayerSelect");
    }

    IEnumerator DeletePaneltoMain()
    {
        SE.Play();
        yield return new WaitForSeconds(SE.clip.length);
        DeletePanel.SetActive(false);
        MainPanel.SetActive(true);

    }
}

