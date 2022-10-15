using Godot;
using System;

public class WallLandState : WallState
{
	public WallLandState() : base() {}
	public WallLandState(Character link) : base(link) {}
	
	public override bool Actionable => false;
	
	public override void Init()
	{
		ch.WallClinging = true;
		if(ch.Velocity.x != 0) ch.Direction = Math.Sign(ch.Velocity.x);
		Unsnap();
		ch.vac = Vector2.Zero;
		SetupCollisionParamaters();
		
		ch.vec.y *= (1-ch.WallFriction*ch.WFric);
		
		ch.vuc.x = 0;
		ch.vuc.y *= (1-ch.WallFriction*ch.WFric);
		
		//ch.onSemiSolid = false;
		ch.PlayAnimation("WallLand", true);
		ch.QueueAnimation("Wall", false, false);
		ch.ApplySettings("Wall");
		
		ch.RestoreOptionsOnWallTouch();
	}
	
	protected override bool CalcStateChange()
	{
		if(!ch.Walled)
		{
			ch.States.Change("Air");
			ch.ApplySettings("Default");
			ch.CharacterSprite.Stop();
		}
		else if(frameCount >= ch.WallLand) ch.States.Change("Wall");
		else return false;
		
		return true;
	}
	
	protected override void DoJump() {}
	
	protected override void RepeatActions()
	{
		base.RepeatActions();
		AdjustVelocity();
	}
}
