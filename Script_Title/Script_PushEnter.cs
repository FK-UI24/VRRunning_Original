using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Script_PushEnter : MonoBehaviour
{
    //エンターが押されたかの確認
    bool check_Enter;

    //アタッチしているTMPの情報を入れる
    TextMeshProUGUI Push_Enter;

    //StartGunがなっているかを確認する
    bool check_StartGun;

    //SCript_SEを入れる用
    Script_SE script_SE;

    // Start is called before the first frame update
    void Start()
    {
        //初期値の代入
        check_Enter = false;
        Push_Enter = GetComponent<TextMeshProUGUI>();
        check_StartGun = false;

        //シーン内にあるScript_SEを１つ探す
        script_SE = FindObjectOfType<Script_SE>();

    }

    // Update is called once per frame
    void Update()
    {

        //初めてエンターが押された場合のみ、判定をtrueにする
        if (Input.GetKeyDown(KeyCode.Return) && check_Enter == false)
        {
            //エンターを押したかの確認用
            Debug.Log("PUSH ENTER");

            //Script_SEのStartGunの音を出す
            script_SE.SE_StartGun();

            //check_Enterを切り替える
            check_Enter = true;

        }


        //StartGunがなっているかの判定をする
        check_StartGun = script_SE.check_SE_StartGun();


        //エンターがすでに押されており、StartGunがなり終わっており、TMPのAlpha値が0.01を下回ったらシーンの切り替え
        if (check_Enter == true &&check_StartGun==false&& Push_Enter.color.a < 0.01)
        {
            SceneManager.LoadScene("PlayerSelect");
        }
    }
}
