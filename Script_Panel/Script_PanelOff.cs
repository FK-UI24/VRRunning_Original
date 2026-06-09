using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_PanelOff : MonoBehaviour
{
    //ボタンにアタッチしてSEを鳴らして設定したパネルをオフにするプログラム

    [Header("オフにするパネル")]
    [SerializeField] private GameObject offObject;

    //SE用変数
    private AudioSource SE;

    private void Start()
    {
        SE = GetComponent<AudioSource>();
    }

    public void OnOffPanelButton()
    {
        StartCoroutine(SEtoOff());
    }

    IEnumerator SEtoOff()
    {
        SE.Play();
        yield return new WaitForSeconds(SE.clip.length);
        offObject.SetActive(false);
    }
}
