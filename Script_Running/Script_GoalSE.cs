using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_GoalSE : MonoBehaviour
{
    private AudioSource[] SEs;
    
    public void playSE()
    {
        //SE‚ğæ“¾‚·‚é
        SEs = GetComponents<AudioSource>();

        //0`2‚Ì—”‚ğæ“¾‚·‚é
        int value = Random.Range(0, 3);

        //‘I‚Î‚ê‚½SE‚ğ–Â‚ç‚·
        SEs[value].Play();

    }
}
