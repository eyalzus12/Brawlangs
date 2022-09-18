using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

public class TimedActionsManager
{
	public Dictionary<string, List<TimedAction>> Actions{get; set;} = new();
	
	public void Add(string s, int time, Action action)
	{
		if(!Actions.ContainsKey(s)) Actions.Add(s, new());
		Actions[s].Add(new(s,time,action));
	}
	
	public void Remove(string s)
	{
		if(Actions.ContainsKey(s)) Actions.Remove(s);
	}
	
	public void Update()
	{
		Actions.Values.ForEach(l => l.ForEach(a => a.Update()));
		Dictionary<string, List<TimedAction>> actionsCopy = new(Actions);
		actionsCopy.ForEach(t => 
		{
			var after = t.Value.Where(h => h.FramesLeft > 0).ToList();
			if(after.Count == 0) Actions.Remove(t.Key);
			else Actions[t.Key] = after;
		});
	}
	
	public override string ToString()
	{
		var result = new StringBuilder();
		foreach(var entry in Actions)
			foreach(var action in entry.Value)
				result.Append($"{entry.Key} : {action.FramesLeft}\n");
		return result.ToString();
	}
}
