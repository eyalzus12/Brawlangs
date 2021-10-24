using Godot;
using System;

public class MovementPlayer : AnimationPlayer
{
	public override void _PhysicsProcess(float delta)
	{
		if(Input.IsActionJustPressed("reset"))
		{
			Stop();
			Play();
		}
	}
}
