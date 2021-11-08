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
	}
	
	protected override void DoMovement()
	{
		if(ch.InputingDirection()) DoInputMovement();
	}
	
	protected void DoInputMovement()
	{
		float sped = ch.direction * ch.groundSpeed * (2-ch.ffric);
		float acc = ch.direction * ch.groundAcceleration * ch.ffric;
		
		ch.vec.x.Lerp(sped, acc);
	}
	
	protected void DoFriction()
	{
		float friction = ch.ffric * 
			(ch.onSlope?ch.slopeFriction:ch.groundFriction);
		
		ch.vec.x -= friction*ch.vec.x;
	}
	
	protected override void DoJump()
	{
		jump = true;
	}
	
	protected override void LightAttack()
	{
		if(jump || !IsActionable() || ch.currentAttack != null) return;
		
		if(ch.upHeld) ch.ExecuteAttack("NLight");
		else if(ch.downHeld) ch.ExecuteAttack("DLight");
		else if(ch.rightHeld || ch.leftHeld) ch.ExecuteAttack("SLight");
		else ch.ExecuteAttack("NLight");
		
		MarkForDeletion("player_light_attack", true);
	}
	
	protected override void HeavyAttack()
	{
		if(jump || !IsActionable() || ch.currentAttack != null) return;
		
		if(ch.upHeld) ch.ExecuteAttack("NStrong");
		else if(ch.downHeld) ch.ExecuteAttack("DStrong");
		else if(ch.rightHeld || ch.leftHeld) ch.ExecuteAttack("SStrong");
		else ch.ExecuteAttack("NStrong");
		
		MarkForDeletion("player_heavy_attack", true);
	}
	
	protected override void SpecialAttack()
	{
		if(jump || !IsActionable() || ch.currentAttack != null) return;
		
		if(ch.upHeld) ch.ExecuteAttack("NSpecial");
		else if(ch.downHeld) ch.ExecuteAttack("DSpecial");
		else if(ch.rightHeld || ch.leftHeld) ch.ExecuteAttack("SSpecial");
		else ch.ExecuteAttack("NSpecial");
		
		MarkForDeletion("player_special_attack", true);
	}
	
	public override void SetInputs()
	{
		SetHorizontalAlternatingInputs();
		SetDownHoldingInput();
		SetUpHoldingInput();
	}
	
	protected override bool CalcStateChange()
	{
		if(ch.currentAttack != null) return true;
		//trick other states into not switching during an attack
		
		if(jump)
			ch.ChangeState("Jump");
		else if(!ch.grounded)
			ch.ChangeState("Air");
		else if(ch.downHeld && !ch.crouching && !ch.onSemiSolid)
			ch.ChangeState("Duck");
		else return false;
		
		return true;
	}
	
	protected override void LoopActions()
	{
		if(ch.onSemiSolid && ch.downHeld)
		{
			ch.SetCollisionMaskBit(DROP_THRU_BIT, false);
			ch.vic.y = VCF;
		}
		else AdjustVelocity();
	}
	
	protected override void AdjustVelocity()
	{
		if(ch.fvel.y > 0)
			ch.vec.y = ch.fvel.y;
		else ch.vec.y = 0;
		
		ch.vec.y += VCF;
		
		snapVector = -VCF * ch.fnorm;
	}
}
