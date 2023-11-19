using System.Runtime.InteropServices;
using Multiplayer;

public class AsyncData<T>
{
	private bool Processed = true;

	public T MP_Data;

	public ResponseFlag flag;

	public bool processed
	{
		get
		{
			return Processed;
		}
		set
		{
			Processed = value;
		}
	}

	public void Set(ResponseFlag a_flag, [Optional] T a_MP_Data)
	{
		Processed = false;
		MP_Data = a_MP_Data;
		flag = a_flag;
	}
}
