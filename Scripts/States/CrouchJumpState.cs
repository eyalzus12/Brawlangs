using Godot;
using System;

public class CrouchJumpState : BaseCrouchState
{
	bool jumpActive = false;
	
	public CrouchJumpState() : base() {}
	public CrouchJumpState(Character link) : base(link) {}
	
	public override bool Actionable => false;
	
	public override void Init()
	{
		ch.TurnConditional();
		MarkForDeletion("Jump", true);
		ch.vac = Vector2.Zero;
		jump = false;
		jumpActive = false;
		if(ch.CurrentAttack is null)
		{
			SetupCollisionParamaters();
			AdjustVelocity();
		}
		
		ch.PlayAnimation("CrouchJumpReady", true);
		ch.QueueAnimation("CrouchJump", false, false);
	}
	
	protected override void LoopActions()
	{
		AdjustVelocity();
		
		if(frameCount >= ch.crouchJumpSquat)
		{
			jumpActive = true;
			var height = Inputs.IsActionReallyPressed("Jump")?ch.crouchJumpHeight:ch.crouchShorthopHeight;
			ch.fnorm = new Vector2(0,-1);
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
		if(!ch.grounded && jumpActive)
		{
			ch.ApplySettings("Default");
			ch.States.Change("Air");
		}
		else return false;
		
		return true;
	}
}
