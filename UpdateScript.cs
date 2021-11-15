using Godot;
using System;
using System.Linq;

public class UpdateScript : Node
{
	public override void _Process(float delta)
	{
		if(Input.IsActionJustPressed("toggle_fullscreen"))
			OS.WindowFullscreen = !OS.WindowFullscreen;
	}
}
