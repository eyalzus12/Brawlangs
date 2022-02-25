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
		if(!actionable || jump) return;
		
		if(ch.InputtingHorizontalDirection() && !ch.InputtingTurn())
		{
			ch.direction *= -1;
			ch.ChangeState("BackRoll");
		}
	}
	
	protected override bool CalcStateChange()
	{
		//if(ch.currentAttack != null) return false;
		if(!ch.grounded) ch.ChangeState("Air");
		else if(frameCount >= ch.walkTurn + Math.Round(TURNING_MULT*(1-ch.ffric))) ch.ChangeState("Walk");
		else return false;
		
		return true;
	}
}
