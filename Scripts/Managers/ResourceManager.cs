using Godot;
using System;
using System.Collections.Generic;
using System.Text;

public class ResourceManager
{
	public Dictionary<string, int> Resources{get; set;} = new();
	
	public int this[string resource]
	{
		get => Resources.GetValueOrDefault(resource,0);
		set => Resources[resource] = value;
	}
	
	public void Give(string resource, int amount, int min, int max)
	{
		int desired = Math.Max(Math.Min(this[resource] + amount, max), Math.Max(min,0));
		if(desired == 0) Remove(resource);
		else this[resource] = desired;
	}
	
	public void Remove(string resource)
	{
		if(Resources.ContainsKey(resource)) Resources.Remove(resource);
	}
	
	public void Give(string resource, int amount, int max) => Give(resource, amount, 0, max);
	public void Give(string resource, int amount) => Give(resource, amount, int.MaxValue);
	
	public bool Has(string resource) => HasBeyondThreshold(resource, 0);
	public bool HasBeyondThreshold(string resource, int threshold) => this[resource] > threshold;
	
	public override string ToString()
	{
		var result = new StringBuilder();
		foreach(var entry in Resources) result.Append($"{entry.Key.ToString()} : {entry.Value.ToString()}\n");
		return result.ToString();
	}
}
