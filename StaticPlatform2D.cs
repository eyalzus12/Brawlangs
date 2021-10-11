using Godot;
using System;
using System.Collections.Generic;

public class StaticPlatform2D : StaticBody2D
{
	public StaticPlatform2D() {}
	
	public override void _Ready()
	{
		SetCollisionLayerBit(0, !FallThrough);
		SetCollisionLayerBit(1, FallThrough);
		
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
			SetCollisionLayerBit(0, !value);
			SetCollisionLayerBit(1, value);
		}
	}
	
	/*private List<CollisionShape2D> getshape()
	{
		List<CollisionShape2D> l = new List<CollisionShape2D>();
		
		foreach(Node n in GetChildren())
			if(n is CollisionShape2D c)
				l.Add(c);
		return l;
	}*/
}
