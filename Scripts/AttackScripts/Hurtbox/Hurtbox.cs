using Godot;
using System;
using System.Collections.Generic;

public class Hurtbox : Area2D
{
	public CollisionShape2D col;
	
	public CapsuleShape2D Shape{get => (CapsuleShape2D)col.Shape; set => col.Shape = value;}
	
	public float Radius {get => Shape.Radius; set => Shape.SetDeferred("radius", value);}
	public float Height {get => Shape.Height; set => Shape.SetDeferred("height", value);}
	
	public Dictionary<string, HurtboxCollisionState> states = new Dictionary<string, HurtboxCollisionState>();
	public string CurrentCollisionState{get; set;} = "Default";
	public HurtboxCollisionState CurrentCollisionStateData;
	
	public HitCondition TeamHitCondition{get; set;}
	public HitCondition SelfHitCondition{get; set;}
	
	public int TeamNumber{get => OwnerObject.TeamNumber; set => OwnerObject.TeamNumber = value;}
	
	public Vector2 CollisionPosition
	{
		get => col.Position;
		set
		{
			var val = value*new Vector2(OwnerObject.Direction, 1);
			col?.SetDeferred("position", val);
			originalPosition = value;
		}
	}
	
	public float CollisionRotation
	{
		get => col.Rotation;
		set
		{
			var val = value*OwnerObject.Direction;
			col?.SetDeferred("rotation", val);
			originalRotation = value;
		}
	}
	
	public Hurtbox()
	{
		col = new CollisionShape2D();
		col.Name = $"HurtboxCollision";
		AddChild(col);
		var shape = new CapsuleShape2D();
		col.Shape = shape;
	}
	
	public Vector2 originalPosition = Vector2.Zero;
	public float originalRotation = 0f;
	
	public IHittable OwnerObject{get; set;}
	
	public override void _Ready()
	{
		ZIndex = 2;
		CollisionLayer ^= 0b11111;//reset five rightmost bits
		CollisionLayer |= 0b01000;//rightmost bits set to 01000
		CollisionMask ^= 0b11111;//reset five rightmost bits
		//CollisionMask |= 0b00000;//rightmost bits set to 00000. basically does nothing
		
		originalPosition = CollisionPosition;
		originalRotation = CollisionRotation;
		
		ChangeState("Default");
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
			GD.PushError($"Collision State {newState} is not defined for hurtbox {this}");
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
		if(!this.GetRootNode<UpdateScript>("UpdateScript").debugCollision) return;
		this.DrawCapsuleShape(Shape, CollisionPosition, CollisionRotation, GetDrawColor());
	}
	
	public virtual void HandleHit(Hitbox hit)
	{
		
	}
	
	public virtual Color GetDrawColor() => new Color(0,1,0,1);
}
