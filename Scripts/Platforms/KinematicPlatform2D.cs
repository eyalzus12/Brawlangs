using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class KinematicPlatform2D : KinematicBody2D
{
	public KinematicPlatform2D() {}
	
	//public Dictionary<string, Type> LoadExtraProperties = new Dictionary<string, Type>();
	
	public override void _Ready()
	{
		//foreach(var c in getshape())
		//	c.OneWayCollision = OneWay;
		getshape().ForEach(c=>c.OneWayCollision=OneWay);
		
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
	public float PlatformAngularVelocity = 0f;
	[Export]
	public bool Clingable = true;
	[Export]
	public bool OneWay = false;
	[Export]
	public bool FallThrough = false;
	
	public bool OneWayPlatform
	{
		get => OneWay;
		set
		{
			OneWay = value;
			getshape().ForEach(c=>c.OneWayCollision=value);
			//foreach(var c in getshape())
			//	c.OneWayCollision = value;
		}
	}
	
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
	
	//public void LoadExtraProperty<T>(string s) => LoadExtraProperties.Add(s, typeof(T));
	
	private List<CollisionShape2D> getshape() => GetChildren().Enumerable().FilterType<CollisionShape2D>().ToList();
}
