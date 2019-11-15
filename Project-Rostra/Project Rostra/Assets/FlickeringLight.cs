using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    private new Light light;
    private float lightIntensity1;
    private float lightIntensity2;
    private float timeToChangeFlicker = 0.3f;
    void Start()
    {
        light = gameObject.GetComponent<Light>();
        lightIntensity1 = light.intensity;
        lightIntensity2 = light.intensity - 0.5f;

        StartCoroutine(Flicker());
    }

    private IEnumerator Flicker()
    {
        yield return new WaitForSeconds(timeToChangeFlicker);
        if(light.intensity == lightIntensity1)
        {
            light.intensity = lightIntensity2;
            StartCoroutine(Flicker());
        }
        else
        {
            light.intensity = lightIntensity1;
            StartCoroutine(Flicker());
        }
    }
}
