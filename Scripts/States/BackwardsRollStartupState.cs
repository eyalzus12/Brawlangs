using Godot;
using System;

public class BackwardsRollStartupState : State
{
	public BackwardsRollStartupState() : base() {}
	public BackwardsRollStartupState(Character link) : base(link) {}
	
	public override bool IsActionable() => false;
	
	public override void Init()
	{
		ch.PlayAnimation("BackwardsRoll");
	}
	
	protected override bool CalcStateChange()
	{
		if(frameCount >= ch.backwardsRollStartup)
		{
			ch.ChangeState("BackwardsRoll");
			//ch.ApplySettings("BackwardsRoll");
		}
		else return false;
		
		return true;
	}
}
