using Godot;
using System;
using System.Collections.Generic;

public class Attack : Node2D
{
	public AttackPart start;
	public AttackPart currentPart;
	public Character ch;
	
	public AttackPart lastUsedPart;
	
	public int frameCount = 0;
	public bool active = false;
	
	public float damageMult = 1f;
	public float knockbackMult = 1f;
	public int stunMult = 1;
	
	public State connected = null;
	
	public Dictionary<string, ParamRequest> LoadExtraProperties = new Dictionary<string, ParamRequest>();
	
	[Signal]
	public delegate void AttackStarts(Attack a);
	[Signal]
	public delegate void AttackEnds(Attack a);
	
	public float attackFriction = 0.5f;
	
	public Attack() {}
	
	public Attack(AttackPart ap)
	{
		start = ap;
		currentPart = start;
	}
	
	public override void _Ready()
	{
		frameCount = 0;
		Init();
	}
	
	public virtual void Start()
	{
		damageMult = 1f;
		knockbackMult = 1f;
		stunMult = 1;
		frameCount = 0;
		active = true;
		EmitSignal(nameof(AttackStarts), this);
		if(connected != null) Disconnect("AttackEnds", connected, "SetEnd");
		Connect("AttackEnds", ch.currentState, "SetEnd");
		connected = ch.currentState;
		currentPart = start;
		OnStart();
		currentPart.Activate();
	}
	
	public void SetPart(AttackPart newPart)
	{
		//GD.Print($"Part set to {newPart}. current is {currentPart}");
		if(newPart is null) Stop();
		else if(currentPart != newPart)
		{
			lastUsedPart = currentPart;
			if(currentPart != null) currentPart.Stop();
			currentPart = newPart;
			currentPart.Activate();
		}
	}
	
	public virtual void Stop()
	{
		lastUsedPart = currentPart;
		active = false;
		OnEnd();
		if(currentPart != null) currentPart.Stop();
		EmitSignal(nameof(AttackEnds), this);
		currentPart = null;
	}
	
	public override void _PhysicsProcess(float delta)
	{
		++frameCount;
		Loop();
	}
	
	public virtual void Init() {}
	public virtual void OnStart() {}
	public virtual void Loop() {}
	public virtual void OnEnd() {}
	public virtual void OnHit(Hitbox hitbox, Area2D hurtbox) {}
	public virtual bool CanActivate() => true;
	
	public void LoadExtraProperty<T>(string s, T @default = default(T))
	{
		var toAdd = new ParamRequest(typeof(T), s, @default);
		LoadExtraProperties.Add(s, toAdd);
	}
	
	public virtual int GetCooldown() => (currentPart??lastUsedPart).GetCooldown();
	public virtual string GetEndlagAnimation() => (currentPart??lastUsedPart).endlagAnimation;
	public virtual int GetEndlag() => (currentPart??lastUsedPart).GetEndlag();
}
