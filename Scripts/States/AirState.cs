using Godot;
using System;

public class AirState : State
{
	bool platformCancel = false;//currently disabled. dont pay attention
	
	public AirState() : base() {}
	public AirState(Character link): base(link) {}
	
	public override void Init()
	{
		ch.vac = Vector2.Zero;
		Unsnap();
		ch.onSemiSolid = false;
		ch.onSlope = false;
		var currentAnimation = ch.sprite.currentSheet.name;
		if(currentAnimation == "Jump" || currentAnimation == "WallJump")
			ch.QueueAnimation("Drift");
		else
			ch.PlayAnimation("Drift");
	}
	
	protected override void DoMovement()
	{
		if(ch.InputingDirection()) DoInputMovement();
		else DoFriction();
	}
	
	protected virtual void DoInputMovement()
	{
		ch.vec.x.Lerp(ch.direction * ch.airSpeed, ch.direction * ch.airAcceleration);
	}
	
	protected virtual void DoFriction()
	{
		//ch.vec.x -= ch.vec.x * ch.airFriction;
		ch.vec.x *= (1f-ch.airFriction);
	}
	
	protected override void DoGravity()
	{
		if(ch.fastfalling) DoFastFall();
		else DoNormalGravity();
	}
	
	protected override void DoJump()
	{
//		platformCancel = !ch.GetCollisionMaskBit(DROP_THRU_BIT);
		if(platformCancel) return;
		
		if(ch.jumpCounter < ch.jumpNum)
		{
			MarkForDeletion("player_jump", true);
			ch.vec.y = -ch.doubleJumpHeight;
			ch.jumpCounter++;
			ch.fastfalling = false;
			ch.PlayAnimation("Jump");
			ch.QueueAnimation("Drift");
		}
	}
	
	protected override void LightAttack()
	{
		if(ch.upHeld) ch.ExecuteAttack("NAir");
		else if(ch.downHeld) ch.ExecuteAttack("DAir");
		else if(ch.rightHeld || ch.leftHeld) ch.ExecuteAttack("SAir");
		else ch.ExecuteAttack("NAir");
		
		MarkForDeletion("player_light_attack", true);
	}
	
	protected override void HeavyAttack()
	{
		if(ch.upHeld) ch.ExecuteAttack("NSlam");
		else if(ch.downHeld) ch.ExecuteAttack("DSlam");
		else if(ch.rightHeld || ch.leftHeld) ch.ExecuteAttack("SSlam");
		else ch.ExecuteAttack("NSlam");
		
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
	
	public override void SetInputs()
	{
		SetHorizontalAlternatingInputs();
		SetDownHoldingInput();
		SetFastFallInput();
		SetUpHoldingInput();
	}
	
	protected override bool CalcStateChange()
	{
		if(platformCancel)//not active
		{
			var move = new Vector2(0f, -1000f);
			ch.MoveAndCollide(move);
			move *= -1;
			ch.SetCollisionMaskBit(DROP_THRU_BIT, true);
			GD.Print(ch.MoveAndCollide(move));
		}
		
		if(ch.walled && ch.wallJumpCounter < ch.wallJumpNum) ch.ChangeState("WallLand");
		else if(ch.grounded)
		{
			if(platformCancel) ch.ChangeState("Jump");//not active
			else
			{
				if(ch.onSemiSolid && ch.downHeld)
				{
					ch.SetCollisionMaskBit(DROP_THRU_BIT, false);
					ch.vic.y = VCF;
					SetupCollisionParamaters();
				}
				else ch.ChangeState("Land");
			}
		}
		else return false;
		
		return true;
	}
	
	protected override void LoopActions()
	{
		if(ch.ceilinged)
		{
			if(ch.cnorm == Vector2.Down)
				ch.vec *= ch.bounce;
			else if(ch.vec.y < 0) 
				ch.vec.x = 0;
		}
	}
	
	protected override void RepeatActions()
	{
		ch.TurnConditional();
		if(ch.crouching) ch.Uncrouch();
	}
	
	protected virtual void DoNormalGravity()
	{
		ch.vec.y.Lerp(ch.fallSpeed, ch.gravity);
	}
	
	protected virtual void DoFastFall()
	{
		ch.vec.y.Lerp(ch.fastFallSpeed, ch.fastFallGravity);
	}
}
