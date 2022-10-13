using Godot;
using System;

public class MovingPlatform2D : KinematicPlatform2D
{
	public int frameCount = 0;
	
	public override void _Ready()
	{
		frameCount = 0;
		base._Ready();
		Motion__syncToPhysics = true;
		Init();
	}
	
	public override void _PhysicsProcess(float delta)
	{
		++frameCount;
		Loop();
		SetDeferred("position", GetNextPosition());
	}
	
	public virtual void Init() {}
	public virtual void Loop() {}
	public virtual Vector2 GetNextPosition() => Position;
}
