using System;
using Godot;

public class WalkWallState: GroundedState
{
	public WalkWallState(): base() {}
	public WalkWallState(Character link): base(link) {}
	
	public override void Init()
	{
		ch.QueueAnimation("Walk", ch.AnimationLooping, true);
	}
	
	protected override void DoMovement()
	{
		if(ch.InputtingHorizontalDirection)
			ch.vec.x = ch.MovementDirection * ch.AppropriateAcceleration;
		else
			ch.vec.x = 0;
	}
	
	protected override bool CalcStateChange()
	{
		if(base.CalcStateChange()) return true;
		if(ch.InputtingTurn) ch.States.Change("WalkTurn");
		else if(!ch.walled) ch.States.Change("Walk");
		else return false;
		
		return false;
	}
}
