using Godot;
using System;

public class Hurtbox : Area2D
{
	public CollisionShape2D col;
	
	public CapsuleShape2D Shape => col.Shape as CapsuleShape2D;
	public float Radius {get => Shape.Radius; set => Shape.Radius = value;}
	public float Height {get => Shape.Height; set => Shape.Height = value;}
	
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
	}
}
