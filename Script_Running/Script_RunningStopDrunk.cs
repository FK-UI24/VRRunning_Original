using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_RunningStopDrunk : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (Script_StopDrunk.isStopDrunk)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

}
