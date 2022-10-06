using Godot;
using System;
using System.Collections.Generic;

public class Hurtbox : Area2D
{
	public CollisionShape2D HurtboxCollision{get; set;}
	
	public CapsuleShape2D Shape{get => (CapsuleShape2D)HurtboxCollision.Shape; set => HurtboxCollision.Shape = value;}
	
	public float Radius {get => Shape.Radius; set => Shape.SetDeferred("radius", value);}
	public float Height {get => Shape.Height; set => Shape.SetDeferred("height", value);}
	
	public Dictionary<string, HurtboxCollisionState> States{get; set;} = new Dictionary<string, HurtboxCollisionState>();
	public string CurrentCollisionState{get; set;} = "Default";
	public HurtboxCollisionState CurrentCollisionStateData{get; set;}
	
	public Vector2 OriginalPosition{get; set;} = Vector2.Zero;
	public float OriginalRotation{get; set;} = 0f;
	
	public IHittable OwnerObject{get; set;}
	
	public HitCondition TeamHitCondition{get; set;}
	public HitCondition SelfHitCondition{get; set;}
	
	public int TeamNumber{get => OwnerObject.TeamNumber; set => OwnerObject.TeamNumber = value;}
	
	public Vector2 CollisionPosition
	{
		get => HurtboxCollision.Position;
		set
		{
			var val = value*new Vector2(OwnerObject.Direction, 1);
			HurtboxCollision?.SetDeferred("position", val);
			OriginalPosition = value;
		}
	}
	
	public float CollisionRotation
	{
		get => HurtboxCollision.Rotation;
		set
		{
			var val = value*OwnerObject.Direction;
			HurtboxCollision?.SetDeferred("rotation", val);
			OriginalRotation = value;
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
	
	public Hurtbox()
	{
		HurtboxCollision = new CollisionShape2D();
		HurtboxCollision.Name = $"HurtboxCollision";
		AddChild(HurtboxCollision);
		HurtboxCollision.Shape = new CapsuleShape2D();
	}
	
	public override void _Ready()
	{
		ZIndex = 2;
		CollisionLayer ^= 0b11111;//reset five rightmost bits
		CollisionLayer |= 0b01000;//rightmost bits set to 01000
		CollisionMask ^= 0b11111;//reset five rightmost bits
		//CollisionMask |= 0b00000;//rightmost bits set to 00000. basically does nothing
		
		OriginalPosition = CollisionPosition;
		OriginalRotation = CollisionRotation;
		
		ChangeState("Default");
	}
	
	public override void _PhysicsProcess(float delta)
	{
		CollisionPosition = OriginalPosition;
		CollisionRotation = OriginalRotation;
		
		if(Input.IsActionJustPressed("debug_collision"))
			DebugCollision = !DebugCollision;
		
		if(DebugCollision) Update();
	}
	
	public virtual void AddState(HurtboxCollisionState newState)
	{
		States.Add(newState.Name, newState);
	}
	
	public virtual void ChangeState(string newState)
	{
		HurtboxCollisionState changeTo;
		if(States.TryGetValue(newState, out changeTo)) ChangeState(changeTo); 
		else GD.PushError($"Collision State {newState} is not defined for hurtbox {this}");
	}
	
	public virtual void ChangeState(HurtboxCollisionState newState)
	{
		CurrentCollisionState = newState.Name;
		CurrentCollisionStateData = newState;
		Radius = newState.Radius;
		Height = newState.Height;
		CollisionPosition = newState.Position;
		CollisionRotation = newState.Rotation;
		SelfHitCondition = newState.SelfHitCondition;
		TeamHitCondition = newState.TeamHitCondition;
		Update();
	}
	
	public virtual bool CanBeHitBy(IHitter hitter)
	{
		if(hitter == OwnerObject || hitter.OwnerObject == OwnerObject)
		{
			if(OwnerObject.FriendlyFire) return SelfHitCondition != HitCondition.ForceNo;
			else return SelfHitCondition == HitCondition.Force;
		}
		else if(TeamNumber == hitter.TeamNumber)
		{
			if(OwnerObject.FriendlyFire) return TeamHitCondition != HitCondition.ForceNo;
			else return TeamHitCondition == HitCondition.Force;
		}
		else return true;
	}
	
	public virtual bool CanBeHitBy(IAttacker attacker)
	{
		if(attacker == OwnerObject)
		{
			if(OwnerObject.FriendlyFire) return SelfHitCondition != HitCondition.ForceNo;
			else return SelfHitCondition == HitCondition.Force;
		}
		else if(TeamNumber == attacker.TeamNumber)
		{
			if(OwnerObject.FriendlyFire) return TeamHitCondition != HitCondition.ForceNo;
			else return TeamHitCondition == HitCondition.Force;
		}
		else return true;
	}
	
	public override void _Draw()
	{
		if(!DebugCollision) return;
		DrawSetTransform(CollisionPosition, CollisionRotation, Vector2.One);
		this.DrawShape(Shape, DrawColor);
	}
	
	public virtual void HandleHit(Hitbox hit)
	{
		
	}
	
	public static readonly Color Green = new Color(0,1,0,0.5f);
	public virtual Color DrawColor => Green;
}
