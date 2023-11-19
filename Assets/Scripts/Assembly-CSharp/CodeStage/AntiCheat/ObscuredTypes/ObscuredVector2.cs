using System;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredVector2
	{
		[Serializable]
		public struct RawEncryptedVector2
		{
			public int x;
			public int y;
		}

		private ObscuredVector2(ObscuredVector2.RawEncryptedVector2 value) : this()
		{
		}

		[SerializeField]
		private int currentCryptoKey;
		[SerializeField]
		private RawEncryptedVector2 hiddenValue;
		[SerializeField]
		private Vector2 fakeValue;
		[SerializeField]
		private bool inited;
	}
}
