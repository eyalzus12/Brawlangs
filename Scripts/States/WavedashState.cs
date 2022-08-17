using Godot;
using System;

public class WavedashState : GroundedState
{
	public override string LightAttackType => "Light";
	public override string SpecialAttackType => "Special";
	public override string TauntType => "Taunt";
	
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
		if(Math.Sign(ch.vuc.x) != ch.Direction && ch.InputDirection == ch.Direction)
		{
			ch.vuc.x.Towards(0, ch.groundAcceleration);
		}
		else
		{
			ch.vuc.x *= (1f-ch.AppropriateFriction);
		}
	}
	
	protected override void DoMovement() {}
	protected override void DoFriction() {}
	
	protected override void DoJump()
	{
		ch.TurnConditional();
		ch.States.Change("Jump");
		MarkForDeletion("Jump", true);
	}
	
	protected override void LightAttack()
	{
		ch.TurnConditional();
		base.LightAttack();
	}
	
	protected override void SpecialAttack()
	{
		ch.TurnConditional();
		base.SpecialAttack();
	}
	
	protected override void Taunt()
	{
		ch.TurnConditional();
		base.Taunt();
	}
	
	protected override bool CalcStateChange()
	{
		if(jump) ch.States.Change("Jump");
		else if(!ch.grounded) ch.States.Change("Air");
		else if(ch.Idle) ch.States.Change("Idle");
		else if(Math.Abs(ch.vuc.x) < ch.groundSpeed)
		{
			if(ch.InputtingHorizontalDirection) ch.States.Change("Walk");
			else ch.States.Change("WalkStop");
		}
		else return false;
			
		return true;
	}
}
