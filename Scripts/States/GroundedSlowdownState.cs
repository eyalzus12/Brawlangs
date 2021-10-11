using Godot;
using System;

public class GroundedSlowdownState : GroundedState
{
	public GroundedSlowdownState(): base() {}
	public GroundedSlowdownState(Character link): base(link) {}
	
	protected override void DoMovement()
	{
		DoFriction();
	}
}
