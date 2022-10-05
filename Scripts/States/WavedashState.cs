using Godot;
using System;

public class WavedashState : GroundedState
{
	public WavedashState() : base() {}
	public WavedashState(Character link) : base(link) {}
	
	public override void Init()
	{
		ch.vec.y = VCF;
		ch.SetCollisionMaskBit(DROP_THRU_BIT, true);
		ch.RestoreOptionsOnGroundTouch();
		AdjustVelocity();
		ch.PlayAnimation("WavedashStart", true);
		ch.QueueAnimation("Wavewdash", false, false);
		
		ch.vuc.x *= ch.WavedashVelocityMutliplier;
	}
	
	protected override void LoopActions()
	{
		//koopabackdashwaveslidehoverwalkmoonlanding
		if(Math.Sign(ch.vuc.x) != ch.Direction && ch.InputDirection == ch.Direction)
		{
			ch.vuc.x.Towards(0, ch.WavedashFrictionMultiplier*ch.GroundAcceleration);
		}
		else
		{
			ch.vuc.x *= (1f-ch.WavedashFrictionMultiplier*ch.AppropriateFriction);
		}
	}
	
	protected override void DoMovement() {}
	protected override void DoFriction() {}
	
	protected override bool CalcStateChange()
	{
		if(jump) ch.States.Change("Jump");
		else if(!ch.Grounded) ch.States.Change("Air");
		else if(ch.Idle) ch.States.Change("Idle");
		else if(Math.Abs(ch.vuc.x) < ch.GroundSpeed)
		{
			if(ch.InputtingHorizontalDirection) ch.States.Change("Walk");
			else ch.States.Change("WalkStop");
		}
		else return false;
			
		return true;
	}
}
