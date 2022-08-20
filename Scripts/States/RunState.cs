using Godot;
using System;

public class RunState : GroundedState
{
	public RunState() : base() {}
	public RunState(Character link) : base(link) {}
	
	protected override void DoMovement()
	{
		ch.vuc.x.Towards(ch.Direction * ch.runSpeed * (2-ch.ffric), ch.runAcceleration * ch.ffric);
	}
	
	protected override bool CalcStateChange()
	{
		if(jump) ch.States.Change("RunJump");
		else if(!ch.grounded) ch.States.Change("Air");
		else if(!ch.HoldingRun || !ch.InputtingHorizontalDirection) ch.States.Change("RunStop");
		else if(ch.InputtingTurn) ch.States.Change("RunTurn");
		else if(ch.walled) ch.States.Change("RunWall");
		else return false;
		
		return true;
	}
}
