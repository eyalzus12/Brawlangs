using Godot;
using System;

public partial class PauseHandeler : Node
{
	public bool queueNextFrame = false;
	
	public override void _Ready()
	{
		ProcessMode = Node.ProcessModeEnum.Always;
	}
	
	public override void _Process(double delta)
	{
		if(queueNextFrame)
		{
			GetTree().Paused = true;
			queueNextFrame = false;
		}
		if(Input.IsActionJustPressed("pause"))
			GetTree().Paused = !GetTree().Paused;
		if(GetTree().Paused && Input.IsActionJustPressed("next_frame"))
		{
			queueNextFrame = true;
			GetTree().Paused = false;
		}
	}
}
