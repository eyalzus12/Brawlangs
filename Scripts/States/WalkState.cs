using Godot;
using System;

public class WalkState : GroundedState
{
	public WalkState(): base() {}
	public WalkState(Character link): base(link) {}
	
	public override void Init()
	{
		ch.PlayAnimation("Walk");
	}
	
	protected override bool CalcStateChange()
	{
		if(base.CalcStateChange()) return true;
		else if(ch.TurnConditional()) ch.ChangeState("WalkTurn");
		else if(ch.walled) ch.ChangeState("WalkWall");
		else if(!ch.InputingDirection()) ch.ChangeState("WalkStop");
		else return false;
		
		return true;
	}
}
