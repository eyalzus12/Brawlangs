using Godot;
using System;
using System.Collections.Generic;
using static System.Linq.Enumerable;

using SectionDictionary = 
	System.Collections.Generic.Dictionary<string, JiniSection>;
	
using PropertyDictionary = 
	System.Collections.Generic.Dictionary<string, object>;
	
using Strl = 
	System.Collections.Generic.List<string>;

public class JiniSection
{
	public string name = "";
	public SectionDictionary sectionDict = new SectionDictionary();
	public PropertyDictionary propertyDict = new PropertyDictionary();
	
	public int Count => (sectionDict.Count+propertyDict.Count);
	public SectionDictionary.KeyCollection SectionNames => sectionDict.Keys;
	public SectionDictionary.ValueCollection Sections => sectionDict.Values;
	public PropertyDictionary.KeyCollection PropertyNames => propertyDict.Keys;
	public PropertyDictionary.ValueCollection PropertyValues => propertyDict.Values;
	
	public JiniSection() {}
	
	public JiniSection(string name)
	{
		this.name = name;
	}
	
	public override string ToString()
	{
		return ToString("");
	}
	
	public string ToString(string tab)
	{
		string res = $"{tab}<{name}>\n{tab}{{\n";
		
		foreach(var entry in propertyDict)
			res += $"{tab}\t{entry.Key} = {entry.Value.ToString()}\n";
		
		foreach(var sec in sectionDict.Values)
			res += sec.ToString(tab+"\t");
		
		return res + $"\n{tab}}}\n";
	}
	
	/*public ReturnType this[params object[] objs]
	{
		get
		{
			object @default = objs[objs.Length - 1];
			string s = objs[0].s();
			var rt = Get(s, @default);
			for(int i = 1; i < objs.Length-1; ++i)
			{
				s=objs[i].s();
				//GD.Print(rt);
				if(rt.isObject) return rt;
				else rt = rt.SectionValue.Get(s, @default);
			}
			
			return rt;
		}
	}*/
	
	public object this[params object[] objs]
	{
		get
		{
			object @default = objs[objs.Length - 1];
			string s = objs[0].s();
			var rt = Get(s, @default);
			for(int i = 1; i < objs.Length-1; ++i)
			{
				s=objs[i].s();
				//GD.Print(rt);
				if(rt.isObject) return rt.ObjectValue;
				else rt = rt.SectionValue.Get(s, @default);
			}
			
			return rt.Value;
		}
	}
	
	public ReturnType Get(string str, object @default = null)
	{
		var rt = new ReturnType();
		
		try
		{
			rt.Value = sectionDict[str];
		}
		catch(KeyNotFoundException)
		{
			try
			{
				rt.Value = propertyDict[str];
			}
			catch(KeyNotFoundException)
			{
				rt.Value = @default;
			}
		}
		
		return rt;
	}
	
	public JiniSection GetSection(string name)
	{
		try {return sectionDict[name];}
		catch(KeyNotFoundException) {return null;}
	}
	
	public object GetProperty(string name, object @default = null)
	{
		try {return propertyDict[name];}
		catch(KeyNotFoundException) {return @default;}
	}
	
	public bool HasSection(string section)
	{
		return GetSection(section) is null;
	}
	
	public bool HasProperty(string prop)
	{
		return GetProperty(prop) is null;
	}
	
	public bool Has(string str)
	{
		try
		{
			Get(str);
			return true;
		}
		catch(KeyNotFoundException) {return false;}
	}
	
	public int Parse(Strl l, int index)
	{
		//string last = "";
		string h = "";
		int i = index;
		for(; i < l.Count; ++i)
		{
			h=l[i];
			if(h[0] == '{') continue;
			else if(h[0] == '}') break;
			else if(h[0] == '<' && h[h.Length - 1] == '>')
			{
				var sec = h.Substring(1, h.Length-2);
				var jsc = new JiniSection(sec);
				sectionDict.Add(sec, jsc);
				i = sectionDict[sec].Parse(l, i+1);
			}
			else
			{
				Store(h);
			}
		}
		
		return i;
	}
	
	private void Store(string line)
	{
		var s = line.Split('=');//two parts
		var s0 = s[0].Trim();//left part
		var s1 = s[1].Trim();//right part
		var ss1 = s1.Split(',');//item list
		var sl = new Strl();
		
		if(ss1.Length > 1)//actual list
		{
			for(int i = 0; i < ss1.Length; ++i)//going overlist
			{
				string asf = ss1[i].Trim();//take item
				if(asf[0] == '(')//starts with (
				{
					sl.Add(asf + ss1[i+1]);//add both items together
					++i;
				}
				else sl.Add(asf);
			}
		}
		else sl.Add(s1);
		
		propertyDict.Add(s0, Norm(sl));
	}
	
	private object Norm(Strl s)
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
			
			return RemoveQuotes(ss);
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
	
	private string RemoveQuotes(string s)
	{
		if(s[0] == '\"' && s[s.Length-1] == '\"')
			return s.Substring(1, s.Length-2);
		else return s;
	}
	
	public class ReturnType
	{
		protected JiniSection js = new JiniSection();
		protected object oj = new object();
		
		public ReturnType() {}
		public ReturnType(object val)
		{
			Value = val;
		}
		
		public object ObjectValue
		{
			get => oj;
			set
			{
				js = null;
				oj = value;
			}
		}
		
		public JiniSection SectionValue
		{
			get => js;
			set
			{
				oj = null;
				js = value;
			}
		}
		
		public bool isSection
		{
			get => !(js is null) && (oj is null);
		}
		
		public bool isObject
		{
			get => !(oj is null) && (js is null);
		}
		
		public object Value
		{
			get => js ?? oj;
			set
			{
				if(value is JiniSection)
				{
					js = (JiniSection)value;
					oj = null;
				}
				else
				{
					oj = value;
					js = null;
				}
			}
		}
		
		public override string ToString()
		{
			return Value.ToString();
		}
	}
}
