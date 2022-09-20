using Godot;
using System;
using System.Collections.Generic;
using System.Text;

public class CooldownManager
{
	public Dictionary<string, int> Cooldowns{get; set;} = new();
	
	public int this[string s]
	{
		get => Cooldowns.GetValueOrDefault(s,0);
		set => Cooldowns[s] = value;
	}
	
	public bool InCooldown(string s) => this[s] > 0;
	
	public void Update()
	{
		List<string> keys = new(Cooldowns.Keys);
		foreach(var k in keys)
		{
			Cooldowns[k]--;
			if(Cooldowns[k] <= 0) Cooldowns.Remove(k);
		}
	}
	
	public override string ToString()
	{
		StringBuilder result = new();
		foreach(var entry in Cooldowns) result.Append($"{entry.Key.ToString()} : {entry.Value.ToString()}\n");
		return result.ToString();
	}
}
