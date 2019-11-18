using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableBoss : MonoBehaviour
{
	public bool enableBoss;
	private void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("Player") && enableBoss == true)
		{
			EnemySpawner.instance.isBoss = true;
		}
		if (col.CompareTag("Player") && enableBoss == false)
		{
			EnemySpawner.instance.isBoss = false;
		}
	}
}
