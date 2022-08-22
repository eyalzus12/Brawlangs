using Godot;
using System;

public class TimedAction
{
	public string Name{get; private set;}
	public int FramesLeft{get; private set;}
	public Action DesignatedAction{get; private set;}
	
	public TimedAction(string name, int framesLeft, Action action)
	{
		Name = name;
		FramesLeft = framesLeft;
		DesignatedAction = action;
	}
	
	public void Update()
	{
		FramesLeft--;
		if(FramesLeft == 0) {Execute(); GD.Print($"Doing your mom {FramesLeft}");}
	}
	
	public void Execute() => DesignatedAction();
}
