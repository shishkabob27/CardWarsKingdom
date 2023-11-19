using UnityEngine;

public class FX_Footprints : MonoBehaviour
{
	private int footIndex = 1;

	public GameObject footprintL;

	public GameObject footprintR;

	public GameObject footprintLPos;

	public GameObject footprintRPos;

	private float currentTime;

	public float stepGapTime;

	private int index;

	private void Start()
	{
		index = 1;
	}

	private void Update()
	{
		if (Time.time - currentTime > stepGapTime)
		{
			if (index == 1)
			{
				FootprintsA();
			}
			if (index == -1)
			{
				FootprintsB();
			}
			index = -index;
			currentTime = Time.time;
		}
	}

	private void FootprintsA()
	{
		float y = GameObject.Find("FX_FootprintsParent").transform.eulerAngles.y;
		GameObject gameObject = Object.Instantiate(footprintL, footprintLPos.transform.position, footprintLPos.transform.rotation) as GameObject;
		gameObject.transform.localEulerAngles = new Vector3(-90f, y - 90f, 0f);
	}

	private void FootprintsB()
	{
		float y = GameObject.Find("FX_FootprintsParent").transform.eulerAngles.y;
		GameObject gameObject = Object.Instantiate(footprintR, footprintRPos.transform.position, footprintRPos.transform.rotation) as GameObject;
		gameObject.transform.localEulerAngles = new Vector3(-90f, y - 90f, 0f);
	}
}
