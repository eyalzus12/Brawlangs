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
	public IniDictionary dict;
	public string filePath;
	public File file;
	
	public IniFile() => Reset();
	
	public void Reset()
	{
		dict = new IniDictionary();
		dict.Add("", new Dictionary<string,object>());
	}
	
	public int Count => dict.Count;
	public IniDictionary.KeyCollection Keys => dict.Keys;
	public IniDictionary.ValueCollection Values => dict.Values;
	
	public Dictionary<string, object> this[string section] => dict[section];
	public object this[string section, string key] => dict[section][key];
	
	public object this[string section, string key, object @default]
	{
		get
		{
			if(dict.ContainsKey(section) && dict[section].ContainsKey(key)) return dict[section][key];
			else return @default;
		}
	}
	
	public override string ToString()
	{
		var res = new StringBuilder();
		foreach(var h in dict.Keys) res.Append($"[{h}]\n{k2s(h)}");
		return res.ToString();
	}
	
	public object GetValue(string section, string key, object @default = null) => this[section, key, @default];
	public bool HasSection(string section) => dict.ContainsKey(section);
	
	private string k2s(string key)
	{
		var res = new StringBuilder();
		foreach(var entry in dict[key]) res.Append($"{entry.Key} = {entry.Value.ToString()}\n");
		return res.ToString();
	}
	
	public Error Load(string path)
	{
		File f = new File();//create new file
		var er = f.Open(path, File.ModeFlags.Read);//open file
		if(er != Error.Ok) return er;//if error, return
		filePath = path;
		file = f;
		string content = f.GetAsText();//read text
		f.Close();//flush buffer
		Parse(content);//parse
		return Error.Ok;
		//everything went well
		//unless there was a parse error
		//in which case an error was thrown anyways
	}
	
	public void Parse(string str) => Parse(str.Split('\n'));
	public void Parse(string[] str) => Parse(new Strl(str));
	
	public void Parse(Strl l)
	{
		var ll = Clean(l);
		var section = "";
		foreach(var s in ll)
		{
			if(s[0] == '[' && s[s.Length - 1] == ']')
			{
				section = s.Substring(1, s.Length-2);
				dict.Add(section, new Dictionary<string, object>());
			}
			else
			{
				Store(section, s);
			}
		}
	}
	
	private void Store(string key, string line)
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
		
		dict[key].Add(left, Norm(store));
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
				/*var lo = new List<object>();
				foreach(var s in l) lo.Add(Norm(s));
				return lo;*/
		}
	}
	
	private object Norm(string s)
	{
		int i;
		float f;
		Vector2 v2;
		Vector3 v3;
		Quat q;
			
		if(string.Equals(s, "true",  StringComparison.OrdinalIgnoreCase)) return true;//represents true
		else if(string.Equals(s, "false",  StringComparison.OrdinalIgnoreCase)) return false;//represents false
		else if(int.TryParse(s, out i)) return i;//represents a number
		else if(float.TryParse(s, out f)) return f;//represents a float
		else if(s2v2(s, out v2)) return v2;//represents a Vector2
		else if(s2v3(s, out v3)) return v3;//represents a Vector3
		else if(s2q(s, out q)) return q;//represents a Quat
		else return s.Trim('\"');//represents a string
	}
	
	private bool s2v2(string s, out Vector2 v)
	{
		var st = s.Trim();
		if(st[0] != '(' || st[st.Length-1] != ')')
		{
			v = default;
			return false;
		}
		else
		{
			string[] ss = st.Substring(1, st.Length-2).Split(',');
			if(ss.Length != 2)
			{
				v = default;
				return false;
			}
			float x = float.Parse(ss[0].Trim());
			float y = float.Parse(ss[1].Trim());
			v = new Vector2(x, y);
			return true;
		}
	}
	
	private bool s2v3(string s, out Vector3 v)
	{
		var st = s.Trim();
		if(st[0] != '(' || st[st.Length-1] != ')')
		{
			v = default;
			return false;
		}
		else
		{
			string[] ss = st.Substring(1, st.Length-2).Split(',');
			if(ss.Length != 3)
			{
				v = default;
				return false;
			}
			float x = float.Parse(ss[0].Trim());
			float y = float.Parse(ss[1].Trim());
			float z = float.Parse(ss[2].Trim());
			v = new Vector3(x, y, z);
			return true;
		}
	}
	
	private bool s2q(string s, out Quat q)
	{
		if(s[0] != '(' || s[s.Length-1] != ')')
		{
			q = default;
			return false;
		}
		else
		{
			string[] ss = s.Substring(1, s.Length-2).Split(',');
			if(ss.Length != 4)
			{
				q = default;
				return false;
			}
			float x = float.Parse(ss[0].Trim());
			float y = float.Parse(ss[1].Trim());
			float z = float.Parse(ss[2].Trim());
			float w = float.Parse(ss[3].Trim());
			q = new Quat(x, y, z, w);
			return true;
		}
	}
	
	private Strl Clean(Strl l) => l.Select(Clean).Where(s => s != "").ToList<string>();
	/*{
		Strl ll = new Strl();
		
		foreach(var s in l)
		{
			string h = Clean(s);
			if(h != "") ll.Add(h);
		}
		
		return ll;
	}*/
	
	private string Clean(string s) => s.Split(';')[0].Trim();
}
