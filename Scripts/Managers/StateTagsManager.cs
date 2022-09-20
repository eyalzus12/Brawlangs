using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class StateTagsManager
{
	public Dictionary<string, StateTag> Tags{get; set;} = new();
	
	public StateTag this[string s]
	{
		get => Tags.GetValueOrDefault(s,StateTag.NotDefined);
		set => Tags[s] = value;
	}
	
	public void Update()
	{
		Tags.Keys.ToList().ForEach(s => this[s] = NextTag(this[s]));
	}
	
	private StateTag NextTag(StateTag t) => t switch
	{
		StateTag.Starting => StateTag.Active,
		StateTag.Ending or StateTag.Instant => StateTag.NotActive,
		_ => t,//NotActive, Active, or NotDefined
	};
	
	public override string ToString()
	{
		var result = new StringBuilder();
		foreach(var entry in Tags) if(entry.Value != StateTag.NotActive) result.Append($"{entry.Key.ToString()} : {entry.Value.ToString()}\n");
		return result.ToString();
	}
}
