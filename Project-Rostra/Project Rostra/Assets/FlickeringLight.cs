using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    private new Light light;
    private float lightIntensity1;
    private float lightIntensity2;
	private bool lightUp = true;
    private float timeToChangeFlicker = 0.2f;
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
		//if (light.intensity == lightIntensity1)
		//{
		//	light.intensity += 0.1f; // chanege this back to value
		//	StartCoroutine(Flicker());
		//}
		//else
		//{
		//	light.intensity = lightIntensity1;
		//	StartCoroutine(Flicker());
		//}

		if (light.intensity <= lightIntensity1 && lightUp == true)
		{
			Debug.Log("LightUp");
			light.intensity += 0.2f;
			StartCoroutine(Flicker());
			if (light.intensity >= lightIntensity2)
			{
				lightUp = false;
			}
		}
		else if(light.intensity >= lightIntensity2 && lightUp == false)
		{
			Debug.Log("Lightdown");
			light.intensity -= 0.2f;
			if (light.intensity <= lightIntensity2)
			{
				lightUp = true;
			}
			StartCoroutine(Flicker());
		}


	}
}
