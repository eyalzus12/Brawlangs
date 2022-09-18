using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Godict = Godot.Collections.Dictionary;

public partial class PropertyMap
{
	public Dictionary<string, object> dict;
	
	public PropertyMap() => Reset();
	public void Reset() => dict = new();
	
	public int Count => dict.Count;
	public Dictionary<string, object>.KeyCollection Keys => dict.Keys;
	public Dictionary<string, object>.ValueCollection Values => dict.Values;
	
	public object this[string index] => dict[index];
	
	public void Add(string str, object obj) => dict.Add(str, obj);
	
	public void LoadProperties(Godot.Object obj)
	{
		foreach(var entry in dict)
			obj.Set(entry.Key, Variant.CreateFrom((dynamic)entry.Value));
	}
	
	public void StoreProperties(Godot.Object obj, List<string> propertyList)
	{
		foreach(Godict d in obj.GetPropertyList())
		{
			var name = d["name"].s();
			if(propertyList.Contains(name)) Add(name, obj.Get(name));
		}
	}
	
	public bool ReadFromConfigFile(string path, string sectionName)
	{
		var cfg = new CfgFile();
		
		if(cfg.Load(path) != Error.Ok)
		{
			GD.PushError($"failed to read property file \"{path}\"");
			return false;
		}
		
		dict = cfg.dict;
		return true;
	}
	
	public bool ConfigFileToPropertyList(Godot.Object obj, string path, 
		string sectionName, List<string> propertyList)
	{
		var res = ReadFromConfigFile(path, sectionName);
		propertyList = dict.Keys.ToList();
		return res;
	}
	
	public override string ToString()
	{
		var res = new StringBuilder();
		foreach(var entry in dict)
			res.Append($"{{{entry.Key}, {entry.Value.ToString()}}}\n");
		return res.ToString();
	}
}
