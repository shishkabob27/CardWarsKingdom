using UnityEngine;

public class FX_RandomSpawn : MonoBehaviour
{
	public GameObject fx;

	public bool lockX;

	public float minSpawnTime = 3f;

	private float currentTime;

	private float spawnTime;

	private void Start()
	{
		spawnTime = Random.Range(2, 11);
	}

	private void Update()
	{
		if (Time.time - currentTime > spawnTime)
		{
			SpwanFX();
			currentTime = Time.time;
		}
	}

	private void SpwanFX()
	{
		if (!lockX)
		{
			Object.Instantiate(fx, new Vector3(Random.Range(-6, 10), 0f, Random.Range(16, -10)), base.transform.rotation);
		}
		else
		{
			Object.Instantiate(fx, new Vector3(Random.Range(-6, 10), 0f, 40f), base.transform.rotation);
		}
		spawnTime = Random.Range(minSpawnTime, 11f);
	}
}
