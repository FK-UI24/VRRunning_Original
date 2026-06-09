using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Script_OKButton : MonoBehaviour
{
    //InputField用の変数
    public TMP_InputField InputName;
    public TMP_InputField InputAge;
    public TMP_InputField InputHeight;
    public TMP_InputField InputWeight;

    //InputFieldの中身参照用の変数
    private string Name;
    private string Age;
    private string Height;
    private string Weight;

    //エラーのときに出てくるメッセージ
    public TMP_Text Error_Name;
    public TMP_Text Error_Age;
    public TMP_Text Error_Gender;


    //入力されたデータを新しくjsonファイルを作成し、そこに保存する関数が書いてあるScriptを格納する用
    public Script_InputDate inputData;

    //SE用変数
    AudioSource SE;


    // Start is called before the first frame update
    void Start()
    {
        //最初エラー文は表示しない
        Error_Name.gameObject.SetActive(false);
        Error_Age.gameObject.SetActive(false);
        Error_Gender.gameObject.SetActive(false);

        //このオブジェクトにアタッチされているSEを格納する
        SE=GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PushOK()
    {
        //それぞれのインプットフィールドの内容を代入する
        Name = InputName.text;
        Age = InputAge.text;
        Height = InputHeight.text;
        Weight = InputWeight.text;

        //OKボタンが押されたとき瞬間は毎回エラー文を非表示にする
        //これによりOKボタンを押したときにエラー文表示をリセットできる
        //再び押されたときにそのときに対応したエラー文を表示できるようになる
        Error_Name.gameObject.SetActive(false);
        Error_Age.gameObject.SetActive(false);
        Error_Gender.gameObject.SetActive(false);

        //もしName,Ageが入力されていて、Genderボタンのいずれかが選ばれていた場合
        if (CheckInputField(Name) &&
            CheckInputField(Age) &&
            CheckGenderButton())
        {

            //Weight,Heightは入力されなくてもいい
            //ただしその場合GPTで調べた全年齢のそれぞれの平均身長,体重が自動で入る
            //ManでHeight,Weightが入力されていない場合
            if (Script_Gender.Gender_num == 0 && Height == "")
            {
                Height = "167.3";
            }
            if (Script_Gender.Gender_num == 0 && Weight == "")
            {
                Weight = "67.4";
            }

            //WomanでHeight,Weightが入力されていない場合
            if (Script_Gender.Gender_num == 1 && Height == "")
            {
                Height = "154.5";
            }
            if (Script_Gender.Gender_num == 1 && Weight == "")
            {
                Weight = "54.0";
            }

            //Prefeer not to sayでHeight,Weightが入力されていない場合
            if (Script_Gender.Gender_num == 2 && Height == "")
            {
                Height = "166.5";
            }
            if (Script_Gender.Gender_num == 2 && Weight == "")
            {
                Weight = "67.0";
            }

            //入力されたデータの表示
            //名前、年齢、性別番号、身長、体重、プレイヤー選択画面で選んだスロット番号を表示、関数に渡す
            Debug.Log(Script_PlayerSelectManager.SelectedPlayerSlot+" "+Name + " " + Age + " " + 
                Script_Gender.Gender_num + " " + Height + " " + Weight);

            //
            inputData.SaveBasicInformation(Name, int.Parse(Age), Script_Gender.Gender_num, float.Parse(Height), 
                float.Parse(Weight), Script_PlayerSelectManager.SelectedPlayerSlot);

            //SEが鳴り終わってからシーンが切り替わるようにする
            StartCoroutine(SEtoLoadScene());

        }

        //Nameが入力されていない場合、エラー文の表示
        if (CheckInputField(Name) == false)
        {
            Error_Name.gameObject.SetActive(true);
            Debug.Log("名前が足りない");
        }

        //Ageが入力されていない場合、エラー文の表示
        if (CheckInputField(Age) == false)
        {
            Error_Age.gameObject.SetActive(true);
            Debug.Log("年齢が足りない");
        }

        //Genderが選択されていない場合、エラー文の表示
        if (CheckGenderButton() == false)
        {
            Error_Gender.gameObject.SetActive(true);
            Debug.Log("性別を選択していない");
        }



    }

    //SEが鳴り終わってからシーンを切り替える
    IEnumerator SEtoLoadScene()
    {
        SE.Play();
        yield return new WaitForSeconds(SE.clip.length);
        SceneManager.LoadScene("Main");
    }

    //3つのGenderボタンの内1つでも選択されているか確認する関数
    bool CheckGenderButton()
    {
        //Script_Gender.Gender_numは初期値が-1であり、いずれかが選択されると0,1,2のどれかになり、何も選択されていない状態に戻れない
        //-1以外の場合、trueを返す
        if (Script_Gender.Gender_num != -1)
        {
            return true;
        }
        //該当しないならfalseを返す
        return false;
    }

    //引数のstringが空白のみ、null、0文字を除外する関数
    bool CheckInputField(string str)
    {
        //特定の文字カウント用変数
        int count = 0;

        //nullは除外
        if (str == null) return false;

        //文字数0は除外
        if (str.Length == 0) return false;

        //文字列を1文字ずつ確認して、空白文字があったらcountを1プラス
        //countと文字列の文字数が同じなら、全て空白であるので除外
        foreach (char c in str)
        {
            if (c == ' ' || c == '　')
            {
                count++;
            }
        }
        if (count == str.Length)
        {
            return false;
        }

        return true;

    }
}
