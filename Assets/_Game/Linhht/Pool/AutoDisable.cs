using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yoolax.Framework;

public class AutoDisable : MonoBehaviour
{
	[SerializeField] float duration = 1;

	private float time;

	private void OnEnable()
	{
		time = 0;
	}

	private void Update()
	{
		time += Time.deltaTime;
		if(time > duration)
		{
			this.gameObject.SetActive(false);	
		}
	}

	public void SetDuration(float duration)
	{
		this.duration = duration;
	}

}
