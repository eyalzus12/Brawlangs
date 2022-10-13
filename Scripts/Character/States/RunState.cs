using Godot;
using System;

public class RunState : GroundedState
{
	public RunState() : base() {}
	public RunState(Character link) : base(link) {}
	
	protected override void DoMovement()
	{
		ch.vuc.x = ch.MovementDirection * ch.RunSpeed * (2f-ch.FFric);
	}
	
	protected override bool CalcStateChange()
	{
		if(jump) ch.States.Change("RunJump");
		else if(!ch.Grounded) ch.States.Change("Air");
		else if(!ch.HoldingRun || !ch.InputtingHorizontalDirection) ch.States.Change("RunStop");
		else if(ch.InputtingTurn) ch.States.Change("RunTurn");
		else if(ch.Walled) ch.States.Change("RunWall");
		else return false;
		
		return true;
	}
}
