using Godot;
using System;
using System.Collections.Generic;
using System.Text;

public class InvincibilityManager
{
	public Dictionary<string, int> Timers{get; set;} = new Dictionary<string, int>();
	
	public int Count => Timers.Count;
	public void Add(string source, int length) => Timers.Add(source, length);
	public void Remove(string source) => Timers.Remove(source);
	public void Has(string source) => Timers.ContainsKey(source);
	public void Clear() => Timers.Clear();
	
	public int this[string source]
	{
		get => Timers[source];
		set => Timers[source] = value;
	}
	
	public void Update()
	{
		var keys = new List<string>(Timers.Keys);
		foreach(var k in keys)
		{
			this[k]--;
			if(this[k] <= 0) Remove(k);
		}
	}
	
	public override string ToString()
	{
		var result = new StringBuilder();
		foreach(var entry in Timers) result.Append($"{entry.Key.ToString()} : {entry.Value.ToString()}\n");
		return result.ToString();
	}
}
