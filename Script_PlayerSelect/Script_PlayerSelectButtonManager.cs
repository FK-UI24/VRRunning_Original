using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

//ボタンクリック時に対応する処理を呼ぶ
public class Script_PlayerSelectButtonManager : MonoBehaviour
{

    //Script_PlayerSelectManagerの中身を参照する用の変数
    //参照するのは「OnSlotSelectedメソッド」
    //こっちのスクリプトから、引数のスロットナンバーを入れて、動作させる
    //ここでは「Script_PlayerSelectManager」型の変数を作っているだけで、中身は空である
    //publicにすることで、中身を「Inspector」からドラッグアンドドロップで代入できる
    public Script_PlayerSelectManager selectManager;

    //3つのボタンの参照用変数
    public Button SelectButton_1;
    public Button SelectButton_2;
    public Button SelectButton_3;

    //SE用変数
    AudioSource SE;

    //保存されているデータの参照用
    string Data_1;
    string Data_2;
    string Data_3;

    //JSONファイルの中身を格納する用の構造体
    [System.Serializable]
    public class Format_BasicInformation
    {
        public string playerName;
        public int age;
        public int gender;
        public float height;
        public float weight;
    }


    void Start()
    {
        //このオブジェクトにアタッチされているSEを格納する
        SE=GetComponent<AudioSource>();

        //それぞれ「Basic_Information.json」ファイルまでのパスを一気に作成する
        Data_1 = Path.Combine(Application.persistentDataPath, "Player_Data", "1_PlayerData", "basic_Information.json");
        Data_2 = Path.Combine(Application.persistentDataPath, "Player_Data", "2_PlayerData", "basic_Information.json");
        Data_3 = Path.Combine(Application.persistentDataPath, "Player_Data", "3_PlayerData", "basic_Information.json");

        //それぞれが存在しているかを確認して、あったら中身の情報を表示する
        if (File.Exists(Data_1))
        {
            //指定されたパスのjsonファイルを取得する
            string json = File.ReadAllText(Data_1);

            //取得したjsonファイルの中身を上部で定義した構造体の形に変換する
            //ここではJSONファイル内のキーと、構造体のフィールド名を照らし合わせて格納する
            //順番ではなく、名前で対応するので、jsonファイル内のデータを参照するときはキーと完全一致でないといけない
            //もし構造体側のみにjsonファイルにはない変数がある場合、デフォルト値が入る
            //jsonにあって構造体にない場合は、問題なく無視される
            //大文字小文字など名前が部分的に違う場合、データが入らない
            Format_BasicInformation data = JsonUtility.FromJson<Format_BasicInformation>(json);

            //JSONファイルから取得した性別番号を引数として対応する文字列を格納する変数
            string gender = NumberToGender(data.gender);


            SelectButton_1.GetComponentInChildren<TMP_Text>().text = 
                MakeNuttonText(data.playerName, data.age, gender, data.height, data.weight);

            //フォントサイズの変更
            SettingFontSize(1, data.playerName);
        }
        else
        {
            SelectButton_1.GetComponentInChildren<TMP_Text>().text = "Player1\nNo Data...";
        }

        //Data_2
        if (File.Exists(Data_2))
        {
            string json = File.ReadAllText(Data_2);

            Format_BasicInformation data = JsonUtility.FromJson<Format_BasicInformation>(json);

            string gender=NumberToGender(data.gender);

            SelectButton_2.GetComponentInChildren<TMP_Text>().text = 
                MakeNuttonText(data.playerName, data.age, gender, data.height, data.weight);

            SettingFontSize(2, data.playerName);

        }
        else
        {
            SelectButton_2.GetComponentInChildren<TMP_Text>().text = "Player2\nNo Data...";
        }

        //Data_3
        if (File.Exists(Data_3))
        {
            string json = File.ReadAllText(Data_3);

            Format_BasicInformation data = JsonUtility.FromJson<Format_BasicInformation>(json);

            string gender = NumberToGender(data.gender);

            SelectButton_3.GetComponentInChildren<TMP_Text>().text =
                MakeNuttonText(data.playerName, data.age, gender, data.height, data.weight);

            SettingFontSize(3, data.playerName);

        }
        else
        {
            SelectButton_3.GetComponentInChildren<TMP_Text>().text = "Player3\nNo Data...";
        }

    }

    //引数の整数をもとに対応する性別の文字列を返す
    string NumberToGender(int n)
    {
        if (n == 0) return "男性";
        else if (n == 1) return "女性";
        else if (n == 2) return "未設定";
        else return "これが表示されてるということはバグです";

    }

    //名前、年齢、性別、身長、体重を引数として、それらを改行を含めて整理して文字列として返す
    string MakeNuttonText(string name,int age,string gender,float height,float weight)
    {
        return "名前：" + name + "\n" + "年齢：" + age + "才\n" + "性別：" + gender + "\n" +
                "身長：" + height + "cm\n" + "体重：" + weight + "kg";

    }

    //6文字以下だと55、それより多いと45にフォントサイズを変更する
    //引数は名前と整数nであり、このnに対応しているボタンのテキストのフォントサイズを変更する
    void SettingFontSize(int n,string name)
    {
        if (name.Length <= 6)
        {
            if (n == 1) SelectButton_1.GetComponentInChildren<TMP_Text>().fontSize = 55;
            else if(n==2) SelectButton_2.GetComponentInChildren<TMP_Text>().fontSize = 55;
            else if (n == 3) SelectButton_3.GetComponentInChildren<TMP_Text>().fontSize = 55;
        }

        else
        {
            if (n == 1) SelectButton_1.GetComponentInChildren<TMP_Text>().fontSize = 40;
            else if (n == 2) SelectButton_2.GetComponentInChildren<TMP_Text>().fontSize = 40;
            else if (n == 3) SelectButton_3.GetComponentInChildren<TMP_Text>().fontSize = 40;
        }
    }
    void Update()
    {
        
    }

    //クリック時のSEを鳴らしてから、次のコルーチン処理を行う
    //コルーチン処理後に、変数nを引数にして、selectManager.OnSlotSelectedを行う
    //コルーチン：途中で停止したり中断できる処理の塊
    //IEnumerator：通常の関数とは違い、途中で処理を一時停止（yield return）できる特別な関数
    //yield return new WaitForSeconds(x)：指定した秒数だけ処理を止めて、そのあとに続きの処理を実行する
    IEnumerator SE_Play(int n)
    {
        SE.Play();
        //SE.clip.lengthは再生するAudioClip（SE）の長さ（秒数）
        yield return new WaitForSeconds(SE.clip.length);

        //基本的にコルーチンの後に行いたい、つまり時間を停止,中断させた後に行いたい動作は、コルーチンと同じ関数（IEnumerator）内に記述する
        //なぜならコルーチンは非同期で動くため、同じ関数内にないと、順序付けができずにすぐに実行されてしまうから
        selectManager.OnSlotSelected(n);
    }

    //スロット１が押されたときに呼ばれる関数
    public void SelectSlot1()
    {
        Debug.Log("スロット１が押されたことにより、データの確認へ");

        //unityではStartCoroutineにIEnumeratorを渡すことで、時間を待つ処理が可能
        //StartCoroutine：IEnumerator型の関数を実行するためのメソッド
        StartCoroutine(SE_Play(1));
        

    }

    //スロット２が押されたときに呼ばれる関数
    public void SelectSlot2()
    {

        Debug.Log("スロット２が押されたことにより、データの確認へ");

        StartCoroutine(SE_Play(2));

    }

    //スロット３が押されたときに呼ばれる関数
    public void SelectSlot3()
    {
        Debug.Log("スロット３が押されたことにより、データの確認へ");

        StartCoroutine(SE_Play(3));

    }

}
