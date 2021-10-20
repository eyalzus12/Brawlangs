using Godot;
using System;

public class WalkStopState : GroundedSlowdownState
{
	public WalkStopState(): base() {}
	public WalkStopState(Character link): base(link) {}
	
	public override void Init()
	{
		ch.PlayAnimation("WalkStop");
	}
	
	protected override bool CalcStateChange()
	{
		if(base.CalcStateChange()) return true;
		else if(ch.IsIdle()) ch.ChangeState("Idle");
		else if(ch.TurnConditional()) ch.ChangeState("WalkTurn");
		else if(ch.InputingDirection()) ch.ChangeState("Walk");
		else return false;
		
		return true;
	}
}
