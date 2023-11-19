public class KFFRandom
{
	private static FasterRandom Gen = new FasterRandom();

	public static float Value
	{
		get
		{
			return (float)Gen.NextDouble();
		}
	}

	public static int Seed
	{
		get
		{
			int num = Gen.Next();
			Gen.Reinitialise(num);
			return num;
		}
		set
		{
			Gen.Reinitialise(value);
		}
	}

	public static int Range(int min, int max)
	{
		return Gen.Next(min, max + 1);
	}

	public static int Percent()
	{
		return Range(1, 100);
	}

	public static bool Percent(int chance)
	{
		return Percent() <= chance;
	}

	public static float Range(float min, float max)
	{
		return (float)((double)min + (double)(max - min) * Gen.NextDouble());
	}

	public static int RandomIndex(int count)
	{
		return Gen.Next(count);
	}
}
