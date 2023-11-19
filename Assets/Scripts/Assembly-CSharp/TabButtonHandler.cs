using System;
using UnityEngine;

public class TabButtonHandler : MonoBehaviour
{
	public event Action Clicked = delegate
	{
	};

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnClick()
	{
		this.Clicked();
	}
}
