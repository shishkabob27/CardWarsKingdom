using UnityEngine;

public class VFX_Desert : MonoBehaviour
{
	public GameObject fx;

	public float spawnRangeMin = 2f;

	public float spawnRangeMax = 6f;

	private void Start()
	{
		InvokeRepeating("RandomSpawn", 2f, Random.Range(spawnRangeMin, spawnRangeMax));
	}

	private void RandomSpawn()
	{
		Vector3 position = base.transform.position;
		position.x = Random.Range(-9f, 9f);
		position.z = Random.Range(-20f, 20f);
		GameObject gameObject = Object.Instantiate(fx, position, Quaternion.identity) as GameObject;
	}
}
