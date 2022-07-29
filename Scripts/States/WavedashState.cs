using Godot;
using System;

public class WavedashState : GroundedState
{
	public WavedashState() : base() {}
	public WavedashState(Character link) : base(link) {}
	
	public override bool IsActionable() => true;
	
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
		if(Math.Sign(ch.vec.x) != ch.direction && ch.GetInputDirection() == ch.direction)//koopabackdashwaveslidehoverwalkmoonlanding
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
		ch.ChangeState("Jump");
		MarkForDeletion("player_jump", true);
	}
	
	protected override void LightAttack()
	{
		ch.TurnConditional();
		if(ch.upHeld) ch.ExecuteAttack("ULight");
		else if(ch.downHeld) ch.ExecuteAttack("DLight");
		else if(ch.rightHeld || ch.leftHeld) ch.ExecuteAttack("SLight");
		else ch.ExecuteAttack("NLight");
		
		MarkForDeletion("player_light_attack", true);
	}
	
	protected override void SpecialAttack()
	{
		ch.TurnConditional();
		if(ch.upHeld) ch.ExecuteAttack("USpecial");
		else if(ch.downHeld) ch.ExecuteAttack("DSpecial");
		else if(ch.rightHeld || ch.leftHeld) ch.ExecuteAttack("SSpecial");
		else ch.ExecuteAttack("NSpecial");
		
		MarkForDeletion("player_special_attack", true);
	}
	
	protected override void Taunt()
	{
		ch.TurnConditional();
		if(ch.upHeld) ch.ExecuteAttack("UTaunt");
		else if(ch.downHeld) ch.ExecuteAttack("DTaunt");
		else if(ch.rightHeld || ch.leftHeld) ch.ExecuteAttack("STaunt");
		else ch.ExecuteAttack("NTaunt");
		
		MarkForDeletion("player_taunt", true);
	}
	
	protected override bool CalcStateChange()
	{
		if(base.CalcStateChange()) return true;
		else if(ch.IsIdle) ch.ChangeState<IdleState>();
		else if(Math.Abs(ch.vec.x) < ch.groundSpeed)
		{
			if(ch.InputtingHorizontalDirection) ch.ChangeState<WalkState>();
			else ch.ChangeState<WalkStopState>();
		}
		else return false;
			
		return true;
	}
}