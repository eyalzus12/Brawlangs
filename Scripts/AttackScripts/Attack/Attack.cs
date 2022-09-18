using Godot;
using System;
using System.Collections.Generic;

public partial class Attack : Node2D
{
	public AttackPart StartPart{get; set;}
	public AttackPart CurrentPart{get; set;}
	public AttackPart LastUsedPart{get; set;}
	public Dictionary<string, AttackPart> Parts{get; set;} = new();
	
	//states
	protected Character ch;
	public IAttacker OwnerObject{get => ch; set
		{
			if(value is Character c) ch = c;
		}
	}
	
	public int frameCount = 0;
	public bool active = false;
	
	public float DamageMult{get; set;}
	public float KnockbackMult{get; set;}
	public int StunMult{get; set;}
	
	public State connected = null;
	
	public List<string> SharesCooldownWith{get; set;} = new();
	
	public Dictionary<string, ParamRequest> LoadExtraProperties = new();
	
	[Signal]
	public delegate void AttackStartsEventHandler(Attack a);
	[Signal]
	public delegate void AttackEndsEventHandler(Attack a);
	
	public float AttackFriction{get; set;}
	
	public Attack() {}
	
	public Attack(AttackPart ap)
	{
		StartPart = ap;
		CurrentPart = StartPart;
	}
	
	public override void _Ready()
	{
		frameCount = 0;
		Init();
	}
	
	public virtual void Start()
	{
		DamageMult = 1f;
		KnockbackMult = 1f;
		StunMult = 1;
		frameCount = 0;
		active = true;
		EmitSignal(nameof(AttackStarts), this);
		if(connected is not null) Disconnect("AttackEnds",new Callable(connected,"SetEnd"));
		Connect("AttackEnds",new Callable(ch.States.Current,"SetEnd"));
		connected = ch.States.Current;
		CurrentPart = StartPart;
		LastUsedPart = null;
		OnStart();
		CurrentPart.Activate();
	}
	
	public virtual void SetPart(string s)
	{
		if(Parts.ContainsKey(s) && Parts[s] is not null)
		{
			LastUsedPart = CurrentPart;
			CurrentPart?.Stop();
			CurrentPart = Parts[s];
			CurrentPart.Activate();
		}
		else Stop();
	}
	
	public virtual void Stop()
	{
		LastUsedPart = CurrentPart;
		active = false;
		OnEnd();
		CurrentPart?.Stop();
		EmitSignal(nameof(AttackEnds), this);
		CurrentPart = null;
	}
	
	public override void _PhysicsProcess(double delta)
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
	public virtual int GetEndlag() => (CurrentPart??LastUsedPart).GetEndlag();
}
