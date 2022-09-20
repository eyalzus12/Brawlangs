using Godot;
using System;
using System.Collections.Generic;

public class StateMachine
{
	public Dictionary<string, State> States{get; set;} = new();
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
		var tempState = this[s];
		
		Current?.OnChange(tempState);
		Current?.EmitSignal("StateEnd", Current);
		
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
		else throw new Exception($"When getting state of type {typeof(T).Name} StateMachine found state of type {state.GetType().Name}");
	}
	
	public T Change<T>() where T : State
	{
		var newState = Change(Get<T>().ToString());
		if(newState is T t) return t;
		throw new Exception($"The type safe Get function for states was called with {typeof(T).Name} and properly returned but attempt to change to that state found a wrong state type {newState.GetType().Name}. If this shows, something got really fucked up");
	}
	
	public void Update(double delta)
	{
		Current?.SetInputs();
		Current?.DoPhysics(delta);
	}
}
