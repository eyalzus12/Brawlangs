using Godot;
using System;
using System.Collections.Generic;
using System.Text;

public class ResourceManager
{
	public Dictionary<string, int> Resources{get; set;} = new Dictionary<string, int>();
	
	public int this[string resource]
	{
		get => Resources.ContainsKey(resource)?Resources[resource]:0;
		set => Resources[resource] = value;
	}
	
	public void Give(string resource, int amount, int min, int max)
	{
		if(Resources.ContainsKey(resource))
		{
			int desired = Math.Max(Math.Min(Resources[resource] + amount, max), Math.Max(min,0));
			if(desired == 0) Resources.Remove(resource);
			else Resources[resource] = desired;
		}
		else if(amount > 0) Resources.Add(resource, amount);
	}
	
	public void Remove(string resource)
	{
		if(Resources.ContainsKey(resource)) Resources.Remove(resource);
	}
	
	public void Give(string resource, int amount, int max) => Give(resource, amount, 0, max);
	public void Give(string resource, int amount) => Give(resource, amount, int.MaxValue);
	
	public int Count(string resource) => Resources.ContainsKey(resource)?Resources[resource]:0;
	
	public bool Has(string resource) => HasBeyondThreshold(resource, 0);
	public bool HasBeyondThreshold(string resource, int threshold) => this[resource] > threshold;
	
	public override string ToString()
	{
		var result = new StringBuilder();
		foreach(var entry in Resources) result.Append($"{entry.Key.ToString()} : {entry.Value.ToString()}\n");
		return result.ToString();
	}
}
