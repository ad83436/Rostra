using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Contact : MonoBehaviour
{
    public void JumpToLink(string url)
    {
        Application.OpenURL(url);
    }
}
