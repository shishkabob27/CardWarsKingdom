using System;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredInt
	{
		private ObscuredInt(int value) : this()
		{
		}

		[SerializeField]
		private int currentCryptoKey;
		[SerializeField]
		private int hiddenValue;
		[SerializeField]
		private int fakeValue;
		[SerializeField]
		private bool inited;
	}
}
