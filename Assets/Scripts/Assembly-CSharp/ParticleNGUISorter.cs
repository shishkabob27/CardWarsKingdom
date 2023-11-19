using UnityEngine;

public class ParticleNGUISorter : MonoBehaviour
{
	private ParticleSystem[] _InitialPS;

	[SerializeField]
	private ParticleSortType _SortType = ParticleSortType.AlwaysOnTop;

	private void Awake()
	{
		ParticleSystem[] componentsInChildren = base.gameObject.GetComponentsInChildren<ParticleSystem>();
		if (_SortType == ParticleSortType.AlwaysOnTop)
		{
			ParticleSystem[] array = componentsInChildren;
			foreach (ParticleSystem particleSystem in array)
			{
				particleSystem.GetComponent<Renderer>().material.renderQueue = 9999;
			}
		}
		if (_SortType == ParticleSortType.AlwaysBehind)
		{
			ParticleSystem[] array2 = componentsInChildren;
			foreach (ParticleSystem particleSystem2 in array2)
			{
				particleSystem2.GetComponent<Renderer>().material.renderQueue = 2500;
			}
		}
		_InitialPS = componentsInChildren;
	}

	private void Update()
	{
		if ((_SortType != 0 && _SortType != ParticleSortType.DynamicBehind) || base.transform.parent == null)
		{
			return;
		}
		UIPanel parentPanel = GetParentPanel();
		for (int i = 0; i < _InitialPS.Length; i++)
		{
			ParticleSystem particleSystem = _InitialPS[i];
			if ((bool)particleSystem)
			{
				SetRenderQueue(particleSystem, parentPanel);
			}
		}
		ParticleSystem component = base.gameObject.GetComponent<ParticleSystem>();
		if (component != null)
		{
			SetRenderQueue(component, parentPanel);
		}
	}

	private UIPanel GetParentPanel()
	{
		UIPanel uIPanel = null;
		GameObject gameObject = base.transform.parent.gameObject;
		while (gameObject != null)
		{
			uIPanel = gameObject.GetComponent<UIPanel>();
			if (uIPanel != null)
			{
				return uIPanel;
			}
			if (gameObject.transform.parent != null)
			{
				gameObject = gameObject.transform.parent.gameObject;
			}
		}
		return null;
	}

	private void SetRenderQueue(ParticleSystem inPS, UIPanel inPanel)
	{
		if (_SortType == ParticleSortType.DynamicBehind)
		{
			inPS.GetComponent<Renderer>().material.renderQueue = inPanel.startingRenderQueue - 1;
		}
		else
		{
			inPS.GetComponent<Renderer>().material.renderQueue = inPanel.startingRenderQueue + inPanel.drawCalls.Count - 1;
		}
	}
}
