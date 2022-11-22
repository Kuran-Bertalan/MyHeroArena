using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetLevels : MonoBehaviour
{  
    public void Clicked()
    {
        PlayerPrefs.DeleteAll();
    }
}
