using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IniDictionary =
System.Collections.Generic
	.Dictionary<string, 
		System.Collections.Generic
	.Dictionary<string, object>>;
	
using Strl = System.Collections.Generic.List<string>;

public class IniFile
{
	public IniDictionary Dict{get; set;} = new IniDictionary();
	public string FilePath{get; set;}
	
	public IniFile() => Reset();
	
	public void Reset()
	{
		Dict = new IniDictionary();
		Dict.Add("", new Dictionary<string,object>());
	}
	
	public int Count => Dict.Count;
	public IniDictionary.KeyCollection Keys => Dict.Keys;
	public IniDictionary.ValueCollection Values => Dict.Values;
	
	public Dictionary<string, object> this[string section] => Dict[section];
	public object this[string section, string key] => Dict[section][key];
	
	public object this[string section, string key, object @default] => Dict.GetValueOrDefault(section,null)?.GetValueOrDefault(key,@default)??@default;
	
	public override string ToString()
	{
		var res = new StringBuilder();
		foreach(var section in Dict)
		{
			res.Append($"[{section.Key}]\n");
			foreach(var entry in section.Value) res.Append($"{entry.Key} = {entry.Value.ToString()}\n");
		}
		return res.ToString();
	}
	
	public object GetValue(string section, string key, object @default = null) => this[section, key, @default];
	public bool HasSection(string section) => Dict.ContainsKey(section);
	
	public Error Load(string path)
	{
		string content;
		var er = Utils.ReadFile(path, out content);
		if(er != Error.Ok) return er;//if error, return
		FilePath = path;
		Parse(content);//parse
		return Error.Ok;
	}
	
	public void Parse(string str) => Parse(str.Split('\n'));
	public void Parse(string[] str) => Parse(new Strl(str));
	
	public void Parse(Strl l)
	{
		var section = "";
		for(int i = 0; i < l.Count; ++i)
		{
			var s = Clean(l[i]);
			if(s == "") continue;
			if(s[0] == '[')
			{
				if(s[s.Length - 1] != ']') throw new FormatException($"File {FilePath}: Opening [ without closing ] found in line {i}");
				section = s.Substring(1, s.Length-2);
				if(Dict.ContainsKey(section)) throw new FormatException($"File {FilePath}: Duplicate section name {section} in line {i}");
				Dict.Add(section, new Dictionary<string, object>());
			}
			else
			{
				Store(section, s, i);
			}
		}
	}
	
	private void Store(string key, string line, int lineCount)
	{
		var leftright = line.Split('=');//split to left side and right side
		if(leftright.Length != 2) throw new FormatException($"File {FilePath}: Improper amount of =s in line {lineCount}");
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
						if(parts.Length <= 2) throw new FormatException($"File {FilePath}: No closing ) found in line {lineCount}");
						var trim2 = parts[i+2].Trim();//get third element
						if(trim2[trim2.Length-1] == ')')//is really a vector
						{
							store.Add(element + ',' + trim1 + ',' + trim2);//add
							i += 2;
						}
						else
						{
							if(parts.Length <= 3) throw new FormatException($"File {FilePath}: No closing ) found in line {lineCount}");
							var trim3 = parts[i+3].Trim();//get fourth element
							if(trim3[trim3.Length-1] == ')')//is really a vector
							{
								store.Add(element + ',' + trim1 + ',' + trim2 + ',' + trim3);//add
								i += 3;
							}
							else
							{
								throw new FormatException($"File {FilePath}: Too many arguments in vector in line {lineCount}");
							}
						}//end of Quat else
					}//end of Vector3 else
				}//end of Vector check
				else store.Add(element);
			}//end of for loop
		}//end of list check
		else store.Add(right);
		
		if(Dict[key].ContainsKey(left)) throw new FormatException($"File {FilePath}: Duplicate property {left} in line {lineCount}");
		Dict[key].Add(left, Norm(store));
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
	
	private string Clean(string s) => s.Split(';')[0].Trim();
}
