using Godot;
using System;

public partial class RunJumpState : RunState
{
	public RunJumpState() : base() {}
	public RunJumpState(Character link) : base(link) {}
	
	public bool jumpActive = false;
	
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
		
		ch.PlayAnimation("JumpRunReady", true);
		ch.QueueAnimation("JumpRun", false, false);
	}
	
	protected override void LoopActions()
	{
		AdjustVelocity();
		
		if(frameCount >= ch.runJumpSquat)
		{
			jumpActive = true;
			ch.fnorm = new Vector2(0,-1);
			ch.vuc.x = Math.Sign(ch.vuc.x) * ch.runJumpSpeed * (2-ch.ffric);
			ch.vuc.y = -ch.runJumpHeight;
			Unsnap();
		}
	}
	
	protected override void DoJump() {}
	protected override void DoDodge() {}
	
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
