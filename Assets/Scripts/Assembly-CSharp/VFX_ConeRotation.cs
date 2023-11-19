using UnityEngine;

public class VFX_ConeRotation : MonoBehaviour
{
	public float rotateSpeed = 0.3f;

	public float rotateAmount = 30f;

	public Quaternion startRotation;

	public bool rotL;

	public bool rotR;

	private void Start()
	{
		startRotation = base.transform.rotation;
	}

	private void Update()
	{
		if (rotL)
		{
			base.transform.Rotate(Vector3.left * Time.deltaTime * rotateSpeed);
		}
		if (rotR)
		{
			base.transform.Rotate(Vector3.right * Time.deltaTime * rotateSpeed);
		}
	}
}
