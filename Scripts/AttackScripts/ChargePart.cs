using Godot;
using System;

public class ChargePart : Node2D
{
	[Export]
	public int MaxCharge = 10;
	[Export]
	public bool hasMovement = false;
	[Export]
	public Vector2 movement = default;
	
	public int frameCount = 0;
	public bool active = false;
	public Character ch;
	public ChargeableAttack att;
	public string input = "player_heavy_attack";
	
	//public Func<int, int> DamageMultiplier = (h)=>h;
	
	public override void _Ready()
	{
		frameCount = 0;
		att = GetParent() as ChargeableAttack;
		ch = att.ch;
		Init();
	}
	
	public void Activate()
	{
		active = true;
		//GD.Print("activated");
		frameCount = 0;
		if(hasMovement)
		{
			ch.vec = movement * new Vector2(ch.direction, 1);
			if(ch.grounded) ch.vec.y = State.VCF;
		}
		
		OnStart();
	}
	
	public override void _PhysicsProcess(float delta)
	{
		if(!active) return;
		++frameCount;
		Loop();
		if(frameCount >= MaxCharge || !ch.Inputs.IsActionPressed(input))
		{
			Stop();
			att.SetPart(att.start);
		}
	}
	
	public virtual void Stop()
	{
		OnEnd();
		active = false;
		frameCount = 0;
	}
	
	public virtual void Init(){}
	public virtual void Loop(){}
	public virtual void OnStart(){}
	public virtual void OnEnd(){}
}
