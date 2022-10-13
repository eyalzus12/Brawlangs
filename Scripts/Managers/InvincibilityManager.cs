using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

public class InvincibilityManager
{
	public Dictionary<string, int> Timers{get; set;} = new Dictionary<string, int>();
	
	public int Count => Timers.Count;
	public void Add(string source, int length) => Timers.Add(source, length);
	public bool Remove(string source) => Timers.Remove(source);
	public void Has(string source) => Timers.ContainsKey(source);
	public void Clear() => Timers.Clear();
	
	public int this[string source]
	{
		get => Timers[source];
		set => Timers[source] = value;
	}
	
	public void Update()
	{
		if(Timers.Count == 0) return;
		var keys = Timers.Keys.ToList();
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
