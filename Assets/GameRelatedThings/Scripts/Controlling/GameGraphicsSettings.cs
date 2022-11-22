using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GameGraphicsSettings : MonoBehaviour
{
    public TMP_Dropdown resDb;
    Resolution[] resolutions;

    // Start is called before the first frame update
    private void Start()
    {
        //Felbont�si be�ll�t�s
        resolutions = Screen.resolutions;

        resDb.ClearOptions();

        List<string> options = new List<string>();

        int currentResIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResIndex = i;
            }
        }

        resDb.AddOptions(options);
        resDb.value = currentResIndex;
        resDb.RefreshShownValue();
    }

    //Teljesk�perny�
    public void SetFullscreen(bool isfullScreen)
    {
        Screen.fullScreen = isfullScreen;
    }

    //Min�s�gbe�ll�t�s
    public void SetQuality(int qIndex)
    {
        QualitySettings.SetQualityLevel(qIndex);
    }

    //Felbont�s be�ll�t�s
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}
