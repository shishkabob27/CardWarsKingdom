using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VFXAutoDestructShuriken : MonoBehaviour
{
	public bool OnlyDeactivate;

	private List<ParticleSystem> pSystems = new List<ParticleSystem>();

	private void OnEnable()
	{
		ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>(true);
		pSystems = componentsInChildren.ToList();
		StartCoroutine("CheckIfAlive");
	}

	private IEnumerator CheckIfAlive()
	{
		bool anyAlive;
		do
		{
			yield return new WaitForSeconds(0.5f);
			anyAlive = false;
			foreach (ParticleSystem ps in pSystems)
			{
				if (ps.IsAlive(true))
				{
					anyAlive = true;
					break;
				}
			}
		}
		while (anyAlive);
		if (OnlyDeactivate)
		{
			base.gameObject.SetActive(false);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}
}
