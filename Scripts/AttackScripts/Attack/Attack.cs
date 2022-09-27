using Godot;
using System;
using System.Collections.Generic;

public class Attack : Node2D
{
	public AttackPart StartPart{get; set;}
	public AttackPart CurrentPart{get; set;}
	public AttackPart LastUsedPart{get; set;}
	public Dictionary<string, AttackPart> Parts{get; set;} = new Dictionary<string, AttackPart>();
	
	//states
	protected Character ch;
	public IAttacker OwnerObject{get => ch; set
		{
			if(value is Character c) ch = c;
		}
	}
	
	public int frameCount = 0;
	
	protected bool _active = false;
	public bool Active{get => _active; set
	{
		SetPhysicsProcess(value);
		if(_active && !value) Stop();
		else if(!_active && value) Activate();
		_active = value;
	}}
	
	public float DamageMult{get; set;}
	public float KnockbackMult{get; set;}
	public int StunMult{get; set;}
	
	public State connected = null;
	
	public List<string> SharesCooldownWith{get; set;} = new List<string>();
	
	public Dictionary<string, ParamRequest> LoadExtraProperties = new Dictionary<string, ParamRequest>();
	
	[Signal]
	public delegate void AttackStarts(Attack a);
	[Signal]
	public delegate void AttackEnds(Attack a);
	
	public float AttackFriction{get; set;}
	
	public Attack() {}
	
	public Attack(AttackPart ap)
	{
		StartPart = ap;
		CurrentPart = StartPart;
	}
	
	public override void _Ready()
	{
		SetPhysicsProcess(false);
		frameCount = 0;
		Init();
	}
	
	protected virtual void Activate()
	{
		DamageMult = 1f;
		KnockbackMult = 1f;
		StunMult = 1;
		frameCount = 0;
		
		OnStart();
		
		EmitSignal(nameof(AttackStarts), this);
		Connect("AttackEnds", ch.States["Attack"], "SetEnd");
		
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
		Disconnect("AttackEnds", ch.States["Attack"], "SetEnd");
		
		CurrentPart.Active = false;
		CurrentPart = null;
	}
	
	public override void _PhysicsProcess(float delta)
	{
		Loop();
		++frameCount;
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
	
	public virtual int GetCooldown() => (CurrentPart??LastUsedPart).GetCooldown();
}
