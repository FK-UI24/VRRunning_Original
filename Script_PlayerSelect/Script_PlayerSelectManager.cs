using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;


//スロット選択後の処理（データ確認・シーン遷移）を行う
public class Script_PlayerSelectManager : MonoBehaviour
{

    //外部から参照する用（現在選択中のスロット番号を保持）
    //何も決まっていないときは-1
    //シーンを切り替えても保持される
    //ただし再起動をしたり、自分で明示的に代入するとリセットされる
    public static int SelectedPlayerSlot;

    void Start()
    {
        SelectedPlayerSlot = -1;
    }

    void Update()
    {
        
    }

    //プレイヤースロットが選択されたときに呼び出される関数
    //データ保存フォルダの確認→指定スロットの確認→あったらシーン切り替え,なかったら登録画面
    public void OnSlotSelected(int slotNumber)
    {
        //選択された番号を記録する
        SelectedPlayerSlot = slotNumber;

        //「Player_Data」フォルダのパスを作成
        //Application.persistentDataPathは、端末・OSごとに書き込み可能な永続的データ保存領域のパスを返す
        //Windowsなら"C:/Users/ユーザー名/AppData/LocalLow/会社名/プロジェクト名"
        //Path.Combineは複数のパス文字列を結合し、OSに応じた区切り文字（/または\）を正しく入れてくれる便利な関数
        //今回の場合は「Application.persistentDataPath」の後に「"Player_Data"」をつなげたパスを作成している
        //この時点ではパスを作成しているだけで、「Player_Data」フォルダはあるかないかわからない
        string baseFolder = Path.Combine(Application.persistentDataPath, "Player_Data");

        //「Player_Data」フォルダが存在するかの確認
        //Directory.Existsは「フォルダ（ディレクトリ）が存在するかどうかのみ」を確認する.ファイルの確認などには使えない
        //なかった場合は、フォルダを作る
        if (!Directory.Exists(baseFolder))
        {
            //フォルダがなければ作成する
            //Directory.CreateDirectoryは内部的に指定されたフォルダの確認をしている
            //存在しないなら作成、あるなら何もしない
            //つまり、自動で「存在するかを確認して、必要なら作る」動作が行われる
            //「Directory.CreateDirectory(baseFolder);」のみでも確認、作成または何もしない」の一連の流れができる
            //今回の場合はフォルダがあった時の動作を別で入れているので、先に確認のみしている
            //Directory.CreateDirectory(path);の返り値は、bool型でないので直接if文には入れられない
            Directory.CreateDirectory(baseFolder);
            Debug.Log("Player_Dataフォルダを作った");
        }
        //あった場合はログのみ
        else if (Directory.Exists(baseFolder)) 
        {
            Debug.Log("既にPlayer_Dataフォルダはある");
        }

        //「Player_Dataフォルダ」のパスに続けた、選択されたスロットナンバーに対応したフォルダのパスを作成する
        string slotFolder = Path.Combine(baseFolder, $"{slotNumber}_PlayerData");

        //「slotFolderパス」のフォルダがあるかの確認
        //なかった場合は、登録画面に遷移
        if (!Directory.Exists(slotFolder))
        {
            Debug.Log("データなし。登録画面へ");
            SceneManager.LoadScene("PlayerRegistration");
        }
        //あった場合は、jsonファイルがあるかを確認してあったらメイン画面に、フォルダはあるがjsonファイルがなかったら登録画面へ
        else
        {
            //「slotFolder」のフォルダは存在するが、保存ファイルがあるかの確認用
            string savePath = Path.Combine(slotFolder, "Basic_Information.json");

            //Basic_Information.jsonがあった場合は、ログを表示してメイン画面に遷移
            if (File.Exists(savePath))
            {
                Debug.Log("既存データあり。メイン画面へ");
                SceneManager.LoadScene("Main");
            }
            else {
                //フォルダはあるけどjsonファイルがない=未登録扱いなので登録画面へ
                Debug.Log("フォルダはあるが、データがない。登録画面へ");
                SceneManager.LoadScene("PlayerRegistration");
            }
        }
    }
}
