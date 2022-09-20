using Godot;
using System;
using static System.IO.File;

using Strl = 
	System.Collections.Generic.List<string>;

public partial class JiniFile : JiniSection
{
	public JiniFile() : base() {}
	public JiniFile(string name) : base(name) {}
	
	public override string ToString()
	{
		return ToString("");
	}
	
	public void Load(string path)
	{
		Parse(ReadLines(path).ls());
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
		Parse(Clean(l), 0);
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
}
