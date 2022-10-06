using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

public class CooldownManager
{
	public Dictionary<string, int> Cooldowns{get; set;} = new Dictionary<string, int>();
	
	public int this[string s]
	{
		get => Cooldowns.GetValueOrDefault(s,0);
		set => Cooldowns[s] = value;
	}
	
	public bool InCooldown(string s) => this[s] > 0;
	
	public void Update()
	{
		if(Cooldowns.Count == 0) return;
		var keys = Cooldowns.Keys.ToArray();
		foreach(var k in keys)
		{
			Cooldowns[k]--;
			if(Cooldowns[k] <= 0) Cooldowns.Remove(k);
		}
	}
	
	public override string ToString()
	{
		var result = new StringBuilder();
		foreach(var entry in Cooldowns) result.Append($"{entry.Key.ToString()} : {entry.Value.ToString()}\n");
		return result.ToString();
	}
}
