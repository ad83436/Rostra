using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatingTexture : MonoBehaviour
{
    private Material material;
    private Vector2 offset;

    public float xVel, yVel;

    private void Awake()
    {
        material = gameObject.GetComponent<Renderer>().material;
    }

    void Start()
    {
        offset = new Vector2(xVel, yVel);
    }

    void Update()
    {
        if (material)
        {
            material.mainTextureOffset += offset * Time.deltaTime;
        }
    }
}
