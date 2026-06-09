using System.Collections.Generic;
using UnityEngine;

public class CanvasManagement : MonoBehaviour
{
    //現在の状態からcanvas制御
    //CurrentStatus.csで呼び出す。状態が変わると対応したパネルに変える。

    [Header("使用するCanvas")]
    [SerializeField] private List<Canvas> canvases=new List<Canvas>();

    public void switchCanvas(int n)
    {
        for(int i = 0; i < canvases.Count; i++)
        {
            if (i == n) canvases[i].gameObject.SetActive(true);
            else
            {
                canvases[i].gameObject.SetActive(false);
            }
        }
    }

}
