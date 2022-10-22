using Godot;
using System;
using System.Collections.Generic;

public class StateMachine
{
	public Dictionary<string, State> States{get; set;} = new Dictionary<string, State>();
	public State Current{get; private set;}
	public State Prev{get; private set;}
	
	public StateMachine(IEnumerable<State> states)
	{
		foreach(var state in states) States.Add(state.ToString(), state);
	}
	
	public State this[string s] => States[s];
	public bool Contains(string s) => States.ContainsKey(s);
	
	public State Get(string s) => this[s];
	
	public State Change(string s)
	{
		if(!Contains(s))
		{
			GD.PushError($"Attempt to change to unknown state {s}");
			return null;
		}
		
		var tempState = this[s];
		
		Current?.OnChange(tempState);
		
		if(Current == tempState)
		{
			Current.ForcedInit();
			return Current;
		}
		
		Prev = Current;
		Current = tempState;
		Current.ForcedInit();
		
		return Current;
	}
	
	public T Get<T>() where T : State
	{
		var name = typeof(T).Name.Replace("State", "");
		var state = Get(name);
		if(state is T t) return t;
		else
		{
			GD.PushError($"When getting state of type {typeof(T).Name} StateMachine found state of type {state.GetType().Name}");
			return null;
		}
	}
	
	public T Change<T>() where T : State
	{
		State s = Change(Get<T>().ToString());
		if(s is T t) return t;
		GD.PushError("The StateMachine state change function got the name of a state of type {typeof(T).Name} but returned a state of type {s.GetType().Name}");
		return null;
	}
	
	public void Update(float delta)
	{
		Current?.SetInputs();
		Current?.DoPhysics(delta);
	}
}
