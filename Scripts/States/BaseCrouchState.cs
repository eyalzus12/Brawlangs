using Godot;
using System;

public class BaseCrouchState : GroundedState
{
	public BaseCrouchState(): base() {}
	public BaseCrouchState(Character link): base(link) {}
	
	protected override void DoMovement()
	{
		ch.TurnConditional();
		
		if(ch.InputtingHorizontalDirection)
			ch.vec.x = ch.direction * ch.crawlSpeed;
		else
			ch.vec.x = 0;
	}
	
	protected override bool CalcStateChange()
	{
		if(jump) ch.ChangeState<CrouchJumpState>();
		else if(base.CalcStateChange()) return true;
		else if(!Inputs.IsActionPressed("player_down")) ch.ChangeState<GetupState>();
		else return false;
		
		return true;
	}
}
