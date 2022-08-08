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
		ch.PlayAnimation("WavedashStart");
		ch.QueueAnimation("Wavewdash");
	}
	
	protected override void LoopActions()
	{
		//koopabackdashwaveslidehoverwalkmoonlanding
		if(Math.Sign(ch.vec.x) != ch.Direction && ch.InputDirection == ch.Direction)
		{
			ch.vec.x.Towards(0, ch.groundAcceleration);
		}
		else
		{
			ch.vec.x *= (1f-ch.AppropriateFriction);
		}
	}
	
	protected override void DoInputMovement() {}
	protected override void DoFriction() {}
	
	protected override void DoJump()
	{
		ch.TurnConditional();
		ch.States.Change("Jump");
		MarkForDeletion("jump", true);
	}
	
	protected override void LightAttack()
	{
		ch.TurnConditional();
		if(ch.upHeld) ch.ExecuteAttack("ULight");
		else if(ch.downHeld) ch.ExecuteAttack("DLight");
		else if(ch.rightHeld || ch.leftHeld) ch.ExecuteAttack("SLight");
		else ch.ExecuteAttack("NLight");
		
		MarkForDeletion("light", true);
	}
	
	protected override void SpecialAttack()
	{
		ch.TurnConditional();
		if(ch.upHeld) ch.ExecuteAttack("USpecial");
		else if(ch.downHeld) ch.ExecuteAttack("DSpecial");
		else if(ch.rightHeld || ch.leftHeld) ch.ExecuteAttack("SSpecial");
		else ch.ExecuteAttack("NSpecial");
		
		MarkForDeletion("special", true);
	}
	
	protected override void Taunt()
	{
		ch.TurnConditional();
		if(ch.upHeld) ch.ExecuteAttack("UTaunt");
		else if(ch.downHeld) ch.ExecuteAttack("DTaunt");
		else if(ch.rightHeld || ch.leftHeld) ch.ExecuteAttack("STaunt");
		else ch.ExecuteAttack("NTaunt");
		
		MarkForDeletion("taunt", true);
	}
	
	protected override bool CalcStateChange()
	{
		if(base.CalcStateChange()) return true;
		else if(ch.Idle) ch.States.Change("Idle");
		else if(Math.Abs(ch.vec.x) < ch.groundSpeed)
		{
			if(ch.InputtingHorizontalDirection) ch.States.Change("Walk");
			else ch.States.Change("WalkStop");
		}
		else return false;
			
		return true;
	}
}
