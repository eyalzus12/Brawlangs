using Godot;
using System;
using System.Collections.Generic;

public class PropertyMap
{
	Dictionary<string, object> dict;
	
	public PropertyMap() => Reset();
	public void Reset() => dict = new Dictionary<string, object>();
	
	public int Count => dict.Count;
	public Dictionary<string, object>.KeyCollection Keys => dict.Keys;
	public Dictionary<string, object>.ValueCollection Values => dict.Values;
	
	public object this[string index] => dict[index];
	
	public void Add(string str, object obj) => dict.Add(str, obj);
	
	public void LoadProperties(Godot.Object obj)
	{
		//TODO?: use linq ForEach
		foreach(var entry in dict)
			obj.Set(entry.Key, entry.Value);
	}
	
	public void StoreProperties(Godot.Object obj, List<string> propertyList)
	{
		foreach(Godot.Collections.Dictionary d in obj.GetPropertyList())
		{
			string name = d["name"] as string;
			if(propertyList.Contains(name)) Add(name, obj.Get(name));
		}
	}
	
	public bool ReadFromConfigFile(string path, string sectionName)
	{
		IniFile ini = new IniFile();
		
		if(ini.Load(path) != Error.Ok)
		{
			GD.Print($"failed to read property file \"{path}\"");
			return false;
		}
		
		foreach(string key in ini[sectionName].Keys)
			Add(key, ini[sectionName, key]);
			
		return true;
	}
	
	public bool ConfigFileToPropertyList(Godot.Object obj, string path, 
		string sectionName, List<string> propertyList)
	{
		bool res = ReadFromConfigFile(path, sectionName);
		
		//TODO?: use linq ToList
		foreach(string k in dict.Keys)
			propertyList.Add(k);
			
		return res;
	}
	
	public override string ToString()
	{
		//TODO: use a StringBuilder
		var res = "";
		
		foreach(var entry in dict)
		{
			res += "{" + 
						entry.Key + ", " +
						entry.Value.ToString() +
					"}\n";
		}
		
		return res;
	}
}
