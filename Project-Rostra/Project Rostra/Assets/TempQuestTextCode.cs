using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempQuestTextCode : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C) && !DialogueManager.instance.isActive)
        {
            gameObject.SetActive(false);
        }
    }
}
