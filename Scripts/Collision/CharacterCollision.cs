using Godot;
using System;
using System.Collections.Generic;

public class CharacterCollision : CollisionShape2D
{
	public RectangleShape2D RecShape{get => (RectangleShape2D)Shape; set => Shape = value;}
	
	public Vector2 Extents {get => RecShape.Extents; set => RecShape.Extents = value;}
	
	public Dictionary<string, CollisionShapeState> states = new Dictionary<string, CollisionShapeState>();
	public string CurrentCollisionState = "Default";
	public CollisionShapeState CurrentCollisionStateData;
	
	public Vector2 originalPosition = Vector2.Zero;
	
	public Character owner;
	
	public Vector2 DynamicPosition
	{
		get => Position;
		set
		{
			var val = value*new Vector2(direction, 1);
			SetDeferred("position", val);
			originalPosition = value;
		}
	}
	
	public override void _Ready()
	{
		ZIndex = 3;
		Shape = new RectangleShape2D();
		//CollisionLayer ^= 0b11111;//reset five rightmost bits
		//CollisionLayer |= 0b01000;//rightmost bits set to 01000
		//CollisionMask ^= 0b11111;//reset five rightmost bits
		//CollisionMask |= 0b00000;//rightmost bits set to 00000. basically does nothing
		
		originalPosition = DynamicPosition;
	}
	
	public override void _PhysicsProcess(float delta)
	{
		DynamicPosition = originalPosition;
		Update();
	}
	
	public virtual void AddState(CollisionShapeState newState)
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
			GD.Print($"Collision State {newState} is not defined for collision {this}");
		}
	}
	
	public virtual void ChangeState(CollisionShapeState newState)
	{
		CurrentCollisionState = newState.Name;
		CurrentCollisionStateData = newState;
		Extents = newState.Extents;
		DynamicPosition = newState.Position;
	}
	
	public override void _Draw()
	{
		if(!this.GetRootNode<UpdateScript>("UpdateScript").debugCollision) return;
		var rect = BlastZone.CalcRect(Vector2.Zero, Extents);
		DrawRect(rect, GetDrawColor(), true);
	}
	
	public virtual Color GetDrawColor() => new Color(1,1,0,1);
	
	public virtual int GetDirection() => owner.direction;
	public int direction => GetDirection();
}
