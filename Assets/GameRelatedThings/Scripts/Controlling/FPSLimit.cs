using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSLimit : MonoBehaviour
{
    [SerializeField]
    Text fpsLimitText;

    [SerializeField]
    int fpsLimit;

    float deltaTime;
    float count;

    void Awake()
    {
        Application.targetFrameRate = fpsLimit;
    }

    IEnumerator Start()
    {
        while (true)
        {
            if (Time.timeScale == 1)
            {
                yield return new WaitForSeconds(0.1f);
                deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
                count = (1 / Time.deltaTime);
                fpsLimitText.text = Mathf.Ceil(count).ToString();
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

}
