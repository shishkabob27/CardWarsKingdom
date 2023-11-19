using UnityEngine;
using System.Collections.Generic;

public class UIToggledObjects : MonoBehaviour
{
	public List<GameObject> activate;
	public List<GameObject> deactivate;
	[SerializeField]
	private GameObject target;
	[SerializeField]
	private bool inverse;
}
