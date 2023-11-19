using UnityEngine;

public class VFX_RandomPos : MonoBehaviour
{
	public float randomNum = 1f;

	private void Update()
	{
		base.transform.localPosition = new Vector3(Random.Range(randomNum, -1f * randomNum), Random.Range(randomNum, -1f * randomNum), Random.Range(randomNum, -1f * randomNum));
	}
}
