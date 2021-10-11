using Godot;
using System;

public class WalkTurnState : GroundedSlowdownState
{
	public const int TURNING_MULT = 5;
	
	public WalkTurnState(): base() {}
	public WalkTurnState(Character link): base(link) {}
	
	protected override bool CalcStateChange()
	{
		if(!ch.grounded) ch.ChangeState("Air");
		else if(frameCount >= ch.walkTurn + Math.Round(TURNING_MULT*(1-ch.ffric))) ch.ChangeState("Walk");
		else return false;
		
		return true;
	}
}
