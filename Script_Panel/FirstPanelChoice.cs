using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPanelChoice : MonoBehaviour
{
    [Header("すべてのパネルが入っている親オブジェクト（直下にパネル以外が入っているのはダメ）")]
    [SerializeField] private Transform panelParent;

    [Header("シーン開始時に表示させたいパネル")]
    [SerializeField] private string StartPanel;

    // Start is called before the first frame update
    void Start()
    {
        FirstPanel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FirstPanel()
    {
        foreach(Transform panel in panelParent)
        {
            if (panel.gameObject.name == StartPanel)
            {
                panel.gameObject.SetActive(true);
            }
            else
            {
                panel.gameObject.SetActive(false);
            }
        }
    }
}
