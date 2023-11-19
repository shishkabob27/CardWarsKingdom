using System;

public class FasterRandom
{
	private const double REAL_UNIT_INT = 4.656612873077393E-10;

	private const double REAL_UNIT_UINT = 2.3283064365386963E-10;

	private const uint Y = 842502087u;

	private const uint Z = 3579807591u;

	private const uint W = 273326509u;

	private static readonly FasterRandom __seedRng = new FasterRandom(Environment.TickCount);

	private uint _x;

	private uint _y;

	private uint _z;

	private uint _w;

	public FasterRandom()
	{
		Reinitialise(__seedRng.Next());
	}

	public FasterRandom(int seed)
	{
		Reinitialise(seed);
	}

	public void Reinitialise(int seed)
	{
		_x = (uint)(seed * 1431655781 + seed * 1183186591 + seed * 622729787 + seed * 338294347);
		_y = 842502087u;
		_z = 3579807591u;
		_w = 273326509u;
	}

	public int Next()
	{
		uint num = _x ^ (_x << 11);
		_x = _y;
		_y = _z;
		_z = _w;
		return (int)(0x7FFFFFFF & (_w = _w ^ (_w >> 19) ^ (num ^ (num >> 8))));
	}

	public int Next(int upperBound)
	{
		if (upperBound < 0)
		{
			throw new ArgumentOutOfRangeException("upperBound", upperBound, "upperBound must be >=0");
		}
		uint num = _x ^ (_x << 11);
		_x = _y;
		_y = _z;
		_z = _w;
		return (int)(4.656612873077393E-10 * (double)(int)(0x7FFFFFFF & (_w = _w ^ (_w >> 19) ^ (num ^ (num >> 8)))) * (double)upperBound);
	}

	public int Next(int lowerBound, int upperBound)
	{
		if (lowerBound > upperBound)
		{
			throw new ArgumentOutOfRangeException("upperBound", upperBound, "upperBound must be >=lowerBound");
		}
		uint num = _x ^ (_x << 11);
		_x = _y;
		_y = _z;
		_z = _w;
		int num2 = upperBound - lowerBound;
		if (num2 < 0)
		{
			return lowerBound + (int)(2.3283064365386963E-10 * (double)(_w = _w ^ (_w >> 19) ^ (num ^ (num >> 8))) * (double)((long)upperBound - (long)lowerBound));
		}
		return lowerBound + (int)(4.656612873077393E-10 * (double)(int)(0x7FFFFFFF & (_w = _w ^ (_w >> 19) ^ (num ^ (num >> 8)))) * (double)num2);
	}

	public double NextDouble()
	{
		uint num = _x ^ (_x << 11);
		_x = _y;
		_y = _z;
		_z = _w;
		return 4.656612873077393E-10 * (double)(int)(0x7FFFFFFF & (_w = _w ^ (_w >> 19) ^ (num ^ (num >> 8))));
	}

	public void NextBytes(byte[] buffer)
	{
		uint num = _x;
		uint num2 = _y;
		uint num3 = _z;
		uint num4 = _w;
		int num5 = 0;
		int num6 = buffer.Length - 3;
		while (num5 < num6)
		{
			uint num7 = num ^ (num << 11);
			num = num2;
			num2 = num3;
			num3 = num4;
			num4 = num4 ^ (num4 >> 19) ^ (num7 ^ (num7 >> 8));
			buffer[num5++] = (byte)num4;
			buffer[num5++] = (byte)(num4 >> 8);
			buffer[num5++] = (byte)(num4 >> 16);
			buffer[num5++] = (byte)(num4 >> 24);
		}
		if (num5 < buffer.Length)
		{
			uint num7 = num ^ (num << 11);
			num = num2;
			num2 = num3;
			num3 = num4;
			num4 = num4 ^ (num4 >> 19) ^ (num7 ^ (num7 >> 8));
			buffer[num5++] = (byte)num4;
			if (num5 < buffer.Length)
			{
				buffer[num5++] = (byte)(num4 >> 8);
				if (num5 < buffer.Length)
				{
					buffer[num5++] = (byte)(num4 >> 16);
					if (num5 < buffer.Length)
					{
						buffer[num5] = (byte)(num4 >> 24);
					}
				}
			}
		}
		_x = num;
		_y = num2;
		_z = num3;
		_w = num4;
	}
}
