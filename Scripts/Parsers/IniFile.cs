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
	
	public IniFile() => dict = new IniDictionary();
	public void Reset() => dict = new IniDictionary();
	
	public int Count => dict.Count;
	public IniDictionary.KeyCollection Keys => dict.Keys;
	public IniDictionary.ValueCollection Values => dict.Values;
	
	public Dictionary<string, object> this[string section] => dict[section];
	public object this[string section, string key] => dict[section][key];
	
	public object this[string section, string key, object @default]
	{
		get
		{
			try {return dict[section][key];}
			catch(KeyNotFoundException) {return @default;}
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
		var s = line.Split('=');//split to left side and right side
		var s0 = s[0].Trim();//left side
		var s1 = s[1].Trim();//right side
		var ss1 = s1.Split(',');//split right side to parts by commas
		var sl = new Strl();
		
		if(ss1.Length > 1)//ensure actual list
		{
			for(int i = 0; i < ss1.Length; ++i)//go over thee
			{
				var asf = ss1[i].Trim();//get first element
				if(asf[0] == '(')//it actually a vector
				{
					var trim1 = ss1[i+1].Trim();//get second element
					if(trim1[trim1.Length-1] == ')')//is really a vector
						sl.Add(asf + ',' + trim1);//add
					else
					{
						var trim2 = ss1[i+2].Trim();//get third element
						if(trim2[trim2.Length-1] == ')')//is really a vector
							sl.Add(asf + ',' + trim1 + ',' + trim2);//add
						else
						{
							var trim3 = ss1[i+3].Trim();//get fourth element
							if(trim3[trim3.Length-1] == ')')//is really a vector
								sl.Add(asf + ',' + trim1 + ',' + trim2 + ',' + trim3);//add
						}
					}
					++i;
				}
				else sl.Add(asf);
			}
		}
		else sl.Add(s1);
		
		dict[key].Add(s0, Norm(sl));
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
		Vector2 v2 = default;
		Vector3 v3 = default;
		Quat q = default;
			
		if(string.Equals(s, "true",  StringComparison.OrdinalIgnoreCase)) return true;//represents true
		else if(string.Equals(s, "false",  StringComparison.OrdinalIgnoreCase)) return false;//represents false
		else if(int.TryParse(s, out i)) return i;//represents a number
		else if(float.TryParse(s, out f)) return f;//represents a float
		else if(s2v2(s, ref v2)) return v2;//represents a Vector2
		else if(s2v3(s, ref v3)) return v3;//represents a Vector3
		else if(s2q(s, ref q)) return q;//represents a Quat
		else return s.Trim('\"');//represents a string
	}
	
	private bool s2v2(string s, ref Vector2 v)
	{
		var st = s.Trim();
		if(st[0] != '(' || st[st.Length-1] != ')') return false;
		else
		{
			string[] ss = st.Substring(1, st.Length-2).Split(',');
			if(ss.Length != 2) return false;
			float x = float.Parse(ss[0].Trim());
			float y = float.Parse(ss[1].Trim());
			v = new Vector2(x, y);
			return true;
		}
	}
	
	private bool s2v3(string s, ref Vector3 v)
	{
		var st = s.Trim();
		if(st[0] != '(' || st[st.Length-1] != ')') return false;
		else
		{
			string[] ss = st.Substring(1, st.Length-2).Split(',');
			if(ss.Length != 3) return false;
			float x = float.Parse(ss[0].Trim());
			float y = float.Parse(ss[1].Trim());
			float z = float.Parse(ss[2].Trim());
			v = new Vector3(x, y, z);
			return true;
		}
	}
	
	private bool s2q(string s, ref Quat q)
	{
		if(s[0] != ')' || s[s.Length-1] != '(') return false;
		else
		{
			string[] ss = s.Substring(1, s.Length-2).Split(',');
			if(ss.Length != 4) return false;
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
