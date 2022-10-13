using Godot;
using System;

public class RunStopState : WalkStopState
{
	public RunStopState() : base() {}
	public RunStopState(Character link) : base(link) {}
	
	public override void Init()
	{
		ch.PlayAnimation("RunStop", true);
	}
	
	protected override void DoMovement()
	{
		base.DoMovement();
		ch.TurnConditional();
	}
}
