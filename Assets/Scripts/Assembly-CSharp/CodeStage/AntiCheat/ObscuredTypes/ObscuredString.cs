using System;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public class ObscuredString
	{
		private ObscuredString()
		{
		}

		[SerializeField]
		private string currentCryptoKey;
		[SerializeField]
		private byte[] hiddenValue;
		[SerializeField]
		private string fakeValue;
		[SerializeField]
		private bool inited;
	}
}
