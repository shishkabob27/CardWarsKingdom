using UnityEngine;
using System.Collections.Generic;

public class UIToggledComponents : MonoBehaviour
{
	public List<MonoBehaviour> activate;
	public List<MonoBehaviour> deactivate;
	[SerializeField]
	private MonoBehaviour target;
	[SerializeField]
	private bool inverse;
}
