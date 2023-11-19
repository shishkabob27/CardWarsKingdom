using System.Collections.Generic;
using System.Text;
using JsonFx.Json;

namespace Allies
{
	public class AllyData
	{
		private string id;

		private string name;

		private string icon;

		private int level;

		private int helpcount;

		private int anonymoushelpcount;

		private int isAlly;

		private int helperCreatureID;

		private int sincelastactivedate;

		private string landscapes;

		private string helperCreature;

		public string Id
		{
			get
			{
				return id;
			}
			set
			{
				id = value;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		public string Icon
		{
			get
			{
				return icon;
			}
			set
			{
				icon = value;
			}
		}

		public int Level
		{
			get
			{
				return level;
			}
			set
			{
				level = value;
			}
		}

		public int IsAlly
		{
			get
			{
				return isAlly;
			}
			set
			{
				isAlly = value;
			}
		}

		public int HelpCount
		{
			get
			{
				return helpcount;
			}
			set
			{
				helpcount = value;
			}
		}

		public int AnonymousHelpcount
		{
			get
			{
				return anonymoushelpcount;
			}
			set
			{
				anonymoushelpcount = value;
			}
		}

		public int HelperCreatureID
		{
			get
			{
				return helperCreatureID;
			}
			set
			{
				helperCreatureID = value;
			}
		}

		public string Landscapes
		{
			get
			{
				return landscapes;
			}
			set
			{
				landscapes = value;
			}
		}

		public string HelperCreature
		{
			get
			{
				return helperCreature;
			}
			set
			{
				helperCreature = value;
			}
		}

		public int Sincelastactivedate
		{
			get
			{
				return sincelastactivedate;
			}
			set
			{
				sincelastactivedate = value;
			}
		}

		public AllyData()
		{
			id = string.Empty;
			name = "not intialized";
			icon = string.Empty;
			level = 0;
			helpcount = 0;
			anonymoushelpcount = 0;
			isAlly = 0;
			helperCreatureID = 0;
			landscapes = string.Empty;
			helperCreature = string.Empty;
			sincelastactivedate = 0;
		}

		public AllyData(Dictionary<string, object> dict)
		{
			id = dict["id"].ToString();
			name = dict["name"].ToString();
			icon = dict["icon"].ToString();
			level = TFUtils.LoadInt(dict, "level");
			helpcount = TFUtils.LoadInt(dict, "helpcount");
			anonymoushelpcount = TFUtils.LoadInt(dict, "anonymoushelpcount");
			helperCreatureID = TFUtils.LoadInt(dict, "helpercreatureid");
			helperCreature = dict["helpercreature"].ToString();
			landscapes = dict["landscapes"].ToString();
			isAlly = TFUtils.LoadInt(dict, "ally", 0);
			sincelastactivedate = TFUtils.LoadInt(dict, "sincelastactivedate");
		}

		public string Serialize()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('{');
			stringBuilder.Append(MakeJS("id", id) + ",");
			stringBuilder.Append(MakeJS("name", name) + ",");
			stringBuilder.Append(MakeJS("icon", icon) + ",");
			stringBuilder.Append(MakeJS("level", level) + ",");
			stringBuilder.Append(MakeJS("ally", isAlly) + ",");
			stringBuilder.Append(MakeJS("helpcount", helpcount) + ",");
			stringBuilder.Append(MakeJS("anonymoushelpcount", anonymoushelpcount) + ",");
			stringBuilder.Append(MakeJS("helpercreatureid", helperCreatureID) + ",");
			stringBuilder.Append(MakeJS("helpercreature", helperCreature) + ",");
			stringBuilder.Append(MakeJS("landscapes", landscapes) + ",");
			stringBuilder.Append(MakeJS("sincelastactivedate", sincelastactivedate));
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		public void Deserialize(string json)
		{
			if (!string.IsNullOrEmpty(json))
			{
				Dictionary<string, object> dictionary = JsonReader.Deserialize<Dictionary<string, object>>(json);
				id = TFUtils.LoadString(dictionary, "id", string.Empty);
				name = TFUtils.LoadString(dictionary, "name", string.Empty);
				icon = TFUtils.LoadString(dictionary, "icon", string.Empty);
				level = TFUtils.LoadInt(dictionary, "level", 0);
				isAlly = TFUtils.LoadInt(dictionary, "ally", 0);
				helpcount = TFUtils.LoadInt(dictionary, "helpcount", 0);
				anonymoushelpcount = TFUtils.LoadInt(dictionary, "anonymoushelpcount", 0);
				helperCreatureID = TFUtils.LoadInt(dictionary, "helpercreatureid", -1);
				helperCreature = TFUtils.LoadString(dictionary, "helpercreature", string.Empty);
				landscapes = TFUtils.LoadString(dictionary, "landscapes", string.Empty);
				sincelastactivedate = TFUtils.LoadInt(dictionary, "sincelastactivedate", 0);
			}
		}

		public static string MakeJS(string key, object val)
		{
			string text = "\"" + key + "\":";
			if (val is string)
			{
				return string.Concat(text, "\"", val, "\"");
			}
			if (val is bool)
			{
				return text + ((!(bool)val) ? "0" : "1");
			}
			if (val != null)
			{
				return text + val.ToString();
			}
			return text;
		}
	}
}
