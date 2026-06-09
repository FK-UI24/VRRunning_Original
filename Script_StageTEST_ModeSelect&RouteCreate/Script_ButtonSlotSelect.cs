using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_ButtonSlotSelect : MonoBehaviour
{
    //宣言するだけだと、形だけで中身がない
    //そのためインスペクター側で中身ごとセットする
    [Header("「DataSelectManager」をここにセットする")]
    [SerializeField] private DataSelectManager dataSelectManager;

    [Header("ボタンと対応付けさせるスロット番号")]
    [SerializeField] private int SlotNumber;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSlotNumber()
    {
        dataSelectManager.OnSlotSelected(SlotNumber);
    }
}
