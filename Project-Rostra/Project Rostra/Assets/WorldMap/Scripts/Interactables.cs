using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
