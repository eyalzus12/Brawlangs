using Godot;
using System;
using System.Collections.Generic;
using System.Text;

public class CooldownManager
{
	public Dictionary<string, int> Cooldowns{get; set;} = new Dictionary<string, int>();
	
	public int this[string s]
	{
		get
		{
			if(Cooldowns.ContainsKey(s)) return Cooldowns[s];
			else return 0;
		}
		
		set
		{
			if(Cooldowns.ContainsKey(s)) Cooldowns[s] = value;
			else Cooldowns.Add(s, value);
		}
	}
	
	public bool InCooldown(string s) => this[s] > 0;
	
	public void Update()
	{
		var keys = new List<string>(Cooldowns.Keys);
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
