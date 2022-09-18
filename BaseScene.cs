using Godot;
using System;

public partial class BaseScene : Node2D
{
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("exit_game")) GetTree().Quit();
	}
}
