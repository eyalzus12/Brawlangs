using Godot;
using System;

public partial class WalkTurnState : GroundedSlowdownState
{
	public const int TURNING_MULT = 5;
	
	public WalkTurnState(): base() {}
	public WalkTurnState(Character link): base(link) {}
	
	public override void Init()
	{
		ch.QueueAnimation("WalkTurn", ch.AnimationLooping, true);
	}
	
	protected override void DoDodge() {}
	
	protected override bool CalcStateChange()
	{
		if(Actionable && ch.ShouldInitiateRun) ch.States.Change("RunStartup");
		else if(!ch.grounded) ch.States.Change("Air");
		else if(frameCount >= ch.walkTurn + Math.Round(TURNING_MULT*(1-ch.ffric))) ch.States.Change("Walk");
		else return false;
		
		return true;
	}
	
	public override void OnChange(State newState)
	{
		ch.Turn();
		ch.vuc.x *= -1;
	}
}
