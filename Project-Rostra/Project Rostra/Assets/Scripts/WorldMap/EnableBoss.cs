using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableBoss : MonoBehaviour
{
	public bool enableBoss;
	private void OnTriggerStay2D(Collider2D col)
	{
		if (col.CompareTag("Player") && enableBoss == true)
		{
			Debug.Log("Is boss is true");
			EnemySpawner.instance.isBoss = true;
		}
		if (col.CompareTag("Player") && enableBoss == false)
		{
			Debug.Log("Is boss is false");
			EnemySpawner.instance.isBoss = false;
		}
	}
}
