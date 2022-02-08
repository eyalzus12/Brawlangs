using Godot;
using System;
using System.Collections.Generic;

public class Hurtbox : Area2D
{
	public CollisionShape2D col;
	
	public CapsuleShape2D Shape{get => (CapsuleShape2D)col.Shape; set => col.Shape = value;}
	
	public float Radius {get => Shape.Radius; set => Shape.Radius = value;}
	public float Height {get => Shape.Height; set => Shape.Height = value;}
	
	public Dictionary<string, HurtboxCollisionState> states = new Dictionary<string, HurtboxCollisionState>();
	public string CurrentCollisionState = "Default";
	public HurtboxCollisionState CurrentCollisionStateData;
	
	public Vector2 CollisionPosition
	{
		get => col.Position;
		set
		{
			var val = value*new Vector2(direction, 1);
			col?.SetDeferred("position", val);
			originalPosition = value;
		}
	}
	
	public float CollisionRotation
	{
		get => col.Rotation;
		set
		{
			var val = value*direction;
			col?.SetDeferred("rotation", val);
			originalRotation = value;
		}
	}
	
	public Vector2 originalPosition = Vector2.Zero;
	public float originalRotation = 0f;
	
	public IHittable owner;
	
	public override void _Ready()
	{
		var coli = new CollisionShape2D();
		AddChild(coli);
		col = coli;
		var shape = new CapsuleShape2D();
		col.Shape = shape;
		CollisionLayer ^= 0b11111;//reset five rightmost bits
		CollisionLayer |= 0b01000;//rightmost bits set to 01000
		CollisionMask ^= 0b11111;//reset five rightmost bits
		//CollisionMask |= 0b00000;//rightmost bits set to 00000. basically does nothing
		
		originalPosition = CollisionPosition;
		originalRotation = CollisionRotation;
	}
	
	public override void _PhysicsProcess(float delta)
	{
		CollisionPosition = originalPosition;
		CollisionRotation = originalRotation;
		Update();
	}
	
	public virtual void AddState(HurtboxCollisionState newState)
	{
		states.Add(newState.Name, newState);
	}
	
	public virtual void ChangeState(string newState)
	{
		try
		{
			var changeTo = states[newState];
			ChangeState(changeTo);
		}
		catch(KeyNotFoundException)
		{
			GD.Print($"Collision State {newState} is not defined for hurtbox {this}");
		}
	}
	
	public virtual void ChangeState(HurtboxCollisionState newState)
	{
		CurrentCollisionState = newState.Name;
		CurrentCollisionStateData = newState;
		Radius = newState.Radius;
		Height = newState.Height;
		CollisionPosition = newState.Position;
		CollisionRotation = newState.Rotation;
	}
	
	public override void _Draw()
	{
		if(!this.GetRootNode<UpdateScript>("UpdateScript").debugCollision) return;
		ZIndex = 2;
		GeometryUtils.DrawCapsuleShape(this, Shape, CollisionPosition, CollisionRotation, GetDrawColor());
	}
	
	public virtual Color GetDrawColor() => new Color(0,1,0,1);
	
	public virtual int GetDirection() => (((Node2D)owner)?.Get("direction")??1).i();
	public int direction => GetDirection();
}
