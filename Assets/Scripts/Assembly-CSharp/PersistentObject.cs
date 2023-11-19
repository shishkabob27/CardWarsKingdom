using UnityEngine;

public class PersistentObject : MonoBehaviour
{
	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		GameObject[] array = GameObject.FindGameObjectsWithTag("Persistent");
		if (array.Length > 1)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
	}
}
