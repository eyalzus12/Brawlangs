using Godot;
using System;

public class SelfDestructingPlatform2D : StaticBody2D
{
	[Export]
	public int LifeTime = 60;
	
	private int frameCount = 0;
	
	public override void _Ready()
	{
		frameCount = 0;
	}
	
	public override void _PhysicsProcess(float delta)
	{
		if(frameCount >= LifeTime) QueueFree();
		else ++frameCount;
	}
}
