using Godot;
using System;

public class LinearProjectileMovement : ProjectileMovementFunction
{
	public Vector2 Velocity{get; set;} = Vector2.Zero;
	public override Vector2 GetNext(Projectile proj) => proj.Position+proj.Direction*Velocity;
	
	public override void LoadProperties()
	{
		LoadExtraProperty<Vector2>("Velocity", Vector2.Zero);
	}
}
