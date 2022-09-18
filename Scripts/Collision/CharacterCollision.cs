using Godot;
using System;
using System.Collections.Generic;

public partial class CharacterCollision : CollisionShape2D
{
	public RectangleShape2D RecShape{get => (RectangleShape2D)Shape; set => Shape = value;}
	
	public Vector2 Extents {get => RecShape.Size; set => RecShape.SetDeferred("extents", value);}
	
	public Dictionary<string, CollisionShapeState> states = new();
	public string CurrentCollisionState = "Default";
	public CollisionShapeState CurrentCollisionStateData;
	
	public Vector2 originalPosition = Vector2.Zero;
	
	public int Direction => owner.Direction;
	public Character owner;
	
	public Vector2 DynamicPosition
	{
		get => Position;
		set
		{
			var val = value*new Vector2(Direction, 1);
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
	
	public override void _PhysicsProcess(double delta)
	{
		DynamicPosition = originalPosition;
		QueueRedraw();
	}
	
	public virtual void AddState(CollisionShapeState newState)
	{
		states.Add(newState.Name, newState);
	}
	
	public virtual void ChangeState(string newState)
	{
		if(states.ContainsKey(newState))
		{
			var changeTo = states[newState];
			ChangeState(changeTo);
		}
		else
		{
			GD.PushError($"Collision State {newState} is not defined for collision {this}");
		}
	}
	
	public virtual void ChangeState(CollisionShapeState newState)
	{
		CurrentCollisionState = newState.Name;
		CurrentCollisionStateData = newState;
		//Extents = newState.Extents;
		RecShape.SetDeferred("extents", newState.Extents);
		DynamicPosition = newState.Position;
		QueueRedraw();
	}
	
	public override void _Draw()
	{
		if(!this.GetRootNode<UpdateScript>("UpdateScript").debugCollision) return;
		var rect = GeometryUtils.RectFrom(Vector2.Zero, Extents);
		DrawRect(rect, GetDrawColor(), true);
		
		DrawCircle(-DynamicPosition, 5, new Color(0,0,0,1));
	}
	
	public virtual Color GetDrawColor() => new Color(1,1,0,1);
}
