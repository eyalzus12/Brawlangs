using Godot;
using System;

public class InputManager : Node
{
	public InputManager(): base() {}
	
	public virtual string InputPrefix => "";
	
	public override void _Ready()
	{
		PauseMode = Node.PauseModeEnum.Process;
	}
	
	public virtual void SimulateInput(InputEvent @event) => Input.ParseInputEvent(@event);
	
	public virtual bool IsActionJustPressed(string str) => Input.IsActionJustPressed(str);
	public virtual bool IsActionPressed(string str) => Input.IsActionPressed(str);
	public virtual bool IsActionJustReleased(string str) => Input.IsActionJustReleased(str);
	public virtual bool IsActionReallyJustPressed(string str) => Input.IsActionJustPressed(str);
	public virtual bool IsActionReallyPressed(string str) => Input.IsActionPressed(str);
	public virtual bool IsActionReallyJustReleased(string str) => Input.IsActionJustReleased(str);
	
	public virtual void MarkForDeletion(string action, bool now=false) {}
	public virtual void MarkAllForDeletion(string prefix = "") {}
	
	public virtual string ToString(string prefix) => ToString();
}
