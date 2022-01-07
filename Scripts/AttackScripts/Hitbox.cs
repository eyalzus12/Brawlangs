using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Hitbox : Area2D
{
	[Signal]
	public delegate void HitboxHit(Hitbox hitbox, Area2D hurtbox);
	
	[Export]
	public Vector2 setKnockback = Vector2.Zero;
	[Export]
	public Vector2 varKnockback = Vector2.Zero;
	[Export]
	public int stun = 0;
	[Export]
	public int hitpause = 0;
	[Export]
	public int hitlag = 0;
	[Export]
	public float damage = 0f;
	[Export]
	public int hitPriority = 0;
	[Export]
	public float teamKnockbackMult = 1f;
	[Export]
	public float teamDamageMult = 1f;
	[Export]
	public int teamStunMult = 1;
	[Export]
	public Vector2 momentumCarry = Vector2.Zero;
	[Export]
	public string hitSound = "DefaultHit";
	
	public AngleFlipper horizontalAngleFlipper;
	public AngleFlipper verticalAngleFlipper;
	
	public Dictionary<string, float> stateKnockbackMult;
	public Dictionary<string, float> stateDamageMult;
	public Dictionary<string, float> stateStunMult;
	
	public Dictionary<string, ParamRequest> LoadExtraProperties = new Dictionary<string, ParamRequest>();
	
	public List<Vector2> activeFrames = new List<Vector2>();
	public Character ch;
	
	private bool active = false;
	
	public Vector2 originalPosition = Vector2.Zero;
	public float originalRotation = 0f;
	
	public bool Active
	{
		get => active;
		set
		{
			active = value;
			Visible = value;
			shape?.SetDeferred("disabled", !active);
		}
	}
	
	public CollisionShape2D shape = new CollisionShape2D();
	
	public override void _Ready()
	{
		//shape.SetDeferred("position",
		//	new Vector2(ch.direction*shape.Position.x, shape.Position.y));
		Init();
		Reload();
				
		Active = false;
		Visible = false;
		
		Connect("area_entered", this, nameof(OnHurtboxEnter));
		
		CollisionLayer = 0b1_0000;
		CollisionMask = 0b0_1000;
	}
	
	public virtual void Init() {}
	
	public void LoadExtraProperty<T>(string s, T @default = default(T))
	{
		var toAdd = new ParamRequest(typeof(T), s, @default);
		LoadExtraProperties.Add(s, toAdd);
	}
	
	public virtual void Reload()
	{
		try
		{
			shape = GetChildren().FilterType<CollisionShape2D>().Single();
		}
		catch(InvalidOperationException)
		{
			if(GetChildren().Count >= 2)
			//there's two collision shapes for some reason
				GD.Print("Hitbox {this.ToString()} of character {ch.Name} has more than one collision shape");
			//another option is that there's no collision, probably meaning it was added before its collision
			//the builder should manually call this function after adding collision
		}
		
		originalPosition = shape?.Position ?? default(Vector2);
		originalRotation = shape?.Rotation ?? 0;
	}
	
	public void OnHurtboxEnter(Area2D area)
	{
		OnHit(area);
		EmitSignal(nameof(HitboxHit), this, area);
		//GD.Print("hit");
	}
	
	public virtual void OnHit(Area2D area) {}
	
	public override void _PhysicsProcess(float delta)
	{
		if(!Active) return;
		shape?.SetDeferred("position", originalPosition*new Vector2(ch.direction, 1));
		shape?.SetDeferred("rotation", originalRotation*ch.direction);
		Update();
		Loop();
	}
	
	public override void _Draw()
	{
		//if(!(UpdateScript)n.GetRootNode("UpdateScript").debugCollision) return;
		ZIndex = 4;
		GeometryUtils.DrawCapsuleShape(this,
			shape.Shape as CapsuleShape2D, //shape
			originalPosition*new Vector2(ch.direction, 1), //position
			originalRotation*ch.direction, //rotation
			GetDrawColor()); //color
		
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
	
	public virtual void Loop() {}
	
	private float TeamMult(Character c, float chooseFrom) => (c == ch || c.teamNumber == ch.teamNumber)?chooseFrom:1f;
	
	private float StateMult(Character c, Dictionary<string, float> chooseFrom)
	{
		if(chooseFrom is null) return 1f;
		
		var f = 1f;
		
		for(var t = c.currentState.GetType(); t.Name != "State"; t = t.BaseType)
		{
			if(chooseFrom.TryGetValue(t.Name.Replace("State", ""), out f))
				return f;
		}
		
		return 1f;
	}
	
	public virtual float GetKnockbackMultiplier(Character c) => TeamMult(c, teamKnockbackMult) * StateMult(c, stateKnockbackMult);
	public virtual float GetDamageMultiplier(Character c) => TeamMult(c, teamDamageMult) * StateMult(c, stateDamageMult);
	public virtual int GetStunMultiplier(Character c) => (int)(TeamMult(c, (float)teamStunMult) * StateMult(c, stateStunMult));
	
	public virtual Vector2 KnockbackDir(Character hitter, Character hitee) => new Vector2(
		KnockbackDirX(hitter, hitee),
		KnockbackDirY(hitter, hitee)
	);
	
	public virtual float KnockbackDirX(Character hitter, Character hitee)
	{
		switch(horizontalAngleFlipper)
		{
			case AngleFlipper.Away:
			case AngleFlipper.AwayCharacter:
				return Math.Sign((hitee.Position-hitter.Position).x);
			case AngleFlipper.AwayHitbox:
				return Math.Sign((hitee.Position-Position).x);
			case AngleFlipper.None:
				return 1;
			case AngleFlipper.Direction:
			default:
				return hitter.direction;
		}
	}
	
	public virtual float KnockbackDirY(Character hitter, Character hitee)
	{
		switch(verticalAngleFlipper)
		{
			case AngleFlipper.Away:
			case AngleFlipper.AwayCharacter:
				return Math.Sign((hitee.Position-hitter.Position).y);
			case AngleFlipper.AwayHitbox:
				return Math.Sign((hitee.Position-Position).y);
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
	
	public enum AngleFlipper
	{
		Direction,
		Away,
		AwayHitbox,
		AwayCharacter,
		None
	}
}
