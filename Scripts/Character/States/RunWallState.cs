using System;
using Godot;

public class RunWallState: GroundedState
{
	public RunWallState(): base() {}
	public RunWallState(Character link): base(link) {}
	
	public override void Init()
	{
		ch.PlayAnimation("Run", true);
	}
	
	protected override void DoMovement()
	{
		if(ch.InputtingHorizontalDirection)
			ch.vuc.x = ch.MovementDirection * ch.RunInitialSpeed * (2f-ch.FFric);
		else
			ch.vuc.x = 0;
	}
	
	protected override bool CalcStateChange()
	{
		if(base.CalcStateChange()) return true;
		if(ch.InputtingTurn) ch.States.Change("RunTurn");
		else if(!ch.Walled) ch.States.Change("Run");
		else return false;
		
		return false;
	}
}
