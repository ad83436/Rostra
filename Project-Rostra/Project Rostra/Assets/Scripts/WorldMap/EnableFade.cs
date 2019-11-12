using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableFade : MonoBehaviour
{
	public Canvas fade; 
	private void OnTriggerStay2D(Collider2D collision)
	{
		fade.gameObject.SetActive(true);
	}
}
