using Godot;
using System;
using System.Collections.Generic;
using System.Text;

public class PublicData : Node
{
	public Dictionary<string, object> dict;
	
	public PublicData() => Reset();
	public void Reset() => dict = new Dictionary<string, object>();
	
	public int Count => dict.Count;
	public Dictionary<string, object>.KeyCollection Keys => dict.Keys;
	public Dictionary<string, object>.ValueCollection Values => dict.Values;
	
	public object this[string index]
	{
		get
		{
			try {return dict[index];}
			catch(KeyNotFoundException) {return null;}
		}
		
		set
		{
			try{dict[index] = value;}
			catch(KeyNotFoundException) {}
		}
	}
	
	public void Add(string str, object obj)
	{
		try {dict.Add(str, obj);}
		catch(ArgumentException) {}
	}
	
	public void AddOverride(string str, object obj)
	{
		try {dict.Add(str, obj);}
		catch(ArgumentException) {dict[str]=obj;}
	}
	
	public bool Remove(string str) => dict.Remove(str);
	public bool HasKey(string str) => dict.ContainsKey(str);
	public bool HasValue(object obj) => dict.ContainsValue(obj);
	public bool TryGet(string str, out object o) => dict.TryGetValue(str, out o);
	public object GetOrDefault(string str, object @default)
	{
		try {return dict[str];}
		catch(KeyNotFoundException) {return @default;}
	}
	public void Clear() => dict.Clear();
	public bool Empty => (Count == 0);
	
	public override string ToString()
	{
		var res = new StringBuilder();
		foreach(var entry in dict) res.Append($"{{{entry.Key}, {entry.Value.ToString()}}}\n");
		return res.ToString();
	}
}
