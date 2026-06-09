using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_OnlySE : MonoBehaviour
{
    AudioSource SE;

    void Start()
    {
        SE = GetComponent<AudioSource>();
    }

    public void OnButtonToSE()
    {
        SE.Play();
    }
}
