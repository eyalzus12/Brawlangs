using Godot;
using System;

public class ForwardRollStartupState : State
{
	public ForwardRollStartupState() : base() {}
	public ForwardRollStartupState(Character link) : base(link) {}
	
	public override bool IsActionable() => false;
	
	public override void Init()
	{
		ch.PlayAnimation("ForwardRoll");
	}
	
	protected override bool CalcStateChange()
	{
		if(frameCount >= ch.forwardRollStartup)
		{
			ch.ChangeState("ForwardRoll");
			//ch.ApplySettings("ForwardRoll");
		}
		else return false;
		
		return true;
	}
}
