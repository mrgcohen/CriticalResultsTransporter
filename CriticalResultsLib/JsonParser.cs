using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace CriticalResults
{
	public static class ContextParser
	{
		public static Dictionary<string,object> ParseContext(string json)
		{
			json = json.Substring(1, json.Length - 2);
			Dictionary<string,object> dict = Parse(json);
			return dict;
		}

		private static Dictionary<string, object> Parse(string json)
		{
			Dictionary<string,object> table = new Dictionary<string, object>();

			List<char> ca = new List<char>();
			ca.AddRange(json.ToCharArray());

			for (int i = 0; i < ca.Count; i++)
			{
				if (ca[i] == ':')
				{
					KeyValuePair<string, object> kvp = ParseKVP(ca.ToArray(), ref i);
					table.Add(kvp.Key, kvp.Value);
				}
			}

			return table;
		}

		private static KeyValuePair<string, object> ParseKVP(char[] ca, ref int location)
		{
			string name = "";
			int parencount = 0;
			for (int i = location; i > 0; i--)
			{
				if (ca[i] == '\"')
				{
					parencount++;
				}
				else if (ca[i] == ':') { }
				else
				{
					name = ca[i] + name;
				}
				if (parencount == 2)
				{
					break;
				}
			}


			string value = "";
			parencount = 0;
			location++;
			while (ca[location] == ' ')
			{
				location++;
			}

			if (ca[location] == '{')
			{
				int closingParen = FindClosingBraceLocation(ca, location);
				string sub = new string(ca).Substring(location, closingParen - location + 1);
				location = closingParen;
				Dictionary<string, object> table = Parse(sub);
				return new KeyValuePair<string, object>(name, table);
			}
			else
			{
				for (; location < ca.Length; location++)
				{
					if (ca[location] == '\"')
					{
						parencount++;
					}
					else
					{
						value = value + ca[location];
					}
					if (parencount == 2)
					{
						break;
					}
				}
				return new KeyValuePair<string, object>(name, value);
			}
		}

		private static int FindClosingBraceLocation(char[] ca, int startLocation)
		{
			int parenCount = 0;
			for (int i = startLocation; i < ca.Length; i++)
			{
				if (ca[i] == '{')
				{
					parenCount++;
				}
				else if (ca[i] == '}')
				{
					parenCount--;
				}
				if (parenCount == 0)
				{
					return i;
				}
			}
			return -1;
		}

		public static object GetValueFromHash(Hashtable table, string key)
		{
			object value = "Key not found";
			foreach (object dictKey in table.Keys)
			{
				if (key == dictKey as string)
				{
					return ((Hashtable)table[dictKey])["value"];
				}
				else if (table[dictKey].GetType() == typeof(Dictionary<string, object>))
				{
					value = GetValueFromHash(table[dictKey] as Hashtable, key);
				}
			}
			return value;
		}


	}

}
