using Godot;
using System;

public class ProjectileMaker : AttackPart
{
	public override void Init()
	{
		LoadExtraProperty<Vector2>("InitialProjectileMovement", Vector2.Zero);
	}
	
	public override void PostHitboxInit()
	{
		
	}
}
