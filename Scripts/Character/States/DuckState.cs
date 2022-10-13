using Godot;
using System;

public class DuckState : BaseCrouchState
{
	public DuckState() : base() {}
	public DuckState(Character link) : base(link) {}
	
	public override bool Actionable => false;
	
	public override void Init()
	{
		ch.Crouch();
		ch.PlayAnimation("Duck", true);
	}
	
	protected override void DoJump()
	{
		jump = true;
	}
	
	protected override bool CalcStateChange()
	{
		if(base.CalcStateChange()) return true;
		else if(frameCount >= ch.DuckLength)
		{
			if(ch.InputtingHorizontalDirection) ch.States.Change("Crawl");
			else ch.States.Change("Crouch");
		}
		else return false;
		
		return true;
	}
}
