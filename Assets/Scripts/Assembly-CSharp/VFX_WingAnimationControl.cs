using System;
using UnityEngine;

public class VFX_WingAnimationControl : MonoBehaviour
{
	public float rotateSpeed = 0.3f;

	public float rotateAmount = 30f;

	public Quaternion startRotation;

	public bool WingL;

	public bool WingR;

	private void Start()
	{
		startRotation = base.transform.rotation;
	}

	private void Update()
	{
		if (WingL)
		{
			base.transform.rotation = Quaternion.AngleAxis(rotateAmount * Mathf.Sin(Time.time * (float)Math.PI * 2f * rotateSpeed), Vector3.up) * startRotation;
		}
		if (WingR)
		{
			base.transform.rotation = Quaternion.AngleAxis(rotateAmount * Mathf.Sin(Time.time * (float)Math.PI * 2f * rotateSpeed), Vector3.down) * startRotation;
		}
	}
}
