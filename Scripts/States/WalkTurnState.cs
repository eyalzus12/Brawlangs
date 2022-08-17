using Godot;
using System;

public class WalkTurnState : GroundedSlowdownState
{
	public const int TURNING_MULT = 5;
	
	public WalkTurnState(): base() {}
	public WalkTurnState(Character link): base(link) {}
	
	public override void Init()
	{
		ch.PlayAnimation("Turn");
	}
	
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
		if(Actionable && ch.ShouldInitiateRun)
		{
			ch.TurnConditional();
			ch.States.Change("RunStartup");
		}
		else if(!ch.grounded) ch.States.Change("Air");
		else if(frameCount >= ch.walkTurn + Math.Round(TURNING_MULT*(1-ch.ffric))) ch.States.Change("Walk");
		else return false;
		
		return true;
	}
}
