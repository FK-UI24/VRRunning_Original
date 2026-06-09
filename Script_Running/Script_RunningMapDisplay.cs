using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_RunningMapDisplay : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (Script_MapDisplay.isMapDisplay)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

}
