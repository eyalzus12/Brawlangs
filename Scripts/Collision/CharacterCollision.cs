using Godot;
using System;
using System.Collections.Generic;

public class CharacterCollision : CollisionShape2D
{
	public RectangleShape2D RecShape{get => (RectangleShape2D)Shape; set => Shape = value;}
	
	public Vector2 Extents {get => RecShape.Extents; set => RecShape.SetDeferred("extents", value);}
	
	public Dictionary<string, CollisionShapeState> States{get; set;} = new Dictionary<string, CollisionShapeState>();
	public string CurrentCollisionState{get; set;} = "Default";
	public CollisionShapeState CurrentCollisionStateData{get; set;}
	
	public Vector2 OriginalPosition{get; set;} = Vector2.Zero;
	
	public int Direction => OwnerObject.Direction;
	public Character OwnerObject{get; set;}
	
	public Vector2 DynamicPosition
	{
		get => Position;
		set
		{
			var val = value*new Vector2(Direction, 1);
			SetDeferred("position", val);
			OriginalPosition = value;
		}
	}
	
	private bool _debugCollision = false;
	public bool DebugCollision
	{
		get => _debugCollision;
		set
		{
			if(_debugCollision != value)
			{
				_debugCollision = value;
				Update();
			}
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
		
		OriginalPosition = DynamicPosition;
	}
	
	public override void _PhysicsProcess(float delta)
	{
		DynamicPosition = OriginalPosition;
		
		if(Input.IsActionJustPressed("debug_collision"))
			DebugCollision = !DebugCollision;
		
		if(DebugCollision) Update();
	}
	
	public virtual void AddState(CollisionShapeState newState)
	{
		States.Add(newState.Name, newState);
	}
	
	public virtual void ChangeState(string newState)
	{
		CollisionShapeState changeTo;
		if(States.TryGetValue(newState, out changeTo)) ChangeState(changeTo); 
		else GD.PushError($"Collision State {newState} is not defined for collision {this}");
	}
	
	public virtual void ChangeState(CollisionShapeState newState)
	{
		CurrentCollisionState = newState.Name;
		CurrentCollisionStateData = newState;
		Extents = newState.Extents;
		DynamicPosition = newState.Position;
		Update();
	}
	
	public static readonly Color Black = new Color(0,0,0,1);
	public static readonly Color Yellow = new Color(1,1,0,0.5f);
	public override void _Draw()
	{
		if(!DebugCollision) return;
		this.DrawShape(Shape, DrawColor);
		DrawCircle(-DynamicPosition, 5, Black);
	}
	
	public virtual Color DrawColor => Yellow;
}
