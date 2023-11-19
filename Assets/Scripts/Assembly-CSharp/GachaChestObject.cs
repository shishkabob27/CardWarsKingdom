using UnityEngine;

public class GachaChestObject : MonoBehaviour
{
	public Animator Animator;

	public Transform CardNode;

	public GameObject ChestVFXObj;

	public GameObject CardVFXObj;

	public void EnableChestVFXObj(bool isEnable)
	{
		if (ChestVFXObj != null)
		{
			ChestVFXObj.SetActive(isEnable);
		}
	}

	public void EnableCardVFXObj(bool isEnable)
	{
		if (CardVFXObj != null)
		{
			CardVFXObj.SetActive(isEnable);
		}
	}
}
