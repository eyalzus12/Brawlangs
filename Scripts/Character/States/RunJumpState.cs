using Godot;
using System;

public class RunJumpState : RunState
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
		
		if(frameCount >= ch.RunJumpSquat)
		{
			jumpActive = true;
			ch.FNorm = Vector2.Up;
			ch.vuc.x = Math.Sign(ch.vuc.x) * ch.RunJumpSpeed * (2-ch.FFric);
			ch.vuc.y = -ch.RunJumpHeight;
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
		if(!ch.Grounded && jumpActive)
		{
			ch.ApplySettings("Default");
			ch.States.Change("Air");
		}
		else return false;
		
		return true;
	}
}
