using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using CfgDictionary =
System.Collections.Generic.Dictionary<string, object>;
	
using Strl = System.Collections.Generic.List<string>;


//TODO: update to match changes to IniFile
public class CfgFile
{
	public CfgDictionary Dict{get; set;} = new CfgDictionary();
	
	public CfgFile() {}
	
	public int Count => Dict.Count;
	public CfgDictionary.KeyCollection Keys => Dict.Keys;
	public CfgDictionary.ValueCollection Values => Dict.Values;
	
	public object this[string key, object @default = null] => Dict.GetValueOrDefault(key, @default);
	
	public override string ToString()
	{
		var res = new StringBuilder();
		foreach(var entry in Dict) res.Append($"{entry.Key} = {entry.Value.ToString()}\n");
		return res.ToString();
	}
	
	public bool ContainsKey(string key) => Dict.ContainsKey(key);
	public bool ContainsValue(object val) => Dict.ContainsValue(val);
	
	public Error Load(string path)
	{
		string content;
		var er = Utils.ReadFile(path, out content);
		if(er != Error.Ok) return er;//if error, return
		Parse(content);//parse
		return Error.Ok;
	}
	
	public void Parse(string str) => Parse(str.Split('\n'));
	public void Parse(string[] str) => Parse(new Strl(str));
	public void Parse(Strl l) => Clean(l).ForEach(Store);
	
	private void Store(string line)
	{
		var leftright = line.Split('=');//split to left side and right side
		var left = leftright[0].Trim();//left side
		var right = leftright[1].Trim();//right side
		var parts = right.Split(',');//split right side to parts by commas
		var store = new Strl();
		
		if(parts.Length > 1)//ensure actual list
		{
			for(int i = 0; i < parts.Length; ++i)//go over thee
			{
				var element = parts[i].Trim();//get first element
				if(element[0] == '(')//probably a vector
				{
					var trim1 = parts[i+1].Trim();//get second element
					if(trim1[trim1.Length-1] == ')')//is really a vector
					{
						store.Add(element + ',' + trim1);//add
						i += 1;
					}
					else
					{
						var trim2 = parts[i+2].Trim();//get third element
						if(trim2[trim2.Length-1] == ')')//is really a vector
						{
							store.Add(element + ',' + trim1 + ',' + trim2);//add
							i += 2;
						}
						else
						{
							var trim3 = parts[i+3].Trim();//get fourth element
							if(trim3[trim3.Length-1] == ')')//is really a vector
							{
								store.Add(element + ',' + trim1 + ',' + trim2 + ',' + trim3);//add
								i += 3;
							}
						}//end of Quat else
					}//end of Vector3 else
				}//end of Vector check
				else store.Add(element);
			}//end of for loop
		}//end of list check
		else store.Add(right);
		
		Dict.Add(left, Norm(store));
	}
	
	private object Norm(Strl l)
	{
		switch(l.Count)
		{
			case 0:
				return null;
			case 1:
				return Norm(l[0]);
			default:
				return l.Select(Norm).ToList();
		}
	}
	
	private object Norm(string s)
	{
		int i;
		float f;
		Vector2 v2;
		Vector3 v3;
		Quat q;
		
		if(s.EqualsIgnoreCase("true")) return true;
		else if(s.EqualsIgnoreCase("false")) return false;
		else if(int.TryParse(s, out i)) return i;//represents a number
		else if(float.TryParse(s, out f)) return f;//represents a float
		else if(StringUtils.TryParseVector2(s, out v2)) return v2;//represents a Vector2
		else if(StringUtils.TryParseVector3(s, out v3)) return v3;//represents a Vector3
		else if(StringUtils.TryParseQuat(s, out q)) return q;//represents a Quat
		else return s.Trim('\"');//represents a string
	}
	
	private Strl Clean(Strl l) => l.Select(Clean).Where(s => s != "").ToList<string>();
	private string Clean(string s) => s.Split(';')[0].Trim();
}
