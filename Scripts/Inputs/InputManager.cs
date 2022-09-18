using Godot;
using System;

public partial class InputManager : Node
{
	public InputManager(): base() {}
	
	public override void _Ready()
	{
		ProcessMode = Node.ProcessModeEnum.Always;
	}
	
	public virtual bool IsActionJustPressed(string str) => Input.IsActionJustPressed(str);
	public virtual bool IsActionPressed(string str) => Input.IsActionPressed(str);
	public virtual bool IsActionJustReleased(string str) => Input.IsActionJustReleased(str);
	public virtual bool IsActionReallyJustPressed(string str) => Input.IsActionJustPressed(str);
	public virtual bool IsActionReallyPressed(string str) => Input.IsActionPressed(str);
	public virtual bool IsActionReallyJustReleased(string str) => Input.IsActionJustReleased(str);
	
	public virtual void MarkForDeletion(string action, bool now=false) {}
	public virtual void MarkAllForDeletion() {}
}
