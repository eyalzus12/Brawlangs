using Godot;
using System;

public class GetupState : GroundedState
{
	public GetupState() : base() {}
	public GetupState(Character link) : base(link) {}
	
	public override bool IsActionable() => false;
	
	public override void Init()
	{
		ch.Uncrouch();
		ch.PlayAnimation("Getup");
	}
	
	protected override void DoJump()
	{
		jump = true;
	}
	
	protected override void DoMovement()
	{
		ch.TurnConditional();
		
		if(ch.InputtingHorizontalDirection)
			ch.vec.x = ch.direction * ch.crawlSpeed;
		else ch.vec.x = 0;
	}
	
	protected override bool CalcStateChange()
	{
		if(base.CalcStateChange()) return true;
		else if(frameCount >= ch.getupLength)
		{
			if(ch.IsIdle) ch.ChangeState("Idle");
			else if(ch.InputtingHorizontalDirection) ch.ChangeState("Walk");
			else ch.ChangeState("WalkStop");
		}
		else return false;
		
		return true;
	}
}
