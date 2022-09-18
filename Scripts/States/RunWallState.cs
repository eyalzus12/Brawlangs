using System;
using Godot;

public partial class RunWallState: GroundedState
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
			ch.vuc.x = ch.MovementDirection * ch.runInitialSpeed * (2-ch.ffric);
		else
			ch.vuc.x = 0;
	}
	
	protected override bool CalcStateChange()
	{
		if(base.CalcStateChange()) return true;
		if(ch.InputtingTurn) ch.States.Change("RunTurn");
		else if(!ch.walled) ch.States.Change("Run");
		else return false;
		
		return false;
	}
}
