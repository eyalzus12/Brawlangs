using Godot;
using System;

public class BaseCrouchState : GroundedState
{
	public BaseCrouchState(): base() {}
	public BaseCrouchState(Character link): base(link) {}
	
	protected override void DoMovement()
	{
		ch.TurnConditional();
		
		if(ch.InputingDirection())
			ch.vec.x = ch.direction * ch.crawlSpeed;
		else
			ch.vec.x = 0;
	}
	
	protected override bool CalcStateChange()
	{
		bool temp = jump;
		jump = false;//workaround to prevent transition to normal jump
		if(base.CalcStateChange()) return true;
		jump = temp;
		
		if(jump)
			ch.ChangeState("Jump");
		else if(!Inputs.IsActionPressed("player_down"))
			ch.ChangeState("Getup");
		else if(ch.InputingDirection())
			ch.ChangeState("Crawl");
		else return false;
		
		return true;
	}
}
