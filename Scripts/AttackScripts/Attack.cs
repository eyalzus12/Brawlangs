using Godot;
using System;
using System.Collections.Generic;

public class Attack : Node2D
{
	public AttackPart start;
	public AttackPart currentPart;
	public Character ch;
	
	public int frameCount = 0;
	public bool active = false;
	
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
		ch = GetParent() as Character;
		Init();
	}
	
	public virtual void Start()
	{
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
			if(currentPart != null) currentPart.Stop();
			currentPart = newPart;
			currentPart.Activate();
		}
	}
	
	public virtual void Stop()
	{
		active = false;
		OnEnd();
		if(currentPart != null) currentPart.Stop();
		EmitSignal(nameof(AttackEnds), this);
		currentPart = null;
		AttackPart.ResetGlobals();
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
}
