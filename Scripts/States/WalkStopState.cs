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
		else if(ch.Idle) ch.States.Change("Idle");
		else if(ch.TurnConditional()) ch.States.Change("WalkTurn");
		else if(ch.InputtingHorizontalDirection) ch.States.Change("Walk");
		else return false;
		
		return true;
	}
}
