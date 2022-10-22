using Godot;
using System;

public interface IKnockable : IHittable
{
	Vector2 Knockback{get; set;}
	float KnockbackTakenMult{get; set;}
}
