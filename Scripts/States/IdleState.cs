using Godot;
using System;

public class IdleState : GroundedState
{
	public IdleState(): base() {}
	public IdleState(Character link): base(link) {}
	
	public override void Init()
	{
		ch.vec.x = 0;
	}
	
	protected override void DoMovement()
	{
		if(ch.InputingDirection())
			ch.vec.x = ch.GetInputDirection() * ch.groundAcceleration;
	}
	
	protected override bool CalcStateChange()
	{
		if(base.CalcStateChange()) return true;
		else if(ch.TurnConditional())
			ch.ChangeState("WalkTurn");
		else if(ch.InputingDirection())
			ch.ChangeState("Walk");
		else return false;
		
		return true;
	}
}
