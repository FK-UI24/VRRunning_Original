using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Script_PopPause : MonoBehaviour
{

    [Header("PAUSEパネル")]
    [SerializeField] private GameObject pausePanel;

    [Header("Goalパネル")]
    [SerializeField] private GameObject goalPanel;

    [Header("カメラ")]
    [SerializeField] private GameObject cameraObject;

    private AudioSource SE;

    private bool stopScreen;

    //ゴール時の処理を１回だけ行うよう変数
    private bool goalFlag = false;

    //PAUSE画面が表示されているかのフラグ
    private bool pauseFlag;

    private void Start()
    {
        pausePanel.SetActive(false);
        pauseFlag = false;

        SE = GetComponent<AudioSource>();
    }

    private void Update()
    {
        //もし停止中ならPを押せないようにする
        stopScreen = cameraObject.GetComponentInChildren<Script_RunningCameraManager>().isStopRunning;

        if (stopScreen)
        {
            return;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.P) && !pauseFlag && !goalPanel.activeSelf)
            {
                StartCoroutine(downp());
            }
            if (goalPanel.activeSelf && !goalFlag)
            {
                StartCoroutine(onreturn());
                goalFlag = true;
            }
        }
    }

    IEnumerator downp()
    {
        SE.Play();
        yield return new WaitForSeconds(SE.clip.length);
        pausePanel.SetActive(true);
        pauseFlag = true;
        Debug.Log("PAUSE画面表示");
    }

    public void OnReturn()
    {
        StartCoroutine(onreturn());
    }

    IEnumerator onreturn()
    {
        SE.Play();
        yield return new WaitForSeconds(SE.clip.length);
        pausePanel.SetActive(false);
        pauseFlag = false;
        Debug.Log("PAUSE画面非表示");
    }

}
