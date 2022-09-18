using Godot;
using System;

public partial class Effect : Node2D
{
	public Character ch;
	public int length;
	public int frameCount = 0;
	
	public Effect()
	{
		frameCount = 0;
	}
	
	public Effect(int length)
	{
		this.length = length;
		frameCount = 0;
	}
	
	public override void _Ready()
	{
		ch = GetParent() as Character;
		Init();
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if(frameCount >= length)
		{
			OnEnd();
			QueueFree();
		}
		else
		{
			Loop();
			++frameCount;
		}
	}
	
	public virtual void Init() {}
	public virtual void Loop() {}
	public virtual void OnEnd() {}
}
