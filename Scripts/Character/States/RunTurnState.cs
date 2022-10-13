using Godot;
using System;

public class RunTurnState : GroundedState
{
	public const int TURNING_MULT = 5;
	
	public override bool Actionable => true;
	
	public RunTurnState(): base() {}
	public RunTurnState(Character link): base(link) {}
	
	public override void Init()
	{
		ch.PlayAnimation("RunTurn", true);
	}
	
	protected override void DoMovement()
	{
		ch.vuc.x.Towards(ch.MovementDirection * ch.RunSpeed * (2f-ch.FFric), ch.RunAcceleration * ch.FFric);
	}
	
	protected override void DoDodge() {}
	
	protected override bool CalcStateChange()
	{
		if(!ch.Grounded)
			ch.States.Change("Air");
		else if(frameCount >= ch.RunTurn + Math.Round(TURNING_MULT*(1f-ch.FFric)))
			ch.States.Change(/*"Run"*/"RunStartup");
		else return false;
		
		return true;
	}
	
	public override void OnChange(State newState)
	{
		ch.TurnConditional();
	}
}
