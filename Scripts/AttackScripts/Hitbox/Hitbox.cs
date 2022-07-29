using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Hitbox : Area2D
{
	[Signal]
	public delegate void HitboxHit(Hitbox hitbox, Hurtbox hurtbox);
	
	public Vector2 setKnockback = Vector2.Zero;
	public Vector2 varKnockback = Vector2.Zero;
	public float stun = 0;
	public int hitpause = 0;
	public int hitlag = 0;
	public float damage = 0f;
	public int hitPriority = 0;
	public float teamKnockbackMult = 1f;
	public float teamDamageMult = 1f;
	public float teamStunMult = 1;
	public Vector2 momentumCarry = Vector2.Zero;
	
	public AudioStream hitSound;
	
	public int frameCount = 0;
	
	public AngleFlipper horizontalAngleFlipper;
	public AngleFlipper verticalAngleFlipper;
	
	public Dictionary<string, ParamRequest> LoadExtraProperties = new Dictionary<string, ParamRequest>();
	
	public List<Vector2> activeFrames = new List<Vector2>();
	public IHitter OwnerObject{get; set;}
	
	public HashSet<string> whitelistedStates = new HashSet<string>();
	public HashSet<string> blacklistedStates = new HashSet<string>();
	
	private bool active = false;
	
	public Vector2 originalPosition = Vector2.Zero;
	public float originalRotation = 0f;
	
	public bool Active
	{
		get => active;
		set
		{
			active = value;
			if(value) UpdateHitboxPosition();
			Visible = value;
			shape?.SetDeferred("disabled", !active);
		}
	}
	
	public CollisionShape2D shape = new CollisionShape2D();
	
	public Vector2 CollisionPosition
	{
		get => shape.Position;
		set
		{
			var val = value*new Vector2(OwnerObject.Direction, 1);
			shape?.SetDeferred("position", val);
			originalPosition = value;
		}
	}
	
	public float CollisionRotation
	{
		get => shape.Rotation;
		set
		{
			var val = value*OwnerObject.Direction;
			shape?.SetDeferred("rotation", val);
			originalRotation = value;
		}
	}
	
	public override void _Ready()
	{
		frameCount = 0;
		
		Init();
		
		Active = false;
		Visible = false;
		
		Connect("area_entered", this, nameof(OnAreaEnter));
		
		CollisionLayer ^= 0b11111;//reset five rightmost bits
		CollisionLayer |= 0b100000;//rightmost bits set to 10000
		CollisionMask ^= 0b11111;///reset five rightmost bits
		CollisionMask |= 0b01000;//rightmost bits set to 01000
		
		UpdateHitboxPosition();
	}
	
	public virtual void UpdateHitboxPosition()
	{
		CollisionPosition = originalPosition;
		CollisionRotation = originalRotation;
	}
	
	public virtual void Init() {}
	
	public virtual void LoadProperties() {}
	
	public void LoadExtraProperty<T>(string s, T @default = default(T))
	{
		var toAdd = new ParamRequest(typeof(T), s, @default);
		LoadExtraProperties.Add(s, toAdd);
	}
	
	public void OnAreaEnter(Area2D area)
	{
		if(area is Hurtbox hurtbox && OwnerObject.CanHit(hurtbox.OwnerObject))
		{
			if(hurtbox.OwnerObject is Character c)
			{
				for(var t = c.currentState.GetType(); t.Name != "State"; t = t.BaseType)
				{
					var whitelisted = (whitelistedStates.Count == 0 || whitelistedStates.Contains(t.Name));
					var blacklisted = blacklistedStates.Contains(t.Name);
					if(!whitelisted || blacklisted) return;
				}
			}
			
			hurtbox.HandleHit(this);
			OnHit(hurtbox);
			EmitSignal(nameof(HitboxHit), this, hurtbox);
		}
	}
	
	public virtual void OnHit(Hurtbox hurt)
	{
		
	}
	
	public override void _PhysicsProcess(float delta)
	{
		if(!Active) return;
		++frameCount;
		Loop();
		UpdateHitboxPosition();
		Update();
	}
	
	public override void _Draw()
	{
		//if(!(UpdateScript)n.GetRootNode("UpdateScript").debugCollision) return;
		ZIndex = 4;
		GeometryUtils.DrawCapsuleShape(this,
			shape.Shape as CapsuleShape2D, //shape
			CollisionPosition, //position
			CollisionRotation, //rotation
			GetDrawColor()); //color
	}
	
	public virtual void Loop() {}
	
	public float TeamMult(IHittable n, float chooseFrom) => (n == OwnerObject || n.TeamNumber == OwnerObject.TeamNumber)?chooseFrom:1f;
	
	private float StateMult(IHittable n, Dictionary<string, float> chooseFrom)
	{
		if(chooseFrom is null || !(n is Character c)) return 1f;//only works on characters obv
		
		var f = 1f;
		
		for(var t = c.currentState.GetType(); t.Name != "State"; t = t.BaseType)
		{
			if(chooseFrom.TryGetValue(t.Name.Replace("State", ""), out f))
				return f;
		}
		
		return 1f;
	}
	
	public virtual float GetKnockbackMultiplier(IHittable n) => TeamMult(n, teamKnockbackMult);
	public virtual float GetDamageMultiplier(IHittable n) => TeamMult(n, teamDamageMult);
	public virtual float GetStunMultiplier(IHittable n) => TeamMult(n, teamStunMult);
	
	public virtual Vector2 KnockbackDir(IHittable hitChar) => new Vector2(
		KnockbackDirX(hitChar),
		KnockbackDirY(hitChar)
	);
	
	public virtual float KnockbackDirX(IHittable hitChar)
	{
		switch(horizontalAngleFlipper)
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
		switch(verticalAngleFlipper)
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
		if(stun == 0 && hitpause == 0 && hitlag == 0) return new Color(0.9f,0.9f,0.9f,1);
		switch(horizontalAngleFlipper)
		{
			case AngleFlipper.AwayHitbox:
			case AngleFlipper.AwayCharacter:
			case AngleFlipper.Away:
				return new Color(1, 0.3f, 0.1f, 1);
			case AngleFlipper.None:
				switch(verticalAngleFlipper)
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
	
	public enum AngleFlipper {Direction, Away, AwayHitbox, AwayCharacter, None}
}
