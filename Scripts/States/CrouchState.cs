using Godot;
using System;

public class CrouchState : BaseCrouchState
{
	public CrouchState(): base() {}
	public CrouchState(Character link): base(link) {}
	
	public override void Init()
	{
		ch.vec.x = 0;
	}
	
	protected override void DoMovement()
	{
		ch.TurnConditional();
		
		if(ch.InputingDirection())
			ch.vec.x = ch.GetInputDirection() * ch.groundAcceleration;
	}
	
	protected override bool CalcStateChange()
	{
		if(base.CalcStateChange()) return true;
		else if(ch.InputingDirection()) ch.ChangeState("Crawl");
		else return false;
		
		return true;
	}
}
