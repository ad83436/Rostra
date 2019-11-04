using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// oo oo ah ah
public class Interactables : MonoBehaviour
{
    public Transform player;
    public Transform interactable;
    float minDistance = 1.5f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && (transform.position - interactable.position).magnitude <= minDistance)
        {
            Debug.Log("Open");
        }
    }


}
