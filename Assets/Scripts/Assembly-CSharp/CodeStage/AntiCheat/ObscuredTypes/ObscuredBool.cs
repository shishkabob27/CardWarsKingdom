using System;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredBool
	{
		private ObscuredBool(int value) : this()
		{
		}

		[SerializeField]
		private byte currentCryptoKey;
		[SerializeField]
		private int hiddenValue;
		[SerializeField]
		private bool fakeValue;
		[SerializeField]
		private bool fakeValueChanged;
		[SerializeField]
		private bool inited;
	}
}
