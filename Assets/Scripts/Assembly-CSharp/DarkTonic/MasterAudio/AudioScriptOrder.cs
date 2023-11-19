using System;

namespace DarkTonic.MasterAudio
{
	public class AudioScriptOrder : Attribute
	{
		public AudioScriptOrder(int order)
		{
		}

		public int Order;
	}
}
