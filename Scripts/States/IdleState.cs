using Godot;
using System;

public partial class IdleState : GroundedSlowdownState
{
	public IdleState(): base() {}
	public IdleState(Character link): base(link) {}
	
	public override void Init()
	{
		ch.vec.x = 0;
		ch.QueueAnimation("Idle", ch.AnimationLooping, false);
	}
	
	protected override bool CalcStateChange()
	{
		if(base.CalcStateChange()) return true;
		else if(!ch.HoldingStrafe && ch.InputtingTurn) ch.States.Change("WalkTurn");
		else if(ch.InputtingHorizontalDirection) ch.States.Change("Walk");
		else return false;
		
		return true;
	}
}
