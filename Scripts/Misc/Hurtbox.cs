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
			var val = value*new Vector2(ch.direction, 1);
			col?.SetDeferred("position", val);
			originalPosition = value;
		}
	}
	
	public float CollisionRotation
	{
		get => col.Rotation;
		set
		{
			var val = value*ch.direction;
			col?.SetDeferred("rotation", val);
			originalRotation = value;
		}
	}
	
	public Vector2 originalPosition = Vector2.Zero;
	public float originalRotation = 0f;
	
	public Character ch;
	
	public void CreateCollision(/*float rad, float hei*/)
	{
		var coli = new CollisionShape2D();
		AddChild(coli);
		col = coli;
		var shape = new CapsuleShape2D();
		/*shape.Radius = rad;
		shape.Height = hei;*/
		col.Shape = shape;
		CollisionLayer = 0b1000;
		CollisionMask = 0b0;
		
		originalPosition = col?.Position ?? default(Vector2);
		originalRotation = col?.Rotation ?? 0;
		
		ch = GetParent() as Character;
	}
	
	public override void _PhysicsProcess(float delta)
	{
		col?.SetDeferred("position", originalPosition*new Vector2(ch.direction, 1));
		col?.SetDeferred("rotation", originalRotation*ch.direction);
		Update();
	}
	
	public override void _Draw()
	{
		GeometryUtils.DrawCapsuleShape(this,
			Shape, //shape
			originalPosition*new Vector2(ch.direction, 1), //position
			originalRotation*ch.direction, //rotation
			new Color(1,1,1)); //color
		
		/*
		var oval = shape.Shape as CapsuleShape2D;
		var height = oval.Height;
		var radius = oval.Radius;
		var color = new Color(1,1,1);
		var position = originalPosition*new Vector2(ch.direction, 1);
		var rotation = originalRotation*ch.direction;
		DrawSetTransform(position, rotation, new Vector2(1,1));
		var middleRect = BlastZone.CalcRect(Vector2.Zero, new Vector2(radius, height/2));
		DrawRect(middleRect, color);
		DrawCircle(new Vector2(0, height/2), radius, color);
		DrawCircle(new Vector2(0, -height/2), radius, color);
		*/
	}
}
