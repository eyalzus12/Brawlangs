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
		if(Change(Get<T>().ToString()) is T t) return t;
		throw new Exception("This should never fucking happen. If anyone finds this, fuck you.");
	}
	
	public void Update(float delta)
	{
		Current?.SetInputs();
		Current?.DoPhysics(delta);
	}
	
	public void Clear()
	{
		States.Values.ForEach(s=>s.QueueFree());
		States.Clear();
	}
}
