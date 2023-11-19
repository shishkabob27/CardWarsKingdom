using UnityEngine;

public class KFFRequestorController : MonoBehaviour
{
	public bool enablePlacements;

	public static KFFRequestorController controller;

	private void Awake()
	{
		controller = this;
	}

	public static KFFRequestorController GetInstance()
	{
		return controller;
	}

	public GameObject GetRequestor(string name)
	{
		Transform transform = base.gameObject.transform.Find(name);
		if (transform != null)
		{
			return transform.gameObject;
		}
		return null;
	}

	public void RequestContent(string name)
	{
	}

	public void checkLowOnHardCurrency(int itemcost)
	{
	}
}
