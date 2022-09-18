using Godot;
using System;

public partial class GetupState : GroundedState
{
	public GetupState() : base() {}
	public GetupState(Character link) : base(link) {}
	
	public override bool Actionable => false;
	
	public override void Init()
	{
		ch.Uncrouch();
		ch.PlayAnimation("Getup", true);
	}
	
	protected override void DoJump()
	{
		jump = true;
	}
	
	protected override void DoMovement()
	{
		ch.TurnConditional();
		
		if(ch.InputtingHorizontalDirection)
			ch.vec.x = ch.MovementDirection * ch.crawlSpeed;
		else ch.vec.x = 0;
	}
	
	protected override bool CalcStateChange()
	{
		if(base.CalcStateChange()) return true;
		else if(frameCount >= ch.getupLength)
		{
			ch.Uncrouch();
			if(ch.Idle) ch.States.Change("Idle");
			else if(ch.InputtingHorizontalDirection) ch.States.Change("Walk");
			else ch.States.Change("WalkStop");
		}
		else return false;
		
		return true;
	}
}
