using Godot;
using System;

public class DirectionalAirDodgeState : GenericInvincibleState
{
	public DirectionalAirDodgeState() : base() {}
	public DirectionalAirDodgeState(Character link) : base(link) {}
	
	public bool touchedWall = false;
	public Vector2 movement;
	
	public override bool Actionable => false;
	
	public override int Startup => ch.directionalAirDodgeStartup;
	public override int IFrames => ch.directionalAirDodgeLength;
	public override int Endlag => ch.directionalAirDodgeEndlag;
	public override string ActionName => "Dodge";
	public override string StateAnimation => "DirectionAirDodge";
	
	public override void Init()
	{
		base.Init();
		touchedWall = false;
		movement = ch.InputVector*ch.directionalAirDodgeSpeed;
		ch.fastfalling = false;
		CheckWavedashOption();
	}
	
	protected override void LoopActions()
	{
		base.LoopActions();
		ch.vec.x *= (1f-ch.AppropriateFriction);
		ch.vec.y *= (1f-ch.airFriction);
		if(ch.walled && ch.Resources.Has("Clings")) touchedWall = true;
		CheckWavedashOption();
	}
	
	protected override void RepeatActions()
	{
		ch.SetCollisionMaskBit(DROP_THRU_BIT, !ch.downHeld);
		if(ch.grounded) ch.RestoreOptionsOnGroundTouch();
	}
	
	protected virtual void CheckWavedashOption()
	{
		if(ch.grounded && !IsInEndlag && movement.y >= 0)
		{
			if(!IFramesStarted) OnIFramesStart();
			ch.vec.y = VCF;
			ch.States.Change("Wavedash");
		}
	}
	
	protected override void OnIFramesStart()
	{
		ch.vec = movement;
	}
	
	protected override void DecideNextState()
	{
		ch.TurnConditional();
		
		if(touchedWall) ch.RestoreOptionsOnWallTouch();
		
		if(ch.grounded) ch.States.Change("Wavedash");
		else if(ch.walled && ch.Resources.Has("Clings"))
		{
			ch.ApplySettings("Wall");
			ch.States.Change("Wall");
		}
		else ch.States.Change("Air");
	}
}
