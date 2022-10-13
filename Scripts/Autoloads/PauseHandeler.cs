using Godot;
using System;

public class PauseHandeler : Node
{
	public bool queueNextFrame = false;
	
	public override void _Ready()
	{
		PauseMode = Node.PauseModeEnum.Process;
	}
	
	public override void _Process(float delta)
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
