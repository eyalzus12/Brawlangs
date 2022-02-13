using Godot;
using System;

public class PlatformDropDetector : Area2D
{
	public CollisionShape2D collision;
	public Shape2D Shape{get => collision.Shape; set => collision.Shape = value;}
	public RectangleShape2D RShape{get => (RectangleShape2D)Shape; set => Shape = value;}
	public Vector2 Extents{get => RShape.Extents; set => RShape.Extents = value;}
	public Character owner;
	public Vector2 originalPosition = Vector2.Zero;
	
	public Vector2 CollisionPosition
	{
		get => collision.Position;
		set
		{
			var val = value*new Vector2(direction, 1);
			collision?.SetDeferred("position", val);
			originalPosition = value;
		}
	}
	
	public override void _Ready()
	{
		collision = new CollisionShape2D();
		collision.Shape = new RectangleShape2D();
		collision.Name = "DropCollision";
		AddChild(collision);
		originalPosition = CollisionPosition;
		Connect("body_exited", owner, "OnSemiSolidLeave");
		CollisionLayer = 0;
		CollisionMask = 0b10;
	}
	
	
	
	public void UpdateBasedOnCollisionShape() => UpdateBasedOnCollisionShape(owner.collision);
	private const float Y_EXTENTS = 1f;
	public virtual void UpdateBasedOnCollisionShape(CollisionShape2D col)
	{
		var colShape = (RectangleShape2D)col.Shape;
		var colPosition = col.Position;
		var colExtents = colShape.Extents;
		
		var desiredPosition = colPosition;
		var desiredExtents = colExtents;
		//plat drop is now identical to the collision shape
		
		//Update:
		//Step 1: set y extents to low value
		desiredExtents.y = Y_EXTENTS;
		//Step 2: move to the bottom of the collision shape
		desiredPosition.y += colExtents.y;
		
		//plat drop is now at the bottom of the collision shape, is identical in width, and has a very low height
		
		//apply
		Extents = desiredExtents;
		CollisionPosition = desiredPosition;
	}
	
	public virtual int GetDirection() => owner.direction;
	public int direction => GetDirection();
}
