using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Script_InputDate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //プレイヤーの基本情報の構造体を宣言する
    //Unityの「JsonUtility」で使いやすいように「Serializable」属性を付ける
    [System.Serializable]
    public class Format_BasicInformation
    {
        public string playerName;
        public int age;
        public int gender;
        public float height;
        public float weight;
    }

    //プレイヤーデータを保存する関数
    //引数にプレイヤー情報と、どのスロット番号に保存するかの識別用の整数を受け取る
    //流れとしては、保存先のフォルダがあるか、その中にプレイヤーごとのフォルダはあるか確認する
    //JSONファイルを作りその中にデータを保存する
    public void SaveBasicInformation(string name,int age,int gender,float height, float weight,int slotNumber)
    {
        //Format_BasicInformationクラスのインスタンスを作成し、受け取ったデータで初期化
        Format_BasicInformation basic_information = new Format_BasicInformation()
        {
            playerName = name,
            age = age,
            gender = gender,
            height = height,
            weight = weight
        };

        //basic_informationの内容をJSON用文字列に変換する
        //第二引数trueは整形（開業やインデント）を有効にして、人間が読みやすくする
        string json = JsonUtility.ToJson(basic_information, true);

        //ここで行うのは保存先である「PlayerData」フォルダがあるかの確認
        //JSONファイルの保存先のもととなるフォルダを指定する
        //フォルダの作成は「Script_PlayerSelectManager」で行っているので、そこを指定する
        string baseFolder = Path.Combine(Application.persistentDataPath, "Player_Data");

        //一応もし「Player_Data」フォルダがなかった場合に作る
        if (!Directory.Exists(baseFolder))
        {
            Directory.CreateDirectory(baseFolder);
            Debug.Log("Player_Dataフォルダを作った");
        }

        //ここで行うのは「PlayerData」フォルダ内にある「(slotNumber)_PlayerData」があるかの確認
        //スロット番号ごとのフォルダ名を作成する
        //例：0_PlayerData
        string slotFolder = Path.Combine(baseFolder, $"{slotNumber}_PlayerData");

        //一応もし「(slotNumber)_PlayerData」がなかった場合に作る
        if (!Directory.Exists(slotFolder))
        {
            Directory.CreateDirectory(slotFolder);
            Debug.Log(slotNumber + "_PlayerDataフォルダを作った");
        }

        //ここからJSONファイルの保存を行う
        //保存する場所のパスを作成する
        //例：PlayerData/0_PlayerData/Basic_Information.json
        string jsonFilePath = Path.Combine(slotFolder, "Basic_Information.json");

        //JSON文字列を指定したパスのファイルに書き込み（上書き保存）
        //ファイルがあった場合は、中身をすべて削除して書き込む
        //ファイルがなかった場合は、作成して書き込む
        File.WriteAllText(jsonFilePath, json);

        //保存完了のログを表示
        Debug.Log("プレイヤーデータを保存しました" + jsonFilePath);
    }
}
