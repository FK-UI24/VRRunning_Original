using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_SE : MonoBehaviour
{
    //アタッチしている音の配列用変数
    private AudioSource[] SE;

    // Start is called before the first frame update
    void Start()
    {
        SE = GetComponents<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {


    }

    public void SE_StartGun() {
            SE[1].Play();
    }


    //これを使用する場合、舞フレーム呼び出さなければならない
    //条件式などで一瞬呼び出すとtrueのままになる
    public bool check_SE_StartGun() {
        if (SE[1].isPlaying)
        {
            return true;
        }
        return false;
    }

}
