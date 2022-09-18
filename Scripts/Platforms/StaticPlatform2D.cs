using Godot;
using System;
using System.Collections.Generic;

public partial class StaticPlatform2D : StaticBody2D
{
	public StaticPlatform2D() {}
	
	public override void _Ready()
	{
		SetCollisionLayerValue(1, !FallThrough);
		SetCollisionLayerValue(2, FallThrough);
		
		CollisionMask = 0;
	}
	
	[Export]
	public float PlatformFriction = 1f;
	[Export]
	public float PlatformBounce = 0f;
	[Export]
	public Vector2 PlatformLinearVelocity = Vector2.Zero;
	[Export]
	public bool Clingable = true;
	[Export]
	public bool FallThrough = false;
	
	public bool FallThroughPlatform
	{
		get => FallThrough;
		set
		{
			FallThrough = value;
			SetCollisionLayerValue(0, !value);
			SetCollisionLayerValue(1, value);
		}
	}
}
