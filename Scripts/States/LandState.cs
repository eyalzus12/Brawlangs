using Godot;
using System;

public class LandState : GroundedState
{
	public LandState() : base() {}
	public LandState(Character link) : base(link) {}
	
	public override bool ShouldDrop => ch.downHeld && ch.HoldingRun;
	
	public override void Init()
	{
		SetPlatformDropping();
		if(ch.onSemiSolid && ShouldDrop) return;
		
		ch.vac = Vector2.Zero;
		ch.RestoreOptionsOnGroundTouch();
		ch.Tags["Grounded"] = StateTag.Starting;
		
		AdjustVelocity();
		ch.PlayAnimation("Land", true);
	}
	
	protected override void DoJump() {}
	protected override void DoMovement() {}
	
	protected override bool CalcStateChange()
	{
		if(base.CalcStateChange()) return true;
		
		if(frameCount >= ch.impactLand)
		{
			bool turn = ch.TurnConditional();
			
			if(Inputs.IsActionJustPressed("Jump"))
				ch.States.Change("Jump");
			else if(Inputs.IsActionPressed("Down") && !ch.onSemiSolid) 
				ch.States.Change(ch.InputtingHorizontalDirection?ch.walled?"CrawlWall":"Crawl":"Crouch");
			else if(ch.Idle)
				ch.States.Change("Idle");
			else if(turn)
				ch.States.Change("WalkTurn");
			else if(ch.InputtingHorizontalDirection)
				ch.States.Change(ch.walled?"WalkWall":"Walk");
			else
				ch.States.Change("WalkStop");
			
			return true;
		}
		else return false;
	}
}
