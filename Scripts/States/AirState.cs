using Godot;
using System;

public class AirState : State
{
	public bool jump = false;
	
	public AirState() : base() {}
	public AirState(Character link): base(link) {}
	
	public override void Init()
	{
		ch.vac = Vector2.Zero;
		Unsnap();
		ch.onSemiSolid = false;
		ch.onSlope = false;
		jump = false;
		if(ch.CharacterSprite.currentSheet.name.StartsWith("Jump"))
			ch.QueueAnimation("Drift");
		else
			ch.PlayAnimation("Drift");
	}
	
	protected override void DoMovement()
	{
		if(ch.InputtingHorizontalDirection) DoInputMovement();
		else DoFriction();
	}
	
	protected virtual void DoInputMovement()
	{
		ch.vec.x.Towards(ch.Direction * ch.airSpeed, ch.airAcceleration);
	}
	
	protected virtual void DoFriction()
	{
		ch.vec.x *= (1f-ch.airFriction);
	}
	
	protected override void DoGravity()
	{
		ch.vec.y.Towards(ch.AppropriateFallingSpeed, ch.AppropriateGravity);
	}
	
	protected override void DoJump()
	{
		if(ch.Resources.Has("AirJumps")) jump = true;
	}
	
	protected override void DoDodge()
	{
		if(!Actionable || !ch.Resources.Has("Dodge") || ch.Cooldowns.InCooldown("Dodge")) return;
		ch.States.Change((ch.InputtingDirection?"Directional":"Spot")+"AirDodge");
		MarkForDeletion("player_dodge", true);
	}
	
	protected override void LightAttack()
	{
		if(ch.upHeld) ch.ExecuteAttack("UAir");
		else if(ch.downHeld) ch.ExecuteAttack("DAir");
		else if(ch.rightHeld || ch.leftHeld) ch.ExecuteAttack("SAir");
		else ch.ExecuteAttack("NAir");
		
		MarkForDeletion("player_light_attack", true);
	}
	
	/*protected override void HeavyAttack()
	{
		if(ch.upHeld) ch.ExecuteAttack("NSlam");
		else if(ch.downHeld) ch.ExecuteAttack("DSlam");
		else if(ch.rightHeld || ch.leftHeld) ch.ExecuteAttack("SSlam");
		else ch.ExecuteAttack("NSlam");
		
		MarkForDeletion("player_heavy_attack", true);
	}*/
	
	protected override void SpecialAttack()
	{
		if(ch.upHeld) ch.ExecuteAttack("AUSpecial");
		else if(ch.downHeld) ch.ExecuteAttack("ADSpecial");
		else if(ch.rightHeld || ch.leftHeld) ch.ExecuteAttack("ASSpecial");
		else ch.ExecuteAttack("ANSpecial");
		
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
	
	public override void SetInputs()
	{
		base.SetInputs();
		SetFastFallInput();
	}
	
	protected override bool CalcStateChange()
	{
		if(jump) ch.States.Change("AirJump");
		else if(ch.walled && ch.Resources.Has("Clings")) ch.States.Change("WallLand");
		else if(ch.grounded)
		{
			if(ch.onSemiSolid && ch.downHeld)
			{
				ch.SetCollisionMaskBit(DROP_THRU_BIT, false);
				ch.vic.y = VCF;
				SetupCollisionParamaters();
			}
			else ch.States.Change("Land");
		}
		else return false;
		
		return true;
	}
	
	protected override void LoopActions()
	{
		if(ch.ceilinged)
		{
			ch.PlayAnimation("Bonk");
			ch.QueueAnimation("Drift");
			
			if(ch.cnorm == Vector2.Down)
				ch.vec.y *= ch.ceilingBonkBounce;
			else if(ch.vec.y < 0)
			{
				ch.vec.x = 0;
				if(ch.cvel.y != 0f)
					ch.vec.y *= ch.ceilingBonkBounce;
			}
		}
	}
	
	protected override void RepeatActions()
	{
		ch.TurnConditional();
		if(ch.crouching) ch.Uncrouch();
		
		ch.SetCollisionMaskBit(DROP_THRU_BIT, !ch.downHeld);
	}
}
