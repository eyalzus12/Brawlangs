using Godot;
using System;
using Godot.Collections;

public class MovementPath2D : PathFollow2D
{
	[Export]
	public Array<Vector3> actions = new Array<Vector3>();
	//x = speed
	//y = offset to go
	//z = startup in frames
	
	public MovementPath2D() {}
	
	public override void _Ready()
	{
		AddChild(new MovementTween(actions));
	}
}
