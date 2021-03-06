using Godot;
using System;

public class CrouchJumpState : BaseCrouchState
{
	bool jumpActive = false;
	
	public CrouchJumpState() : base() {}
	public CrouchJumpState(Character link) : base(link) {}
	
	public override bool IsActionable() => false;
	
	public override void Init()
	{
		ch.TurnConditional();
		MarkForDeletion("player_jump", true);
		ch.vac = Vector2.Zero;
		jump = false;
		jumpActive = false;
		if(ch.currentAttack is null)
		{
			SetupCollisionParamaters();
			AdjustVelocity();
		}
		ch.PlayAnimation("CrouchJumpReady");
		ch.QueueAnimation("CrouchJump");
	}
	
	protected override void LoopActions()
	{
		AdjustVelocity();
		
		if(frameCount >= ch.crouchJumpSquat)
		{
			jumpActive = true;
			var height = Inputs.IsActionReallyPressed("player_jump")?ch.crouchJumpHeight:ch.crouchShorthopHeight;
			ch.fnorm = new Vector2(0,-1);
			ch.vec.y = -height;
			Unsnap();
		}
	}
	
	protected override void DoJump() {}
	
	protected override void DoDodge()
	{
		if(ch.IsActionInCooldown("Dodge")) return;
		ch.ChangeState<DirectionalAirDodgeState>();
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
			ch.ChangeState("Air");
		}
		else return false;
		
		return true;
	}
}
