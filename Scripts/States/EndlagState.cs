using Godot;
using System;

public class EndlagState : State
{
	public EndlagState() : base() {}
	public EndlagState(Character link) : base(link) {}
	
	public int endlag = 0;
	public Attack att = null;
	
	public override bool IsActionable() => false;
	
	public override void Init()
	{
		endlag = 0;
		att = null;
		SetupCollisionParamaters();
		ch.SetCollisionMaskBit(DROP_THRU_BIT, !ch.downHeld);
		//ch.PlayAnimation(att.endlagAnimation);
		//figure out a way to set the animation after the attack exists
	}
	
	protected override void DoMovement()
	{
		if(att is null) return;
		var friction = att?.attackFriction ?? 0f;
		ch.vec.x *= 1f-friction*(ch.grounded?ch.ffric:1f);
	}
	
	protected override void FirstFrameAfterInit()
	{
		ch.PlayAnimation(att.GetEndlagAnimation());
	}
	
	protected override void RepeatActions()
	{
		if(Inputs.IsActionReallyJustReleased("player_down"))
			ch.SetCollisionMaskBit(DROP_THRU_BIT, true);
	}
	
	protected override void DoGravity()
	{
		if(!ch.grounded) ch.vec.y.Towards(ch.fallSpeed, ch.gravity);
	}
	
	protected override bool CalcStateChange()
	{
		if(frameCount >= endlag)
		{
			var s = ch.GetState<AttackState>();
			ch.TurnConditional();
			ch.SetCollisionMaskBit(DROP_THRU_BIT, true);
			
			if(ch.grounded)
			{
				if(!s.touchedGround) ch.RestoreOptionsOnGroundTouch();
				if(ch.downHeld)
				{
					ch.Crouch();
					ch.ChangeState(ch.IsIdle()?"Crouch":"Crawl");
				}
				else
				{
					ch.Uncrouch();
					ch.ChangeState(ch.IsIdle()?"Idle":"Walk");
				}
			}
			else if(ch.walled && ch.currentClingsUsed < ch.maxClingsAllowed)
			{
				if(!s.touchedWall) ch.RestoreOptionsOnWallTouch();
				else ch.vec.y *= (1-ch.wallFriction*ch.wfric);
				ch.ApplySettings("Wall");
				ch.ChangeState("Wall");
			}
			else ch.ChangeState("Air");
		}
		else return false;
		
		return true;
	}
}
