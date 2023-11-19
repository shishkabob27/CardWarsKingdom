using System;
using System.Collections.Generic;

public class CustomAIData : ILoadableData
{
	public class Row
	{
		public int RepeatCooldown;

		public List<FunctionCall> Conditions = new List<FunctionCall>();

		public List<FunctionCall> Actions = new List<FunctionCall>();
	}

	public class FunctionCall
	{
		public string FunctionName;

		public List<string> Parameters = new List<string>();

		public bool Negated;

		public static FunctionCall Parse(string jsonString)
		{
			FunctionCall functionCall = new FunctionCall();
			int num = jsonString.IndexOf('(');
			int num2 = jsonString.IndexOf(')');
			if (num == -1 || num2 == -1)
			{
				throw new Exception();
			}
			functionCall.FunctionName = jsonString.Substring(0, num);
			string text = jsonString.Substring(num + 1, num2 - num - 1);
			string[] array = text.Split(',');
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				string text3 = text2.Trim();
				if (text3 != string.Empty)
				{
					functionCall.Parameters.Add(text3);
				}
			}
			return functionCall;
		}

		public string GetParam(int index)
		{
			if (Parameters.Count > index)
			{
				return Parameters[index];
			}
			throw new Exception("parameter " + (index + 1) + " not found");
		}

		public string GetParam(int index, string defaultValue)
		{
			if (Parameters.Count > index)
			{
				return Parameters[index];
			}
			return defaultValue;
		}
	}

	public const int NO_REPEAT = -99;

	private List<Row> mRows = new List<Row>();

	public string ID { get; private set; }

	public List<Row> Rows
	{
		get
		{
			return mRows;
		}
	}

	public void Populate(Dictionary<string, object> dict)
	{
		ID = TFUtils.LoadString(dict, "ID", string.Empty);
		AddRow(dict);
	}

	public void AddRow(Dictionary<string, object> dict)
	{
		Row row = new Row();
		mRows.Add(row);
		row.RepeatCooldown = TFUtils.LoadInt(dict, "RepeatCooldown", -99);
		string text = TFUtils.LoadString(dict, "Conditions", string.Empty);
		string[] array = text.Split(';');
		string[] array2 = array;
		foreach (string text2 in array2)
		{
			if (text2 == string.Empty)
			{
				continue;
			}
			try
			{
				bool negated = false;
				string text3 = text2.Trim();
				if (text3.StartsWith("!"))
				{
					text3 = text3.Substring(1, text2.Length - 1).Trim();
					negated = true;
				}
				FunctionCall functionCall = FunctionCall.Parse(text3);
				functionCall.Negated = negated;
				row.Conditions.Add(functionCall);
			}
			catch (Exception)
			{
			}
		}
		string text4 = TFUtils.LoadString(dict, "Actions", string.Empty);
		string[] array3 = text4.Split(';');
		string[] array4 = array3;
		foreach (string text5 in array4)
		{
			if (!(text5 == string.Empty))
			{
				try
				{
					FunctionCall item = FunctionCall.Parse(text5.Trim());
					row.Actions.Add(item);
				}
				catch (Exception)
				{
				}
			}
		}
	}
}
