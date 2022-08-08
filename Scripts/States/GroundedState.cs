using Godot;
using System;

public class GroundedState : State
{
	public bool jump = false;
	
	public GroundedState() : base() {}
	public GroundedState(Character link) : base(link) {}
	
	public override void ForcedInit()
	{
		base.ForcedInit();
		jump = false;
		ch.fastfalling = false;
	}
	
	protected override void DoMovement()
	{
		if(ch.InputtingHorizontalDirection) DoInputMovement();
	}
	
	protected virtual void DoInputMovement()
	{
		var dir = ch.InputDirection;
		float sped = dir * ch.groundSpeed * (2-ch.ffric);
		float acc = ch.groundAcceleration * ch.ffric;
		
		ch.vec.x.Towards(sped, acc);
	}
	
	protected virtual void DoFriction()
	{
		float friction = ch.ffric * (ch.onSlope?ch.slopeFriction:ch.groundFriction);
		ch.vec.x *= (1-friction);
	}
	
	protected override void DoJump()
	{
		if(!Actionable) return;
		MarkForDeletion("jump", true);
		jump = true;
	}
	
	protected override void DoDodge()
	{
		if(!Actionable || jump || !ch.Resources.Has("Dodge")) return;
		
		if(ch.InputtingHorizontalDirection && !ch.Cooldowns.InCooldown("Dodge"))
		{
			ch.States.Change("DirectionalAirDodge");
			MarkForDeletion("dodge", true);
		}
	}
	
	protected override void LightAttack()
	{
		if(jump) return;
		
		if(ch.upHeld) ch.ExecuteAttack("ULight");
		else if(ch.downHeld) ch.ExecuteAttack("DLight");
		else if(ch.rightHeld || ch.leftHeld) ch.ExecuteAttack("SLight");
		else ch.ExecuteAttack("NLight");
		
		MarkForDeletion("light", true);
	}
	
	protected override void SpecialAttack()
	{
		if(jump) return;
		
		if(ch.upHeld) ch.ExecuteAttack("USpecial");
		else if(ch.downHeld) ch.ExecuteAttack("DSpecial");
		else if(ch.rightHeld || ch.leftHeld) ch.ExecuteAttack("SSpecial");
		else ch.ExecuteAttack("NSpecial");
		
		MarkForDeletion("special", true);
	}
	
	protected override void Taunt()
	{
		if(jump) return;
		
		if(ch.upHeld) ch.ExecuteAttack("UTaunt");
		else if(ch.downHeld) ch.ExecuteAttack("DTaunt");
		else if(ch.rightHeld || ch.leftHeld) ch.ExecuteAttack("STaunt");
		else ch.ExecuteAttack("NTaunt");
		
		MarkForDeletion("taunt", true);
	}
	
	protected override bool CalcStateChange()
	{
		if(jump) ch.States.Change("Jump");
		else if(!ch.grounded) ch.States.Change("Air");
		else if(ch.downHeld && !ch.crouching && !ch.onSemiSolid && ch.vec.y > 0f) ch.States.Change("Duck");
		else return false;
		
		return true;
	}
	
	protected override void LoopActions()
	{
		if(ch.onSemiSolid && ch.downHeld) ch.vic.y = VCF;
		else AdjustVelocity();
		ch.SetCollisionMaskBit(DROP_THRU_BIT, !ch.downHeld);
	}
	
	protected override void AdjustVelocity()
	{
		SetupCollisionParamaters();
		
		if(ch.fvel.y > 0)
			ch.vec.y = ch.fvel.y;
		else ch.vec.y = 0;
		
		ch.vec.y += VCF;
		
		snapVector = -VCF * ch.fnorm;
	}
}
