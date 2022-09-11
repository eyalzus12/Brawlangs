using Godot;
using System;

public class WalkState : GroundedState
{
	public WalkState(): base() {}
	public WalkState(Character link): base(link) {}
	
	public override void Init()
	{
		ch.QueueAnimation("Walk", ch.AnimationLooping, true);
	}
	
	protected override bool CalcStateChange()
	{
		if(base.CalcStateChange()) return true;
		else if(ch.InputtingTurn) ch.States.Change("WalkTurn");
		else if(ch.walled) ch.States.Change("WalkWall");
		else if(!ch.InputtingHorizontalDirection) ch.States.Change("WalkStop");
		else return false;
		
		return true;
	}
}
