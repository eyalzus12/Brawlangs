using Godot;
using System;

public class BaseScene : Node2D
{
	public override void _Process(float delta)
	{
		if(Input.IsActionJustPressed("exit_game")) GetTree().Quit();
	}
}
