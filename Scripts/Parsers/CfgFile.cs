using Godot;
using System;
using System.Collections.Generic;
using static System.IO.File;

using CfgDictionary =
System.Collections.Generic.Dictionary<string, object>;
	
using Strl = System.Collections.Generic.List<string>;


//TODO: update to match changes to IniFile
public class CfgFile
{
	public CfgDictionary dict;
	
	public CfgFile() {dict = new CfgDictionary();}
	public void Reset() {dict = new CfgDictionary();}
	
	public int Count => dict.Count;
	public CfgDictionary.KeyCollection Keys => dict.Keys;
	public CfgDictionary.ValueCollection Values => dict.Values;
	
	public object this[string key, object @default = null]
	{
		get
		{
			try {return dict[key];}
			catch(KeyNotFoundException) {return @default;}
		}
	}
	
	public override string ToString()
	{
		string res = "";
		
		foreach(var entry in dict)
			res += $"{entry.Key} = {entry.Value.ToString()}\n";
		
		return res;
	}
	
	public bool HasKey(string key) => dict.ContainsKey(key);
	public bool HasValue(object val) => dict.ContainsValue(val);
	
	public void Load(string path)
	{
		File f = new File();
		f.Open(path, File.ModeFlags.Read);
		string content = f.GetAsText();
		f.Close();
		Parse(content);
	}
	
	public void Parse(string str)
	{
		Parse(str.Split('\n'));
	}
	
	public void Parse(string[] str)
	{
		Parse(new Strl(str));
	}
	
	public void Parse(Strl l)
	{
		var ll = Clean(l);
		foreach(var h in ll)
		{
			Store(h);
		}
	}
	
	private void Store(string line)
	{
		var s = line.Split('=');//split to parts
		var s0 = s[0].Trim();//left side
		var s1 = s[1].Trim();//right side
		var ss1 = s1.Split(','); //try split by list
		var sl = new Strl();
		
		if(ss1.Length > 1)//if an actual list
		{
			//create list
			for(int i = 0; i < ss1.Length; ++i)
			{
				string asf = ss1[i].Trim();
				if(asf[0] == '(')
				{
					sl.Add(asf + ss1[i+1]);
					++i;
				}
				else sl.Add(asf);
			}
		}
		else sl.Add(s1);//just add it to the list
		
		dict.Add(s0, Norm(sl));
	}
	
	private object Norm(Strl s)//turn to real values
	{
		var lo = new List<object>();
		if(s.Count > 1) 
		{
			foreach(var i in s) lo.Add(Norm(i));
			return lo;
		}
		else
		{
			var ss = s[0];
			
			var res = 0f;
			if(float.TryParse(ss, out res)) return res;
			
			var v = Vector2.Zero;
			if(s2v(ss, ref v)) return v;
			
			return RemQuotes(ss);
		}
	}
	
	private object Norm(string s)
	{
		var l = new Strl();
		l.Add(s);
		return Norm(l);
	}
	
	private bool s2v(string s, ref Vector2 v)
	{
		if(s[0] != ')' || s[s.Length-1] != '(') return false;
		else
		{
			string[] ss = s.Split(',');
			float x = float.Parse(ss[0].Substring(1));
			float y = float.Parse(ss[1].Substring(0, ss[1].Length-2));
			v.x = x;
			v.y = y;
			return true;
		}
	}
	
	private Strl Clean(Strl l)
	{
		Strl ll = new Strl();
		
		foreach(var s in l)
		{
			string h = Clean(s);
			if(h != "") ll.Add(h);
		}
		
		return ll;
	}
	
	private string Clean(string s)
	{
		string h = "";
		h = s.Split(';')[0];//removes comments
		h = h.Trim();//removes trailing whitespace
		return h;
	}
	
	private string RemQuotes(string s)
	{
		if(s[0] == '\"' && s[s.Length-1] == '\"')
			return s.Substring(1, s.Length-2);
		else return s;
	}
}
