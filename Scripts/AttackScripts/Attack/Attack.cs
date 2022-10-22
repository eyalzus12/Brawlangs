using Godot;
using System;
using System.Collections.Generic;

public class Attack : Node2D
{
	public AttackPart StartPart{get; set;}
	public AttackPart CurrentPart{get; set;}
	public AttackPart LastUsedPart{get; set;}
	public Dictionary<string, AttackPart> Parts{get; set;} = new Dictionary<string, AttackPart>();
	
	public IAttacker OwnerObject{get; set;}
	
	public long FrameCount{get; set;} = 0;
	
	protected bool _active = false;
	public bool Active{get => _active; set
	{
		SetPhysicsProcess(value);
		if(_active && !value) Stop();
		else if(!_active && value) Activate();
		_active = value;
	}}
	
	public float DamageDoneMult{get; set;}
	public float KnockbackDoneMult{get; set;}
	public int StunDoneMult{get; set;}
	
	public List<string> SharesCooldownWith{get; set;} = new List<string>();
	
	public Dictionary<string, ParamRequest> LoadExtraProperties{get; set;} = new Dictionary<string, ParamRequest>();
	
	[Signal]
	public delegate void AttackStarts(Attack a);
	[Signal]
	public delegate void AttackEnds(Attack a);
	
	public float AttackFriction{get; set;} = 1f;
	
	public Attack() {}
	
	public Attack(AttackPart ap)
	{
		StartPart = ap;
		CurrentPart = StartPart;
	}
	
	public override void _Ready()
	{
		SetPhysicsProcess(false);
		FrameCount = 0;
		Init();
	}
	
	protected virtual void Activate()
	{
		DamageDoneMult = 1f;
		KnockbackDoneMult = 1f;
		StunDoneMult = 1;
		
		FrameCount = 0;
		
		OnStart();
		
		EmitSignal(nameof(AttackStarts), this);
		
		CurrentPart = StartPart;
		LastUsedPart = null;
		
		CurrentPart.Active = true;
	}
	
	public virtual void SetPart(string s)
	{
		if(Parts.ContainsKey(s) && Parts[s] != null)
		{
			LastUsedPart = CurrentPart;
			if(CurrentPart != null) CurrentPart.Active = false;
			CurrentPart = Parts[s];
			CurrentPart.Active = true;
		}
		else Active = false;
	}
	
	protected virtual void Stop()
	{
		LastUsedPart = CurrentPart;
		
		OnEnd();
		
		EmitSignal(nameof(AttackEnds), this);
		
		CurrentPart.Active = false;
		CurrentPart = null;
	}
	
	public override void _PhysicsProcess(float delta)
	{
		Loop();
		++FrameCount;
	}
	
	public virtual void Init() {}
	public virtual void LoadProperties() {}
	public virtual void OnStart() {}
	public virtual void Loop() {}
	public virtual void OnEnd() {}
	public virtual void OnHit(Hitbox hitbox, Hurtbox hurtbox) {}
	public virtual bool CanActivate() => true;
	
	public void LoadExtraProperty<T>(string s, T @default = default(T))
	{
		var toAdd = new ParamRequest(typeof(T), s, @default);
		LoadExtraProperties.Add(s, toAdd);
	}
	
	public virtual int FinalCooldown => (CurrentPart??LastUsedPart).FinalCooldown;
}
