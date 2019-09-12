using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QImage : MonoBehaviour
{
    private UIBTL uiBTL;
    public int imageIndex;

    void Start()
    {
        uiBTL = UIBTL.instance;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag.Equals("ImageRecycler"))
        {
            uiBTL.imageRecycle(imageIndex);
        }
    }
}
