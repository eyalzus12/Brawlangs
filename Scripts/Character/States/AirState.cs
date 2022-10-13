using Godot;
using System;

public class AirState : State
{
	public override string LightAttackType => "Air";
	public override string SpecialAttackType => "ASpecial";
	public override bool ShouldDrop => ch.DownInput && ch.HoldingRun;
	
	public bool jump = false;
	public bool wallTouch = false;
	
	public AirState() : base() {}
	public AirState(Character link): base(link) {}
	
	public override void Init()
	{
		ch.vac = Vector2.Zero;
		Unsnap();
		ch.OnSemiSolid = false;
		ch.OnSlope = false;
		jump = false;
		wallTouch = false;
		
		ch.QueueAnimation("Drift", ch.AnimationLooping, false);
		
		if(ch.Tags["Grounded"] == StateTag.Starting || ch.Tags["Grounded"] == StateTag.Active)
			ch.Tags["Grounded"] = StateTag.Ending;
		
		if(ch.Tags["Walled"] == StateTag.Starting || ch.Tags["Walled"] == StateTag.Active)
			ch.Tags["Walled"] = StateTag.Ending;
	}
	
	protected override void DoMovement()
	{
		if(ch.InputtingHorizontalDirection) DoInputMovement();
		else DoFriction();
		ch.vuc.x *= (1-ch.AppropriateFriction);
	}
	
	protected virtual void DoInputMovement()
	{
		ch.vec.x.Towards(ch.MovementDirection * ch.AppropriateSpeed, ch.AppropriateAcceleration);
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
		else if(ch.Walled && ch.Resources.Has("Clings")) ch.States.Change("WallLand");
		else if(ch.Grounded) ch.States.Change("Land");
		else return false;
		
		return true;
	}
	
	protected override void LoopActions()
	{
		if(ch.Ceilinged)
		{
			ch.PlayAnimation("Bonk", true);
			ch.QueueAnimation("Drift", false, false);
			
			if(ch.CNorm == Vector2.Down)
			{
				ch.vec.y *= ch.CeilingBonkBounce;
				ch.vuc.y = 0;
			}
			else if(ch.vec.y < 0)
			{
				ch.vec.x = 0;
				if(ch.CVel.y != 0f)
					ch.vec.y *= ch.CeilingBonkBounce;
			}
		}
		
		if(!ch.Walled) wallTouch = false;
		else if(!wallTouch && !ch.Resources.Has("Clings"))
			wallTouch = true;
	}
	
	protected override void RepeatActions()
	{
		ch.TurnConditional();
		if(ch.Crouching) ch.Uncrouch();
		if(!ch.Resources.Has("Clings")) ch.Walled = false;
		
		//ch.SetCollisionMaskBit(DROP_THRU_BIT, !ch.downHeld);
	}
}
