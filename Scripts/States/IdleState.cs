using Godot;
using System;

public class IdleState : GroundedSlowdownState
{
	public IdleState(): base() {}
	public IdleState(Character link): base(link) {}
	
	public override void Init()
	{
		ch.vec.x = 0;
		ch.PlayAnimation("Idle");
	}
	
	protected override bool CalcStateChange()
	{
		if(base.CalcStateChange()) return true;
		else if(ch.TurnConditional()) ch.States.Change("WalkTurn");
		else if(ch.InputtingHorizontalDirection) ch.States.Change("Walk");
		else return false;
		
		return true;
	}
}
