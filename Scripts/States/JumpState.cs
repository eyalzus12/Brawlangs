using Godot;
using System;

public class JumpState : GroundedState
{
	bool jumpActive = false;
	
	public JumpState() : base() {}
	public JumpState(Character link) : base(link) {}
	
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
		ch.PlayAnimation("JumpReady");
		ch.QueueAnimation("Jump");
	}
	
	protected override void LoopActions()
	{
		AdjustVelocity();
		
		if(frameCount >= ch.jumpSquat)
		{
			jumpActive = true;
			//ch.vec.x *= (1f-Math.Abs(ch.fnorm.x));
			var height = Inputs.IsActionReallyPressed("player_jump")?ch.jumpHeight:ch.shorthopHeight;
			ch.fnorm = new Vector2(0,-1);
			ch.vec.y = -height;
			Unsnap();
		}
	}
	
	protected override void DoJump() {}
	
	protected override void DoDodge()
	{
		if(ch.IsActionInCooldown("Dodge")) return;
		ch.ChangeState("DirectionalAirDodge");
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
