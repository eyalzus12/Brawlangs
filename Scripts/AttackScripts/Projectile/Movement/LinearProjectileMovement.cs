using Godot;
using System;

public class LinearProjectileMovement : ProjectileMovementFunction
{
	public Vector2 Direction = default;
	public override Vector2 GetNext(Vector2 Pos) => Pos+Direction;
}
