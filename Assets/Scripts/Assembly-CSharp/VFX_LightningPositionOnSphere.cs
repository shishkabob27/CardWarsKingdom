using UnityEngine;

public class VFX_LightningPositionOnSphere : MonoBehaviour
{
	private void Start()
	{
		InvokeRepeating("RandomPos", 2f, 0.3f);
	}

	private void Update()
	{
	}

	private void RandomPos()
	{
		int num = Random.Range(1, 6);
		if (num == 1)
		{
			base.transform.position = new Vector3(-3f, 7f, 7.5f);
		}
		if (num == 2)
		{
			base.transform.position = new Vector3(-2f, 7.8f, 7.5f);
		}
		if (num == 3)
		{
			base.transform.position = new Vector3(-3.5f, 7.4f, 7.5f);
		}
		if (num == 4)
		{
			base.transform.position = new Vector3(-2.6f, 9.5f, 8.5f);
		}
	}
}
