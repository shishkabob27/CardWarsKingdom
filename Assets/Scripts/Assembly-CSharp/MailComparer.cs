using System.Collections.Generic;

public class MailComparer : IComparer<MailItem>
{
	private class SortFunction
	{
		public delegate int CompareFunction(MailItem a, MailItem b);

		public CompareFunction Function;

		public bool Reversed;

		public SortFunction(CompareFunction func, bool reversed)
		{
			Function = func;
			Reversed = reversed;
		}
	}

	private List<SortFunction> mSortFunctions = new List<SortFunction>();

	public MailComparer(List<SortEntry> sortEntries)
	{
		bool flag = false;
		foreach (SortEntry sortEntry in sortEntries)
		{
			SortFunction.CompareFunction compareFunction = null;
			if (sortEntry.SortType == SortTypeEnum.Newest)
			{
				compareFunction = Newest;
				flag = true;
			}
			if (compareFunction != null)
			{
				mSortFunctions.Add(new SortFunction(compareFunction, sortEntry.Reversed));
			}
		}
		if (!flag)
		{
			mSortFunctions.Add(new SortFunction(Newest, false));
		}
	}

	public int Compare(MailItem a, MailItem b)
	{
		foreach (SortFunction mSortFunction in mSortFunctions)
		{
			int num = mSortFunction.Function(a, b);
			if (num != 0)
			{
				return (!mSortFunction.Reversed) ? num : (-1 * num);
			}
		}
		return 0;
	}

	private int Newest(MailItem a, MailItem b)
	{
		return b.TimeStamp.CompareTo(a.TimeStamp);
	}
}
