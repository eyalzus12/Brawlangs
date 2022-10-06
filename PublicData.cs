using Godot;
using System;
using System.Collections.Generic;
using System.Text;

public class PublicData : Node
{
	public Dictionary<string, object> Dict{get; set;} = new Dictionary<string, object>();
	
	public int Count => Dict.Count;
	public Dictionary<string, object>.KeyCollection Keys => Dict.Keys;
	public Dictionary<string, object>.ValueCollection Values => Dict.Values;
	
	public override void _Ready()
	{
		PauseMode = Node.PauseModeEnum.Process;
	}
	
	public object this[string index]
	{
		get => Dict.GetValueOrDefault(index,null);
		set => Dict[index] = value;
	}
	
	public void Add(string str, object obj) => Dict.Add(str, obj);
	public bool TryAdd(string str, object obj) => Dict.TryAdd(str, obj);
	public bool Remove(string str) => Dict.Remove(str);
	public bool HasKey(string str) => Dict.ContainsKey(str);
	public bool HasValue(object obj) => Dict.ContainsValue(obj);
	public bool TryGet(string str, out object o) => Dict.TryGetValue(str, out o);
	public object GetValueOrDefault(string str, object @default = default) => Dict.GetValueOrDefault(str, @default);

	public void Clear() => Dict.Clear();
	public bool Empty => (Count == 0);
	
	public override string ToString()
	{
		var res = new StringBuilder();
		foreach(var entry in Dict) res.Append($"{entry.Key} : {entry.Value.ToString()}\n");
		return res.ToString();
	}
}
