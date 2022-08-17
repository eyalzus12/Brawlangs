using Godot;
using System;

public class EndlagState : State
{
	public EndlagState() : base() {}
	public EndlagState(Character link) : base(link) {}
	
	public int endlag = 0;
	public Attack att = null;
	
	public override bool Actionable => false;
	public override bool ShouldDrop => ch.downHeld && ch.HoldingRun;
	
	public override void Init()
	{
		endlag = 0;
		att = null;
		SetupCollisionParamaters();
		//ch.PlayAnimation(att.endlagAnimation);
		//figure out a way to set the animation after the attack exists
	}
	
	public override void SetInputs()
	{
		base.SetInputs();
		SetFastFallInput();
	}
	
	protected override void DoMovement()
	{
		if(att is null) return;
		DoFriction();
	}
	
	protected virtual void DoFriction()
	{
		ch.vec.x *= (1f-ch.AppropriateFriction);
		ch.vuc.x *= (1f-ch.AppropriateFriction);
	}
	
	protected override void FirstFrameAfterInit()
	{
		ch.PlayAnimation(att.GetEndlagAnimation());
	}
	
	protected override void DoGravity()
	{
		if(!ch.grounded)
		{
			ch.vec.y.Towards(ch.AppropriateFallingSpeed, ch.AppropriateGravity);
			ch.vuc.y.Towards(0, ch.AppropriateGravity);
		}
	}
	
	protected override bool CalcStateChange()
	{
		if(frameCount >= endlag)
		{
			var s = ch.States.Get<AttackState>();
			ch.TurnConditional();
			
			if(ch.grounded)
			{
				if(!s.touchedGround) ch.RestoreOptionsOnGroundTouch();
				if(ch.downHeld)
				{
					ch.Crouch();
					ch.States.Change(ch.Idle?"Crouch":"Crawl");
				}
				else
				{
					ch.Uncrouch();
					ch.States.Change(ch.Idle?"Idle":"Walk");
				}
			}
			else if(ch.walled && ch.Resources.Has("Clings"))
			{
				if(!s.touchedWall) ch.RestoreOptionsOnWallTouch();
				else ch.vec.y *= (1-ch.wallFriction*ch.wfric);
				ch.ApplySettings("Wall");
				ch.States.Change("Wall");
			}
			else ch.States.Change("Air");
		}
		else return false;
		
		return true;
	}
}
