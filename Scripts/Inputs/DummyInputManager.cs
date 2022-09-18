using Godot;
using System;

public partial class DummyInputManager : InputManager
{
	public DummyInputManager(): base() {}
	
	public override bool IsActionJustPressed(string str) => false;
	public override bool IsActionPressed(string str) => false;
	public override bool IsActionJustReleased(string str) => false;
	public override bool IsActionReallyJustPressed(string str) => false;
	public override bool IsActionReallyPressed(string str) => false;
	public override bool IsActionReallyJustReleased(string str) => false;
}
