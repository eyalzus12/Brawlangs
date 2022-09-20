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
	public CfgDictionary dict;
	
	public CfgFile() => Reset();
	public void Reset() {dict = new CfgDictionary();}
	
	public int Count => dict.Count;
	public CfgDictionary.KeyCollection Keys => dict.Keys;
	public CfgDictionary.ValueCollection Values => dict.Values;
	
	public object this[string key, object @default = null]
	{
		get
		{
			if(dict.ContainsKey(key)) return dict[key];
			else return @default;
		}
	}
	
	public override string ToString()
	{
		var res = new StringBuilder();
		foreach(var entry in dict) res.Append($"{entry.Key} = {entry.Value.ToString()}\n");
		return res.ToString();
	}
	
	public bool HasKey(string key) => dict.ContainsKey(key);
	public bool HasValue(object val) => dict.ContainsValue(val);
	
	public Error Load(string path)
	{
		var f = new File();//create new file
		if(!File.FileExists(path))
		{
			GD.PushError($"failed to load cfg file {path}, as it does not exist");
			return Error.FileNotFound;
		}
		var er = f.Open(path, File.ModeFlags.Read);//open file
		if(er != Error.Ok) return er;//if error, return
		var content = f.GetAsText();//read text
		f.Close();//flush buffer
		Parse(content);//parse
		return Error.Ok;
		//everything went well
		//unless there was a parse error
		//in which case an error was thrown anyways
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
						}//end of Vector4 else
					}//end of Vector3 else
				}//end of Vector check
				else store.Add(element);
			}//end of for loop
		}//end of list check
		else store.Add(right);
		
		dict.Add(left, Norm(store));
	}
	
	private object Norm(Strl l) => l.Count switch
	{
		0 => null,
		1 => Norm(l[0]),
		_ => l.Select(Norm).ToList(),
	};
	
	private object Norm(string s)
	{
		int i;
		float f;
		Vector2 v2;
		Vector3 v3;
		Vector4 v4;
			
		if(string.Equals(s, "true",  StringComparison.OrdinalIgnoreCase)) return true;//represents true
		else if(string.Equals(s, "false",  StringComparison.OrdinalIgnoreCase)) return false;//represents false
		else if(int.TryParse(s, out i)) return i;//represents a number
		else if(float.TryParse(s, out f)) return f;//represents a float
		else if(StringUtils.TryParseVector2(s, out v2)) return v2;//represents a Vector2
		else if(StringUtils.TryParseVector3(s, out v3)) return v3;//represents a Vector3
		else if(StringUtils.TryParseVector4(s, out v4)) return v4;//represents a Vector4
		else return s.Trim('\"');//represents a string
	}
	
	private Strl Clean(Strl l) => l.Select(Clean).Where(s => s != "").ToList<string>();
	private string Clean(string s) => s.Split(';')[0].Trim();
}
