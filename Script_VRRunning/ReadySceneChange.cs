using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
public class ReadySceneChange : MonoBehaviour
{
    AudioSource SE;

    void Start()
    {
        SE = GetComponent<AudioSource>();

    }

    void Update()
    {

    }

    public void NextScene()
    {
        StartCoroutine(SEtoChange());
    }

    IEnumerator SEtoChange()
    {
        SE.Play();
        yield return new WaitForSeconds(SE.clip.length);

        if (Script_StageSelectManager.SelectedStageName == "TEST")
        {
            SceneManager.LoadScene("TEST_VRRunning");
        }
        else if (Script_StageSelectManager.SelectedStageName == "Numadu")
        {
            SceneManager.LoadScene("Numadu_VRRunning");
        }
        else
        {
            SceneManager.LoadScene("TEST_VRRunning");
        }
    }
}
