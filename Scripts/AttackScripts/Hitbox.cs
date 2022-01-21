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
	
	public int frameCount = 0;
	
	public AngleFlipper horizontalAngleFlipper;
	public AngleFlipper verticalAngleFlipper;
	
	public Dictionary<string, float> stateKnockbackMult;
	public Dictionary<string, float> stateDamageMult;
	public Dictionary<string, float> stateStunMult;
	
	public Dictionary<string, ParamRequest> LoadExtraProperties = new Dictionary<string, ParamRequest>();
	
	public List<Vector2> activeFrames = new List<Vector2>();
	public Node2D owner;
	
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
		frameCount = 0;
		
		Init();
		Reload();
		
		Active = false;
		Visible = false;
		
		Connect("area_entered", this, nameof(OnHurtboxEnter));
		
		CollisionLayer &= (((ulong)-1)^(0b01111));//rightmost bits set to 10000
		CollisionMask &= (((ulong)-1)^(0b10111));//rightmost bits set to 01000
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
		++frameCount;
		UpdateHitboxPosition();
		Update();
		Loop();
	}
	
	public virtual void UpdateHitboxPosition()
	{
		shape?.SetDeferred("position", originalPosition*new Vector2(direction, 1));
		shape?.SetDeferred("rotation", originalRotation*direction);
	}
	
	public override void _Draw()
	{
		//if(!(UpdateScript)n.GetRootNode("UpdateScript").debugCollision) return;
		ZIndex = 4;
		GeometryUtils.DrawCapsuleShape(this,
			shape.Shape as CapsuleShape2D, //shape
			originalPosition*new Vector2(direction, 1), //position
			originalRotation*direction, //rotation
			GetDrawColor()); //color
	}
	
	public virtual void Loop() {}
	
	private float TeamMult(Node2D n, float chooseFrom) => (n == owner || TeamNum(n) == TeamNum(owner))?chooseFrom:1f;
	private int TeamNum(Node2D n) => (n.Get("teamNumber")??-1).i();
	
	private float StateMult(Node2D n, Dictionary<string, float> chooseFrom)
	{
		if(chooseFrom is null || !(n is Character c)) return 1f;
		
		var f = 1f;
		
		for(var t = c.currentState.GetType(); t.Name != "State"; t = t.BaseType)
		{
			if(chooseFrom.TryGetValue(t.Name.Replace("State", ""), out f))
				return f;
		}
		
		return 1f;
	}
	
	public virtual float GetKnockbackMultiplier(Node2D n) => TeamMult(n, teamKnockbackMult) * StateMult(n, stateKnockbackMult);
	public virtual float GetDamageMultiplier(Node2D n) => TeamMult(n, teamDamageMult) * StateMult(n, stateDamageMult);
	public virtual int GetStunMultiplier(Node2D n) => (int)(TeamMult(n, (float)teamStunMult) * StateMult(n, stateStunMult));
	
	public virtual Vector2 KnockbackDir(Node2D hitChar) => new Vector2(
		KnockbackDirX(hitChar),
		KnockbackDirY(hitChar)
	);
	
	public virtual float KnockbackDirX(Node2D hitChar)
	{
		switch(horizontalAngleFlipper)
		{
			case AngleFlipper.Away:
			case AngleFlipper.AwayCharacter:
				return Math.Sign((hitChar.Position-owner.Position).x);
			case AngleFlipper.AwayHitbox:
				return Math.Sign((hitChar.Position-Position).x);
			case AngleFlipper.None:
				return 1;
			case AngleFlipper.Direction:
			default:
				return direction;
		}
	}
	
	public virtual float KnockbackDirY(Node2D hitChar)
	{
		switch(verticalAngleFlipper)
		{
			case AngleFlipper.Away:
			case AngleFlipper.AwayCharacter:
				return Math.Sign((hitChar.Position-owner.Position).y);
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
	
	public enum AngleFlipper
	{
		Direction,
		Away,
		AwayHitbox,
		AwayCharacter,
		None
	}
	
	public int direction => (owner.Get("direction")??1).i();
}
