using Godot;
using System;

public class BufferInputManagerWrapper : InputManager
{
	public int Device{get; set;}
	public int Profile{get; set;}
	private string InputPrefix => $"D{Device}_P{Profile}_";
	public BufferInputManager BufferInput{get; set;}
	
	public BufferInputManagerWrapper() {}
	public BufferInputManagerWrapper(int device, int profile)
	{
		Device = device;
		Profile = profile;
	}
	
	public override void _Ready()
	{
		BufferInput = this.GetRootNode<BufferInputManager>("BufferInputManager");
	}
	
	public override bool IsActionJustPressed(string str) => BufferInput.IsActionJustPressed(InputPrefix + str);
	public override bool IsActionPressed(string str) => BufferInput.IsActionPressed(InputPrefix + str);
	public override bool IsActionJustReleased(string str) => BufferInput.IsActionJustReleased(InputPrefix + str);
	public override bool IsActionReallyJustPressed(string str) => BufferInput.IsActionJustPressed(InputPrefix + str);
	public override bool IsActionReallyPressed(string str) => BufferInput.IsActionPressed(InputPrefix + str);
	public override bool IsActionReallyJustReleased(string str) => BufferInput.IsActionJustReleased(InputPrefix + str);
	
	public override void MarkForDeletion(string action, bool now=false) => BufferInput.MarkForDeletion(InputPrefix + action, now);
	public override void MarkAllForDeletion(string prefix = "") => BufferInput.MarkAllForDeletion(InputPrefix + prefix);
	
	public override string ToString() => ToString("");
	public override string ToString(string prefix) => BufferInput.ToString(InputPrefix + prefix);
}
