using Godot;
using System;

public class RunTurnState : GroundedState
{
	public const int TURNING_MULT = 5;
	
	public RunTurnState(): base() {}
	public RunTurnState(Character link): base(link) {}
	
	public override void Init()
	{
		ch.PlayAnimation("Turn");
	}
	
	protected override void DoMovement() {}
	
	protected override void DoDodge()
	{
		if(!Actionable || jump) return;
		
		if(!ch.InputtingTurn)
		{
			ch.Turn();
			base.DoDodge();
		}
	}
	
	protected override bool CalcStateChange()
	{
		if(!ch.grounded)
			ch.States.Change("Air");
		else if(frameCount >= ch.runTurn + Math.Round(TURNING_MULT*(1-ch.ffric)))
			ch.States.Change("Run");
		else return false;
		
		return true;
	}
	
	public override void OnChange(State newState)
	{
		ch.vuc.x *= -1;
	}
}
