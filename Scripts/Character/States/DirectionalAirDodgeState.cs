using Godot;
using System;

public class DirectionalAirDodgeState : GenericInvincibleState
{
	public DirectionalAirDodgeState() : base() {}
	public DirectionalAirDodgeState(Character link) : base(link) {}
	
	public override bool ShouldDrop => ch.DownInput && ch.HoldingRun;
	
	public bool touchedWall = false;
	public Vector2 movement;
	
	public override bool Actionable => false;
	
	public override int Startup => ch.DirectionalAirDodgeStartup;
	public override int IFrames => ch.DirectionalAirDodgeLength;
	public override int Endlag => ch.DirectionalAirDodgeEndlag;
	public override string ActionName => "Dodge";
	public override string StateAnimation => "DirectionalAirDodge";
	
	protected override void LoopActions()
	{
		base.LoopActions();
		CheckWavedashOption();
		if(ch.Walled && ch.Resources.Has("Clings")) touchedWall = true;
		if(IFramesStarted)
		{
			ch.vuc.x *= (1f-ch.AppropriateFriction);
			ch.vuc.y *= (1f-ch.AirFriction);
		}
	}
	
	protected override void RepeatActions()
	{
		if(ch.Grounded) ch.RestoreOptionsOnGroundTouch();
	}
	
	protected override void OnStart()
	{
		touchedWall = false;
		movement = ch.InputVector*ch.DirectionalAirDodgeSpeed;
		ch.Fastfalling = false;
		ch.ResetVelocity();
		ch.vuc = movement;
		CheckWavedashOption();
	}
	
	protected virtual void CheckWavedashOption()
	{
		if(ch.Grounded && !IsInEndlag && movement.y >= 0)
		{
			if(!IFramesStarted) OnIFramesStart();
			ch.IFrames.Remove(ActionName);
			ch.vuc.y = 0;
			ch.vec.y = VCF;
			ch.States.Change("Wavedash");
		}
	}
	
	protected override void DecideNextState()
	{
		ch.TurnConditional();
		
		if(touchedWall) ch.RestoreOptionsOnWallTouch();
		
		if(ch.Grounded) ch.States.Change("Wavedash");
		else if(ch.Walled && ch.Resources.Has("Clings"))
		{
			ch.ApplySettings("Wall");
			ch.States.Change("Wall");
		}
		else ch.States.Change("Air");
	}
}
