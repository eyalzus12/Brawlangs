using Godot;
using System;

public class RunTurnState : GroundedState
{
	public const int TURNING_MULT = 5;
	
	public override bool Actionable => false;
	
	public RunTurnState(): base() {}
	public RunTurnState(Character link): base(link) {}
	
	public override void Init()
	{
		ch.PlayAnimation("RunTurn", true);
	}
	
	protected override void DoMovement()
	{
		ch.vuc.x.Towards(ch.MovementDirection * ch.runSpeed * (2-ch.ffric), ch.runAcceleration * ch.ffric);
	}
	
	protected override void DoDodge() {}
	
	protected override bool CalcStateChange()
	{
		if(!ch.grounded)
			ch.States.Change("Air");
		else if(frameCount >= ch.runTurn + Math.Round(TURNING_MULT*(1-ch.ffric)))
			ch.States.Change(/*"Run"*/"RunStartup");
		else return false;
		
		return true;
	}
	
	public override void OnChange(State newState)
	{
		ch.Turn();
	}
}
