using Godot;
using System;

public class IdleState : GroundedState
{
	public IdleState(): base() {}
	public IdleState(Character link): base(link) {}
	
	public override void Init()
	{
		ch.vec.x = 0;
		ch.PlayAnimation("Idle");
	}
	
	protected override void DoMovement()
	{
		if(ch.InputtingHorizontalDirection())
			ch.vec.x = ch.GetInputDirection() * ch.groundAcceleration;
	}
	
	protected override bool CalcStateChange()
	{
		if(base.CalcStateChange()) return true;
		else if(ch.TurnConditional())
			ch.ChangeState("WalkTurn");
		else if(ch.InputtingHorizontalDirection())
			ch.ChangeState("Walk");
		else return false;
		
		return true;
	}
}
