using Godot;
using System;

public class DummyInputManager : InputManager
{
	public DummyInputManager(): base() {}
	
	//TODO: use =>
	
	public override bool IsActionJustPressed(String str)
	{
		return false;
	}
	
	public override bool IsActionPressed(String str)
	{
		return false;
	}
	
	public override bool IsActionJustReleased(String str)
	{
		return false;
	}
	
	public override bool IsActionReallyJustPressed(String str)
	{
		return false;
	}
	
	public override bool IsActionReallyPressed(String str)
	{
		return false;
	}
	
	public override bool IsActionReallyJustReleased(String str)
	{
		return false;
	}
}
