using Godot;
using System;

public class RunStartupState : GroundedState
{
	public RunStartupState() : base() {}
	public RunStartupState(Character link) : base(link) {}
	
	public override bool Actionable => false;
	
	public override void Init()
	{
		ch.TurnConditional();
		ch.ApplySettings("Default");
		ch.Uncrouch();
		ch.PlayAnimation("RunStart");
		ch.QueueAnimation("Run");
	}
	
	protected override void DoMovement()
	{
		ch.vec.x *= (1-ch.AppropriateFriction);
		ch.vuc.x.Towards(ch.Direction * ch.runInitialSpeed * (2-ch.ffric), ch.runAcceleration * ch.ffric);
	}
	
	protected override bool CalcStateChange()
	{
		if(frameCount >= ch.runStartup)
		{
			ch.vec.x = 0;
			ch.vuc.x = ch.Direction * ch.runInitialSpeed * (2-ch.ffric);
			if(ch.HoldingRun) ch.States.Change("Run");
			else ch.States.Change("RunStop");
			return true;
		}
		return false;
	}
}
