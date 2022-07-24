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
			if(dict.ContainsKey(index)) return dict[index];
			else return null;
		}
		
		set
		{
			if(dict.ContainsKey(index)) dict[index] = value;
		}
	}
	
	public new object Get(string s) => this[s];
	public T Get<T>(string s) => (T)Get(s);
	
	public void Add(string str, object obj)
	{
		if(!dict.ContainsKey(str)) dict.Add(str, obj);
	}
	
	public void AddOverride(string str, object obj)
	{
		if(dict.ContainsKey(str)) dict[str]=obj;
		else dict.Add(str, obj);
	}
	
	public bool Remove(string str) => dict.Remove(str);
	public bool HasKey(string str) => dict.ContainsKey(str);
	public bool HasValue(object obj) => dict.ContainsValue(obj);
	public bool TryGet(string str, out object o) => dict.TryGetValue(str, out o);
	
	public bool TryGet<T>(string str, out T t)
	{
		object o;
		var res = dict.TryGetValue(str, out o);
		t = (T)o;
		return res;
	}
	
	public object GetOrDefault(string str, object @default)
	{
		if(dict.ContainsKey(str)) return dict[str];
		else return @default;
	}
	
	//public T GetOrDefault<T>(string str, T @default) => (T)GetOrDefault(str, @default);
	public void Clear() => dict.Clear();
	public bool Empty => (Count == 0);
	
	public override string ToString()
	{
		var res = new StringBuilder();
		foreach(var entry in dict) res.Append($"{entry.Key} : {entry.Value.ToString()}\n");
		return res.ToString();
	}
}
