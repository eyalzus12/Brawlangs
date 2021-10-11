using Godot;
using System;

public class DuckState : BaseCrouchState
{
	public DuckState() : base() {}
	public DuckState(Character link) : base(link) {}
	
	public override bool IsActionable() => false;
	
	public override void Init()
	{
		ch.Crouch();
		ch.PlayAnimation("Duck");
	}
	
	protected override void DoJump()
	{
		jump = true;
	}
	
	protected override bool CalcStateChange()
	{
		if(base.CalcStateChange()) return true;
		else if(frameCount >= ch.duckLength)
		{
			if(ch.InputingDirection()) ch.ChangeState("Crawl");
			else ch.ChangeState("Crouch");
		}
		else return false;
		
		return true;
	}
}
