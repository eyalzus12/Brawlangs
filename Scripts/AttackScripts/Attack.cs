using Godot;
using System;

public class Attack : Node2D
{
	public AttackPart start;
	public AttackPart currentPart;
	public Character ch;
	
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
		ch = GetParent() as Character;
		Init();
	}
	
	public void Start()
	{
		currentPart = start;
		currentPart.Activate();
		EmitSignal(nameof(AttackStarts), this);
		var a = ch.ChangeState("Attack") as AttackState;
		GD.Print($"connecting from attack {this}");
		Connect("AttackEnds", a, "SetEnd");
		a.att = this;
		/*var scl = GetSignalConnectionList("AttackEnds");
			
		bool deez = false;
		foreach(var d in scl)
		{
			var dict = d as Godot.Collections.Dictionary;
			if(dict["method"].ToString() == "SetEnd")
			{
				deez = true;
				break;
			}
		}
			
		if(!deez) */
		
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
		Loop();
	}
	
	public virtual void Init() {}
	public virtual void OnStart() {}
	public virtual void Loop() {}
	public virtual void OnEnd() {}
	public virtual void OnHit(Hitbox hitbox, Area2D hurtbox) {}
}
