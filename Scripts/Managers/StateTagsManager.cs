using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class StateTagsManager
{
	public Dictionary<string, StateTag> Tags{get; set;} = new Dictionary<string, StateTag>();
	
	public StateTag this[string s]
	{
		get => Tags.GetValueOrDefault(s,StateTag.NotDefined);
		set => Tags[s] = value;
	}
	
	public void Update()
	{
		Tags.Keys.ToList().ForEach(s => this[s] = NextTag(this[s]));
	}
	
	private StateTag NextTag(StateTag t)
	{
		switch(t)
		{
			case StateTag.NotDefined:
			case StateTag.NotActive:
			case StateTag.Active:
				return t;
			case StateTag.Starting:
				return StateTag.Active;
			case StateTag.Ending:
			case StateTag.Instant:
				return StateTag.NotActive;
			default:
				throw new Exception("If you see this someone really fucked up");
		}
	}
	
	public override string ToString()
	{
		var result = new StringBuilder();
		foreach(var entry in Tags) if(entry.Value != StateTag.NotActive) result.Append($"{entry.Key.ToString()} : {entry.Value.ToString()}\n");
		return result.ToString();
	}
}
