using Godot;
using System;

public class GroundedState : State
{
	public override string LightAttackType => "Light";
	public override string SpecialAttackType => "Special";
	public override string TauntType => "Taunt";
	public override bool ShouldDrop => ch.DownInput && ch.HoldingRun;
	
	public bool jump = false;
	
	public GroundedState() : base() {}
	public GroundedState(Character link) : base(link) {}
	
	public override void ForcedInit()
	{
		base.ForcedInit();
		jump = false;
		ch.Fastfalling = false;
		ch.vuc.y = 0;
	}
	
	protected override void DoMovement()
	{
		if(ch.InputtingHorizontalDirection) DoInputMovement();
		ch.vuc.x *= (1-ch.AppropriateFriction);
	}
	
	protected virtual void DoInputMovement()
	{
		ch.vec.x.Towards(ch.MovementDirection * ch.AppropriateSpeed, ch.AppropriateAcceleration);
	}
	
	protected virtual void DoFriction()
	{
		ch.vec.x *= (1-ch.AppropriateFriction);
		ch.vuc.x *= (1-ch.AppropriateFriction);
	}
	
	protected override void DoJump()
	{
		if(!Actionable) return;
		jump = true;
		if(!ch.ShouldInitiateRun) MarkForDeletion("Jump", true);//prevent deleting jump input if going to run, so it is properly applied
	}
	
	protected override void DoDodge()
	{
		if(!Actionable || jump) return;
		
		if(ch.InputtingNatDodge)
		{
			if(ch.Cooldowns.InCooldown("Dodge")) return;
			ch.States.Change("SpotGroundedDodge");
		}
		
		MarkForDeletion("Dodge", true);
		MarkForDeletion("NDodge", true);
	}
	
	protected override void LightAttack()
	{
		if(jump) return;
		base.LightAttack();
	}
	
	protected override void SpecialAttack()
	{
		if(jump) return;
		base.SpecialAttack();
	}
	
	protected override void Taunt()
	{
		if(jump) return;
		base.Taunt();
	}
	
	protected override bool CalcStateChange()
	{
		if(Actionable && ch.ShouldInitiateRun)
		{
			ch.TurnConditional();
			ch.States.Change("RunStartup");
		}
		else if(jump) ch.States.Change("Jump");
		else if(!ch.Grounded) ch.States.Change("Air");
		else if(ch.DownInput && !ch.Crouching && ch.vec.y > 0f) ch.States.Change("Duck");
		else return false;
		
		return true;
	}
	
	protected override void LoopActions()
	{
		AdjustVelocity();
	}
	
	protected override void AdjustVelocity()
	{
		SetupCollisionParamaters();
		ch.vec.y = Math.Max(ch.FVel.y, 0) + VCF;
		snapVector = -VCF * ch.FNorm;
	}
}
