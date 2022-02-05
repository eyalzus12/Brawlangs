using Godot;
using System;

public class Hurtbox : Area2D
{
	public CollisionShape2D col;
	
	public CapsuleShape2D Shape => col.Shape as CapsuleShape2D;
	public float Radius {get => Shape.Radius; set => Shape.Radius = value;}
	public float Height {get => Shape.Height; set => Shape.Height = value;}
	
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
		
		originalPosition = col?.Position ?? default(Vector2);
		originalRotation = col?.Rotation ?? 0;
	}
	
	public override void _PhysicsProcess(float delta)
	{
		col?.SetDeferred("position", originalPosition*new Vector2(direction, 1));
		col?.SetDeferred("rotation", originalRotation*direction);
		Update();
	}
	
	public override void _Draw()
	{
		if(!this.GetRootNode<UpdateScript>("UpdateScript").debugCollision) return;
		ZIndex = 2;
		GeometryUtils.DrawCapsuleShape(this,
			Shape, //shape
			originalPosition*new Vector2(direction, 1), //position
			originalRotation*direction, //rotation
			GetDrawColor()); //color
	}
	
	public virtual Color GetDrawColor() => new Color(0,1,0,1);
	
	public int direction => (owner as Character).direction;
}
