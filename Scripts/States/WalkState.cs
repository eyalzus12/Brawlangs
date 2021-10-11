using Godot;
using System;

public class WalkState : GroundedState
{
	public WalkState(): base() {}
	public WalkState(Character link): base(link) {}
	
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
