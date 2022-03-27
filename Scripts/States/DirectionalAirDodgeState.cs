using Godot;
using System;

public class DirectionalAirDodgeState : GenericInvincibleState
{
	public DirectionalAirDodgeState() : base() {}
	public DirectionalAirDodgeState(Character link) : base(link) {}
	
	public bool touchedWall = false;
	public bool touchedGround = false;
	public Vector2 movement;
	
	public override bool IsActionable() => touchedGround;
	
	public override int Startup() => ch.directionalAirDodgeStartup;
	public override int InvincibilityLength() => ch.directionalAirDodgeLength;
	public override int Endlag() => ch.directionalAirDodgeEndlag;
	public override int Cooldown() => touchedGround?ch.groundTouchDodgeCooldownThreshold:ch.directionalAirDodgeCooldown;
	public override string ActionName() => "Dodge";
	public override string Animation() => "DirectionAirDodge";
	
	public override void Init()
	{
		base.Init();
		touchedWall = false;
		touchedGround = false;
		movement = ch.GetInputVector()*ch.directionalAirDodgeSpeed;
		ch.fastfalling = false;
	}
	
	protected override void LoopActions()
	{
		base.LoopActions();
		
		ch.vec.x *= (1f-ch.AppropriateFriction);
		ch.vec.y *= (1f-ch.airFriction);
		
		if(ch.walled && ch.currentClingsUsed < ch.maxClingsAllowed) touchedWall = true;
		if(ch.grounded)
		{
			ch.vec.y = VCF;
			touchedGround = true;
		}
	}
	
	protected override void OnIFramesStart()
	{
		ch.vec = movement;
	}
	
	protected override void DoJump()
	{
		if(!actionable) return;
		ch.ChangeState("Jump");
		MarkForDeletion("player_jump", true);
	}
	
	protected override void LightAttack()
	{
		if(ch.upHeld) ch.ExecuteAttack("NLight");
		else if(ch.downHeld) ch.ExecuteAttack("DLight");
		else if(ch.rightHeld || ch.leftHeld) ch.ExecuteAttack("SLight");
		else ch.ExecuteAttack("NLight");
		
		MarkForDeletion("player_light_attack", true);
	}
	
	protected override void HeavyAttack()
	{
		if(ch.upHeld) ch.ExecuteAttack("NStrong");
		else if(ch.downHeld) ch.ExecuteAttack("DStrong");
		else if(ch.rightHeld || ch.leftHeld) ch.ExecuteAttack("SStrong");
		else ch.ExecuteAttack("NStrong");
		
		MarkForDeletion("player_heavy_attack", true);
	}
	
	protected override void SpecialAttack()
	{
		if(ch.upHeld) ch.ExecuteAttack("USpecial");
		else if(ch.downHeld) ch.ExecuteAttack("DSpecial");
		else if(ch.rightHeld || ch.leftHeld) ch.ExecuteAttack("SSpecial");
		else ch.ExecuteAttack("NSpecial");
		
		MarkForDeletion("player_special_attack", true);
	}
	
	protected override void Taunt()
	{
		if(ch.upHeld) ch.ExecuteAttack("UTaunt");
		else if(ch.downHeld) ch.ExecuteAttack("DTaunt");
		else if(ch.rightHeld || ch.leftHeld) ch.ExecuteAttack("STaunt");
		else ch.ExecuteAttack("NTaunt");
		
		MarkForDeletion("player_taunt", true);
	}
	
	protected override void DecideNextState()
	{
		ch.TurnConditional();
		
		if(touchedGround) ch.RestoreOptionsOnGroundTouch();
		if(touchedWall) ch.RestoreOptionsOnWallTouch();
		
		if(ch.grounded)
		{
			if(ch.downHeld)
			{
				if(ch.onSemiSolid)
				{
					ch.SetCollisionMaskBit(DROP_THRU_BIT, false);
					ch.vic.y = VCF;
					ch.ChangeState("Air");
				}
				else
				{
					ch.Crouch();
					ch.ChangeState("Crawl");
				}
			}
			else
			{
				ch.Uncrouch();
				ch.ChangeState("WalkStop");
			}
		}
		else if(ch.walled && ch.currentClingsUsed < ch.maxClingsAllowed)
		{
			ch.ApplySettings("Wall");
			ch.ChangeState("Wall");
		}
		else ch.ChangeState("Air");
	}
	
	public override void OnChange(State newState)
	{
		base.OnChange(newState);
		ch.lastDodgeUsed = "Directional";
	}
}
