using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Script_Gender : MonoBehaviour
{
    //インスペクター画面から設定された３つのボタンを保持する変数
    public Button Button_Man;
    public Button Button_Woman;
    public Button Button_No;

    public static int Gender_num;


    void Start()
    {
        Gender_num = -1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnAnyButton(int index)
    {
        if (index == 0)
        {
            Debug.Log("Man");

            Button_Man.interactable = false;
            Button_Woman.interactable = true;
            Button_No.interactable = true;

            Gender_num= 0;
            Debug.Log(Gender_num);
            
            
            
        }
        else if (index == 1)
        {
            Debug.Log("Woman");

            Button_Man.interactable = true;
            Button_Woman.interactable = false;
            Button_No.interactable = true;

            Gender_num = 1;
            Debug.Log(Gender_num);


        }
        else if (index == 2)
        {
            Debug.Log("Prefer Not to Say");

            Button_Man.interactable = true;
            Button_Woman.interactable = true;
            Button_No.interactable = false;

            Gender_num = 2;
            Debug.Log(Gender_num);

        }
        else
        {
            Debug.Log("一体どんな選択肢をしたら、このメッセージが表示されるのか...");
        }
    }
    
}
