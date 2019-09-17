using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeterController : MonoBehaviour {

	[SerializeField]
	private GameObject meter;

	/// <param name="value">Ranging from 0 to 1</param>
	public void SetMeterLevel(float value) {
		meter.transform.localScale = new Vector3(value, 1f, 1f);
	}
}
