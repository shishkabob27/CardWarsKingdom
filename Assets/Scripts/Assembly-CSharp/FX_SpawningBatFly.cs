using UnityEngine;

public class FX_SpawningBatFly : MonoBehaviour
{
	public GameObject batFX1;

	public GameObject batFX2;

	private float currentTime;

	public float nextSpawnTime;

	private int index;

	private void Start()
	{
		index = 1;
		batFX1.SetActive(false);
		batFX2.SetActive(false);
	}

	private void Update()
	{
		if (Time.time - currentTime > nextSpawnTime)
		{
			if (index == 1)
			{
				SpawnA();
			}
			if (index == -1)
			{
				SpawnB();
			}
			index = -index;
			currentTime = Time.time;
		}
	}

	private void SpawnA()
	{
		batFX1.SetActive(true);
		batFX2.SetActive(false);
	}

	private void SpawnB()
	{
		batFX1.SetActive(false);
		batFX2.SetActive(true);
	}
}
