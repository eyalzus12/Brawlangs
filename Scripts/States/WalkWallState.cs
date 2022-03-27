using System;
using Godot;

public class WalkWallState: GroundedState
{
	public WalkWallState(): base() {}
	public WalkWallState(Character link): base(link) {}
	
	public override void Init()
	{
		ch.PlayAnimation("Walk");
	}
	
	protected override void DoMovement()
	{
		if(ch.InputtingHorizontalDirection)
			ch.vec.x = ch.direction * ch.groundAcceleration;
		else
			ch.vec.x = 0;
	}
	
	protected override bool CalcStateChange()
	{
		if(base.CalcStateChange()) return true;
		if(ch.TurnConditional()) ch.ChangeState("WalkTurn");
		else if(!ch.walled) ch.ChangeState("Idle");
		else return false;
		
		return false;
	}
}
