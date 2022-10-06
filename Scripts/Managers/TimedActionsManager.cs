using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

public class TimedActionsManager
{
	public Dictionary<string, List<TimedAction>> Actions{get; set;} = new Dictionary<string, List<TimedAction>>();
	
	public void Add(string s, int time, Action action)
	{
		Actions.TryAdd(s, new List<TimedAction>());
		Actions[s].Add(new TimedAction(s,time,action));
	}
	
	public void Remove(string s) => Actions.Remove(s);
	
	public void Update()
	{
		Actions.Values.ForEach(l => l.ForEach(a => a.Update()));
		var actionsCopy = new Dictionary<string, List<TimedAction>>(Actions);
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
