using Godot;
using System;

public class JumpState : GroundedState
{
	public bool jumpActive = false;
	public bool forceShortHop = false;
	
	public JumpState() : base() {}
	public JumpState(Character link) : base(link) {}
	
	public override bool Actionable => false;
	
	public override void Init()
	{
		ch.TurnConditional();
		MarkForDeletion("Jump", true);
		ch.vac = Vector2.Zero;
		jump = false;
		forceShortHop = false;
		jumpActive = false;
		if(ch.CurrentAttack is null)
		{
			SetupCollisionParamaters();
			AdjustVelocity();
		}
		
		ch.PlayAnimation("JumpReady", true);
		ch.QueueAnimation("Jump", false, false);
	}
	
	protected override void LoopActions()
	{
		if(Inputs.IsActionPressed("Run")) forceShortHop = true;
		AdjustVelocity();
		
		if(frameCount >= ch.JumpSquat)
		{
			jumpActive = true;
			//ch.vec.x *= (1f-Math.Abs(ch.fnorm.x));
			var height = (!forceShortHop && Inputs.IsActionReallyPressed("Jump"))?ch.JumpHeight:ch.ShorthopHeight;
			ch.FNorm = Vector2.Up;
			ch.vec.y = -height;
			Unsnap();
		}
	}
	
	protected override void DoJump() {}
	
	protected override void DoDodge()
	{
		if(ch.Cooldowns.InCooldown("Dodge") || !ch.Resources.Has("Dodge")) return;
		ch.States.Change((ch.InputtingNatDodge?"Spot":"Directional") + "AirDodge");
		MarkForDeletion("Dodge", true);
		MarkForDeletion("NDodge", true);
	}
	
	protected override void DoMovement()
	{
		if(!jumpActive) base.DoMovement();
	}
	
	protected override bool CalcStateChange()
	{
		if(!ch.Grounded && jumpActive)
		{
			ch.ApplySettings("Default");
			ch.States.Change("Air");
		}
		else return false;
		
		return true;
	}
}
