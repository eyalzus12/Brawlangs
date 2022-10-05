using Godot;
using System;

public class RunStartupState : GroundedState
{
	public RunStartupState() : base() {}
	public RunStartupState(Character link) : base(link) {}
	
	public override bool Actionable => true;
	
	public override void Init()
	{
		ch.TurnConditional();
		ch.ApplySettings("Default");
		ch.Uncrouch();
		ch.PlayAnimation("RunStart", true);
		ch.QueueAnimation("Run", false, false);
		ch.vuc.x = ch.MovementDirection * ch.RunInitialSpeed * (2f-ch.FFric);
	}
	
	protected override void DoMovement()
	{
		ch.vec.x *= (1f-ch.AppropriateFriction);
	}
	
	protected override bool CalcStateChange()
	{
		if(jump) ch.States.Change("RunJump");
		else if(!ch.Grounded) ch.States.Change("Air");
		else if(ch.Grounded && ch.HoldingRun && ch.TurnConditional()) ch.States.Change("RunStartup");
		else if(frameCount >= ch.RunStartup)
		{
			if(ch.HoldingRun)
			{
				ch.vec.x = 0;
				ch.States.Change("Run");
			}
			else ch.States.Change("RunStop");
		}
		else return false;
		
		return true;
	}
}
