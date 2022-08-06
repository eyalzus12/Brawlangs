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
			ch.vec.x = ch.Direction * ch.crawlSpeed;
		else
			ch.vec.x = 0;
	}
	
	protected override bool CalcStateChange()
	{
		if(jump) ch.States.Change("CrouchJump");
		else if(base.CalcStateChange()) return true;
		else if(!Inputs.IsActionPressed("player_down")) ch.States.Change("Getup");
		else return false;
		
		return true;
	}
}
