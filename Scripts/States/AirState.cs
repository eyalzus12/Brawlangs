using Godot;
using System;

public class AirState : State
{
	public override string LightAttackType => "Air";
	public override string SpecialAttackType => "ASpecial";
	public override bool ShouldDrop => ch.downHeld && ch.HoldingRun;
	
	public bool jump = false;
	public bool wallTouch = false;
	
	public AirState() : base() {}
	public AirState(Character link): base(link) {}
	
	public override void Init()
	{
		ch.vac = Vector2.Zero;
		Unsnap();
		ch.onSemiSolid = false;
		ch.onSlope = false;
		jump = false;
		wallTouch = false;
		if(ch.CharacterSprite.currentSheet.name.StartsWith("Jump"))
			ch.QueueAnimation("Drift");
		else
			ch.PlayAnimation("Drift");
	}
	
	protected override void DoMovement()
	{
		if(ch.InputtingHorizontalDirection) DoInputMovement();
		else DoFriction();
		ch.vuc.x *= (1-ch.AppropriateFriction);
	}
	
	protected virtual void DoInputMovement()
	{
		ch.vec.x.Towards(ch.Direction * ch.AppropriateSpeed, ch.AppropriateAcceleration);
	}
	
	protected virtual void DoFriction()
	{
		ch.vec.x *= (1f-ch.AppropriateFriction);
	}
	
	protected override void DoGravity()
	{
		ch.vec.y.Towards(ch.AppropriateFallingSpeed, ch.AppropriateGravity);
		ch.vuc.y.Towards(0, ch.AppropriateGravity);
	}
	
	protected override void DoJump()
	{
		if(!Actionable || !ch.Resources.Has("AirJumps")) return;
		jump = true;
		MarkForDeletion("Jump", true);
	}
	
	protected override void DoDodge()
	{
		if(!Actionable || !ch.Resources.Has("Dodge") || ch.Cooldowns.InCooldown("Dodge")) return;
		ch.States.Change((ch.InputtingNatDodge?"Spot":"Directional") + "AirDodge");
		MarkForDeletion("Dodge", true);
		MarkForDeletion("NDodge", true);
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
		else if(ch.grounded) ch.States.Change("Land");
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
			{
				ch.vec.y *= ch.ceilingBonkBounce;
				ch.vuc.y = 0;
			}
			else if(ch.vec.y < 0)
			{
				ch.vec.x = 0;
				if(ch.cvel.y != 0f)
					ch.vec.y *= ch.ceilingBonkBounce;
			}
		}
		
		if(!ch.walled) wallTouch = false;
		else if(ch.walled && !wallTouch && !ch.Resources.Has("Clings"))
		{
			wallTouch = true;
			ch.vec.y *= (1-ch.wallFriction*ch.wfric);
			ch.vuc.y *= (1-ch.wallFriction*ch.wfric);
		}
	}
	
	protected override void RepeatActions()
	{
		ch.TurnConditional();
		if(ch.crouching) ch.Uncrouch();
		
		//ch.SetCollisionMaskBit(DROP_THRU_BIT, !ch.downHeld);
	}
}
