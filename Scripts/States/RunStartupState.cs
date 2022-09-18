using Godot;
using System;

public partial class RunStartupState : GroundedState
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
		ch.vuc.x = ch.MovementDirection * ch.runInitialSpeed * (2-ch.ffric);
	}
	
	protected override void DoMovement()
	{
		ch.vec.x *= (1-ch.AppropriateFriction);
	}
	
	protected override bool CalcStateChange()
	{
		if(jump) ch.States.Change("RunJump");
		else if(!ch.grounded) ch.States.Change("Air");
		else if(ch.grounded && ch.HoldingRun && ch.TurnConditional()) ch.States.Change("RunStartup");
		else if(frameCount >= ch.runStartup)
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
