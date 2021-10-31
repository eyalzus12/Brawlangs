using Godot;
using System;

public class Attack : Node2D
{
	public AttackPart start;
	public AttackPart currentPart;
	public Character ch;
	
	public int frameCount = 0;
	
	public State connected = null;
	
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
	
	public void Start()
	{
		frameCount = 0;
		EmitSignal(nameof(AttackStarts), this);
		if(connected != null) Disconnect("AttackEnds", connected, "SetEnd");
		Connect("AttackEnds", ch.currentState, "SetEnd");
		connected = ch.currentState;
		currentPart = start;
		currentPart.Activate();
		OnStart();
	}
	
	public void SetPart(AttackPart newPart)
	{
		if(newPart is null) Stop();
		else if(currentPart != newPart)
		{
			currentPart.Stop();
			currentPart = newPart;
			currentPart.Activate();
		}
	}
	
	public void Stop()
	{
		OnEnd();
		currentPart.Stop();
		EmitSignal(nameof(AttackEnds), this);
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
}
