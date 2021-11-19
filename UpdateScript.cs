using Godot;
using System;
using System.Linq;

public class UpdateScript : Node
{
	public bool debugCollision = false;
	
	public override void _Process(float delta)
	{
		if(Input.IsActionJustPressed("toggle_fullscreen"))
			OS.WindowFullscreen = !OS.WindowFullscreen;
		if(Input.IsActionJustPressed("debug_collision"))
			debugCollision = !debugCollision;
	}
}
