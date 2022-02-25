using Godot;
using System;

public class CrouchState : BaseCrouchState
{
	public CrouchState(): base() {}
	public CrouchState(Character link): base(link) {}
	
	public override void Init()
	{
		ch.vec.x = 0;
		ch.PlayAnimation("Crouch");
	}
	
	protected override void DoMovement()
	{
		ch.TurnConditional();
		
		if(ch.InputtingHorizontalDirection())
			ch.vec.x = ch.GetInputDirection() * ch.groundAcceleration;
	}
	
	protected override bool CalcStateChange()
	{
		if(base.CalcStateChange()) return true;
		else if(ch.InputtingHorizontalDirection()) ch.ChangeState("Crawl");
		else return false;
		
		return true;
	}
}
