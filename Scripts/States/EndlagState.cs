using Godot;
using System;

public class EndlagState : State
{
	public EndlagState() : base() {}
	public EndlagState(Character link) : base(link) {}
	
	public int endlag = 0;
	public Attack att = null;
	
	public override bool IsActionable() => false;
	
	public override void Init()
	{
		endlag = 0;
		att = null;
		SetupCollisionParamaters();
		//ch.PlayAnimation(att.endlagAnimation);
		//figure out a way to set the animation after the attack exists
	}
	
	protected override void DoMovement()
	{
		if(att is null) return;
		ch.vec.x *= 1f-(att.attackFriction*(ch.grounded?ch.ffric:1f));
	}
	
	protected override void DoGravity()
	{
		if(!ch.grounded) ch.vec.y.Lerp(ch.fallSpeed, ch.gravity);
	}
	
	public override void SetInputs()
	{
		SetHorizontalAlternatingInputs();
		SetDownHoldingInput();
		SetUpHoldingInput();
	}
	
	protected override bool CalcStateChange()
	{
		if(frameCount >= endlag)
		{
			if(!ch.downHeld) ch.Uncrouch();
			ch.ChangeState(ch.grounded?(ch.downHeld?"Crawl":"Walk"):ch.walled?"WallLand":"Air");
		}
		else return false;
		
		return true;
	}
}
