using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Hitbox : Area2D
{
	[Signal]
	public delegate void HitboxHit(Hitbox hitbox, Hurtbox hurtbox);
	
	public Vector2 SetKnockback{get; set;}
	public Vector2 VarKnockback{get; set;}
	public float SetStun{get; set;}
	public float VarStun{get; set;}
	public float SetHitPause{get; set;}
	public float VarHitPause{get; set;}
	public float HitLag{get; set;}
	public float Damage{get; set;}
	public int HitPriority{get; set;}
	public float TeamKnockbackMult{get; set;}
	public float TeamDamageMult{get; set;}
	public float TeamStunMult{get; set;}
	public Vector2 MomentumCarry{get; set;}
	
	public AudioStream HitSound{get; set;}
	
	public int frameCount = 0;
	
	public AngleFlipper HorizontalAngleFlipper{get; set;}
	public AngleFlipper VerticalAngleFlipper{get; set;}
	
	public HitCondition TeamHitCondition{get; set;}
	public HitCondition SelfHitCondition{get; set;}
	
	public Dictionary<string, ParamRequest> LoadExtraProperties = new Dictionary<string, ParamRequest>();
	
	public List<Vector2> ActiveFrames{get; set;}
	public IHitter OwnerObject{get; set;}
	
	public HashSet<string> WhitelistedStates{get; set;} = new HashSet<string>();
	public HashSet<string> BlacklistedStates{get; set;} = new HashSet<string>();
	
	public Vector2 OriginalPosition{get; set;} = Vector2.Zero;
	public float OriginalRotation{get; set;} = 0f;
	
	protected bool _active = false;
	public bool Active
	{
		get => _active;
		set
		{
			SetPhysicsProcess(value);
			if(value) UpdateHitboxPosition();
			HitboxShape?.SetDeferred("disabled", !value);
			Visible = value;
			_active = value;
			Monitoring = value;
		}
	}
	
	public CollisionShape2D HitboxShape{get; set;}
	
	public Vector2 CollisionPosition
	{
		get => HitboxShape.Position;
		set
		{
			var val = value*new Vector2(OwnerObject.Direction, 1);
			HitboxShape?.SetDeferred("position", val);
			OriginalPosition = value;
		}
	}
	
	public float CollisionRotation
	{
		get => HitboxShape.Rotation;
		set
		{
			var val = value*OwnerObject.Direction;
			HitboxShape?.SetDeferred("rotation", val);
			OriginalRotation = value;
		}
	}
	
	public override void _Ready()
	{
		Active = false;
		
		frameCount = 0;
		
		Init();
		
		Connect("area_entered", this, nameof(OnAreaEnter));
		
		//TODO: figure how this shit works
		CollisionLayer ^= 0b11111;//reset five rightmost bits
		CollisionLayer |= 0b100000;//rightmost bits set to 10000
		CollisionMask ^= 0b11111;///reset five rightmost bits
		CollisionMask |= 0b01000;//rightmost bits set to 01000
		
		UpdateHitboxPosition();
	}
	
	public virtual void UpdateHitboxPosition()
	{
		CollisionPosition = OriginalPosition;
		CollisionRotation = OriginalRotation;
		Update();
	}
	
	public virtual void Init() {}
	
	public virtual void LoadProperties() {}
	
	public void LoadExtraProperty<T>(string s, T @default = default(T))
	{
		var toAdd = new ParamRequest(typeof(T), s, @default);
		LoadExtraProperties.Add(s, toAdd);
	}
	
	public virtual bool CanHit(IHittable hit)
	{
		if(OwnerObject == hit || OwnerObject.OwnerObject == hit)
		{
			if(OwnerObject.FriendlyFire) return SelfHitCondition != HitCondition.ForceNo;
			else return SelfHitCondition == HitCondition.Force;
		}
		else if(OwnerObject.TeamNumber == hit.TeamNumber)
		{
			if(OwnerObject.FriendlyFire) return TeamHitCondition != HitCondition.ForceNo;
			else return TeamHitCondition == HitCondition.Force;
		}
		else
		{
			if(hit is Character c)
			{
				for(var t = c.States.Current.GetType(); t != typeof(State); t = t.BaseType)
				{
					var Whitelisted = (WhitelistedStates.Count == 0 || WhitelistedStates.Contains(t.Name));
					var Blacklisted = BlacklistedStates.Contains(t.Name);
					if(!Whitelisted || Blacklisted) return false;
				}
			}
			return true;
		}
	}
	
	public void OnAreaEnter(Area2D area)
	{
		if(area is Hurtbox hurtbox && (CanHit(hurtbox.OwnerObject) || hurtbox.CanBeHitBy(OwnerObject)) && OwnerObject.CanHit(hurtbox.OwnerObject))
		{
			#if DEBUG_ATTACKS
			GD.Print($"{OwnerObject.OwnerObject} hitbox detects hurtbox");
			GD.Print($"{OwnerObject.OwnerObject} calls hurtbox Handle Hit");
			#endif
			
			hurtbox.HandleHit(this);
			
			#if DEBUG_ATTACKS
			GD.Print($"{OwnerObject.OwnerObject} hitbox calls self On Hit");
			#endif
			
			OnHit(hurtbox);
			
			#if DEBUG_ATTACKS
			GD.Print($"{OwnerObject.OwnerObject} hitbox emits signal Hitbox Hit");
			#endif
			
			EmitSignal(nameof(HitboxHit), this, hurtbox);
		}
	}
	
	public virtual void OnHit(Hurtbox hurt) {}
	
	public override void _PhysicsProcess(float delta)
	{
		Loop();
		UpdateHitboxPosition();
		++frameCount;
	}
	
	public override void _Draw()
	{
		//if(!(UpdateScript)n.GetRootNode("UpdateScript").debugCollision) return;
		ZIndex = 4;
		GeometryUtils.DrawCapsuleShape(this,
			HitboxShape.Shape as CapsuleShape2D, //shape
			CollisionPosition, //position
			CollisionRotation, //rotation
			GetDrawColor()); //color
	}
	
	public virtual void Loop() {}
	
	public float TeamMult(IHittable n, float chooseFrom) => (n == OwnerObject || n.TeamNumber == OwnerObject.TeamNumber)?chooseFrom:1f;
	
	//IStateHandled
	private float StateMult(IHittable n, Dictionary<string, float> chooseFrom)
	{
		if(chooseFrom is null || !(n is Character c)) return 1f;//only works on characters obv
		
		var f = 1f;
		
		for(var t = c.States.Current.GetType(); t.Name != "State"; t = t.BaseType)
		{
			if(chooseFrom.TryGetValue(t.Name.Replace("State", ""), out f)) return f;
		}
		
		return 1f;
	}
	
	public virtual float GetKnockbackMultiplier(IHittable n) => TeamMult(n, TeamKnockbackMult);
	public virtual float GetDamageMultiplier(IHittable n) => TeamMult(n, TeamDamageMult);
	public virtual float GetStunMultiplier(IHittable n) => TeamMult(n, TeamStunMult);
	
	public virtual Vector2 KnockbackDir(IHittable hitChar) => new Vector2(
		KnockbackDirX(hitChar),
		KnockbackDirY(hitChar)
	);
	
	public virtual float KnockbackDirX(IHittable hitChar)
	{
		switch(HorizontalAngleFlipper)
		{
			case AngleFlipper.Away:
			case AngleFlipper.AwayCharacter:
				return Math.Sign((hitChar.Position-OwnerObject.OwnerObject.Position).x);
			case AngleFlipper.AwayHitbox:
				return Math.Sign((hitChar.Position-Position).x);
			case AngleFlipper.None:
				return 1;
			case AngleFlipper.Direction:
			default:
				return OwnerObject.Direction;
		}
	}
	
	public virtual float KnockbackDirY(IHittable hitChar)
	{
		switch(VerticalAngleFlipper)
		{
			case AngleFlipper.Away:
			case AngleFlipper.AwayCharacter:
				return Math.Sign((hitChar.Position-OwnerObject.OwnerObject.Position).y);
			case AngleFlipper.AwayHitbox:
				return Math.Sign((hitChar.Position-Position).y);
			case AngleFlipper.None:
			case AngleFlipper.Direction:
			default:
				return 1;
		}
	}
	
	public virtual Color GetDrawColor()
	{
		if((SetStun == 0) && (VarStun == 0) && (SetHitPause == 0) && (VarHitPause == 0) && (HitLag == 0)) return new Color(0.9f,0.9f,0.9f,1);
		switch(HorizontalAngleFlipper)
		{
			case AngleFlipper.AwayHitbox:
			case AngleFlipper.AwayCharacter:
			case AngleFlipper.Away:
				return new Color(1, 0.3f, 0.1f, 1);
			case AngleFlipper.None:
				switch(VerticalAngleFlipper)
				{
					case AngleFlipper.AwayHitbox:
					case AngleFlipper.AwayCharacter:
					case AngleFlipper.Away:
						return new Color(0.7f, 0.7f, 0.1f, 1);
					case AngleFlipper.None:
					case AngleFlipper.Direction:
					default:
						return new Color(0.5f, 0.5f, 0, 1);
				}
				
			case AngleFlipper.Direction:
			default:
				return new Color(1, 0.1f, 0.1f, 1);
		}
	}
	
	public enum AngleFlipper{Direction, Away, AwayHitbox, AwayCharacter, None}
}
