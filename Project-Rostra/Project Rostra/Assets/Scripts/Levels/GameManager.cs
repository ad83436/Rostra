using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int targetFPS = 60;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFPS;
    }

    private void Update()
    {
        if (Application.targetFrameRate != targetFPS)
        {
            Application.targetFrameRate = targetFPS;
        }
    }
}
