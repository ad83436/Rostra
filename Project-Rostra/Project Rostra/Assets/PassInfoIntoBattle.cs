using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassInfoIntoBattle : MonoBehaviour
{
    public static Sprite battleBackgroundImage;
    void Start()
    {
        DontDestroyOnLoad(this);
        GameManager.instance.listOfUndestroyables.Add(this.gameObject);
    }
}
