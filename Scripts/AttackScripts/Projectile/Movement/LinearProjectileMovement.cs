using Godot;
using System;

public class LinearProjectileMovement : ProjectileMovementFunction
{
	public Vector2 Velocity = default;
	public override Vector2 GetNext(Projectile proj) => proj.Position+proj.direction*Velocity;
	
	public override void LoadProperties()
	{
		LoadExtraProperty<Vector2>("Velocity", Vector2.Zero);
	}
}
