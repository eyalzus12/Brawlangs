using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class State
{
	public virtual bool Actionable => true;
	public virtual string LightAttackType => "";
	public virtual string SpecialAttackType => "";
	public virtual string TauntType => "";
	public virtual bool ShouldDrop => false;
		
	public const int DROP_THRU_BIT = 1;
	public const float HCF = 20f;//check force
	public const float VCF = 20f;//check force
	public const float FLOOR_ANGLE = (float)(Math.PI/2f - 0.1);
	
	public InputManager Inputs
	{
		get => ch.Inputs;
		set => ch.Inputs = value;
	}
	
	public int frameCount = 0;
	
	protected Character ch;
	protected Vector2 snapVector;
	public bool justInit = false;
	
	public State() {}
	public State(Character link) {ch = link;}
	
	public override string ToString() => GetType().Name.Replace("State", "");
	
	public virtual void DoPhysics(float delta)
	{
		if(this != ch.States.Current) return;
		
		if(!justInit) LoopActions();
		else justInit = false;
		
		if(frameCount == 0) FirstFrameAfterInit();
		
		RepeatActions();
		SetPlatformDropping();
		DoMovement();
		DoGravity();
		
		if(ch.InputtingJump) DoJump();
		if(this != ch.States.Current) return;
		if(ch.InputtingDodge) DoDodge();
		if(this != ch.States.Current) return;
		
		if(this == ch.States.Current && Actionable && ch.CurrentAttack is null)
		{
			if(ch.InputtingLight) LightAttack();
			if(this != ch.States.Current) return;
			if(ch.InputtingSpecial) SpecialAttack();
			if(this != ch.States.Current) return;
			if(ch.InputtingTaunt) Taunt();
			if(this != ch.States.Current) return;
		}
		
		var norm = ch.Grounded?ch.FNorm:Vector2.Zero;
		var v = ch.Velocity.TiltToNormal(norm);
		
		var moveRes = ch.
			MoveAndSlideWithSnap(v, snapVector, Vector2.Up,
			false, 4, FLOOR_ANGLE);
		
		SetupCollisionParamaters();
		CalcStateChange();
		
		ch.vic = Vector2.Zero;
		
		++frameCount;
	}
	
	public virtual void Init() => SetupCollisionParamaters();
	
	public virtual void ForcedInit()
	{
		Init();
		justInit = true;
		frameCount = 0;
	}
	
	protected virtual void DoMovement() {}
	protected virtual void DoGravity() {}
	
	protected virtual void DoJump() {}
	protected virtual void DoDodge() {}
	
	protected virtual void AdjustVelocity() {}
	
	public virtual void SetInputs()
	{
		SetHorizontalAlternatingInputs();
		SetVerticalAlternatingInputs();
	}
	
	protected virtual bool CalcStateChange() => false;
	protected virtual void FirstFrameAfterInit() {}
	protected virtual void LoopActions() {}
	protected virtual void RepeatActions() {}
	public virtual void OnChange(State newState) {}
	
	protected virtual void LightAttack() => HandleAttack("Light", LightAttackType);
	protected virtual void SpecialAttack() => HandleAttack("Special", SpecialAttackType);
	protected virtual void Taunt() => HandleAttack("Taunt", TauntType);
	
	protected void HandleAttack(string baseInput, string attackType)
	{
		if(attackType == "") return;
		
		if(attackType == "Taunt")
		{
			ch.ExecuteAttack("Taunt");
			return;
		}
		
		var dirInput = ch.AttackInputDir(baseInput);
		if(dirInput == "L" || dirInput == "R") dirInput = "S";
		ch.TurnConditional();
		
		ch.ExecuteAttack(dirInput + attackType);
		
		Character.INPUT_DIRS.ForEach(s => MarkForDeletion(s + baseInput, true));
	}
	
	protected void SetHorizontalAlternatingInputs()
	{
		if(ch.LeftPressed)
		{
			if(ch.RightInput && !ch.LeftInput)
				ch.RightInput = false;
			ch.LeftInput = true;
		}
		
		if(ch.LeftReleased)
		{
			ch.LeftInput = false;
			if(ch.RightHeld) ch.RightInput = true;
		}
		
		if(ch.LeftHeld && !ch.LeftInput && !ch.RightInput)
			ch.LeftInput = true;
		
		if(!ch.LeftHeld && ch.LeftInput)
			ch.LeftInput = false;
		
		if(ch.RightPressed)
		{
			if(ch.LeftInput && !ch.RightInput) 
				ch.LeftInput = false;
			ch.RightInput = true;
		}
		
		if(ch.RightReleased)
		{
			ch.RightInput = false;
			if(ch.LeftHeld) ch.LeftInput = true;
		}
		
		if(ch.RightHeld && !ch.LeftInput && !ch.RightInput)
			ch.RightInput = true;
		
		if(!ch.RightHeld && ch.RightInput)
			ch.RightInput = false;
	}
	
	protected void SetVerticalAlternatingInputs()
	{
		if(ch.UpPressed)
		{
			if(ch.DownInput && !ch.UpInput)
				ch.DownInput = false;
			ch.UpInput = true;
		}
		
		if(ch.UpReleased)
		{
			ch.UpInput = false;
			if(ch.DownHeld) ch.DownInput = true;
		}
		
		if(ch.UpHeld && !ch.UpInput && !ch.DownInput)
			ch.UpInput = true;
		
		if(!ch.UpHeld && ch.UpInput)
			ch.UpInput = false;
		
		if(ch.DownPressed)
		{
			if(ch.UpInput && !ch.DownInput) 
				ch.UpInput = false;
			ch.DownInput = true;
		}
		
		if(ch.DownReleased)
		{
			ch.DownInput = false;
			if(ch.UpHeld) ch.UpInput = true;
		}
		
		if(ch.DownHeld && !ch.UpInput && !ch.DownInput)
			ch.DownInput = true;
		
		if(!ch.DownHeld && ch.DownInput)
			ch.DownInput = false;
	}
	
	protected void SetFastFallInput()
	{
		if(ch.InputtingRun && ch.Velocity.y >= ch.FastfallMargin)
			ch.Fastfalling = true;
		if(!ch.HoldingRun || ch.Velocity.y < ch.FastfallMargin)
			ch.Fastfalling = false;
	}
	
	protected virtual void SetPlatformDropping()
	{
		ch.SetCollisionMaskBit(DROP_THRU_BIT, !ShouldDrop);
	}
	
	protected void SetDownHoldingInput() => ch.DownInput = ch.DownHeld;
	protected void SetUpHoldingInput() => ch.UpInput = ch.UpHeld;
	
	public void Unsnap() => snapVector = Vector2.Zero;
	
	public void MarkForDeletion(string action, bool now = false) => Inputs.MarkForDeletion(action, now);
	
	public void SetupCollisionParamaters()
	//does all the needed calculations on collision
	{
		ch.Grounded = ch.IsOnFloor();
		ch.Walled = ch.IsOnWall();
		ch.Ceilinged = ch.IsOnCeiling();
		
		ch.OnSemiSolid = false;//reset semi solid detection
		ch.OnSlope = false;//reset slope detection
		ch.Aerial = true;//assume no collision for the start
		////////////////////////////////////////////////////////////////////////////////////////////////
		//FIX: for some reason, when on a slope multiple collsiions are given. this is the cause for fnorm not properly updating:
		//the (0,-1) normal is also given
		////////////////////////////////////////////////////////////////////////////////////////////////
		for(int i = 0; i < ch.GetSlideCount(); ++i)
		{
			var collision = ch.GetSlideCollision(i);
			var body = collision.Collider;//get collided body
			if(body is null || !Godot.Object.IsInstanceValid(body)) continue;//ensure the collided body actually exists
			
			////////////////////////////////////////////////////////////////////////////////////////////////
			//get paramaters of the platform, like friction, speed, etc
			////////////////////////////////////////////////////////////////////////////////////////////////
			
			var vel = collision.ColliderVelocity;//get collision velocity
			var norm = collision.Normal;//get the collision normal (angle)
			var fric = body.GetProp<float>("PlatformFriction", 1f);//get friction
			var bounce = body.GetProp<float>("PlatformBounce", 0f);//get bounce
			
			////////////////////////////////////////////////////////////////////////////////////////////////
			//now we set the paramaters related to the type of collision itself (ground, wall, ceiling, etc)
			////////////////////////////////////////////////////////////////////////////////////////////////
			
			if(norm == Vector2.Right || norm == Vector2.Left)
			//Wall if straight right or left
			{
				var cling = body.GetProp<bool>("Clingable", true);//get if clingable
				if(ch.Walled && !cling) ch.Walled = false;//if not clingable, ignore being on a wall
				
				ch.WNorm = norm;//get wall normal
				ch.WVel = vel;//get wall velocity
				ch.WFric = fric;//get wall friction
				ch.WBounce = bounce;//get wall bounce
			}
			else if(norm.Angle() > Vector2.Right.Angle()
				 && norm.Angle() < Vector2.Left.Angle())
			//Ceiling if steeper than a straight wall
			{
				ch.CNorm = norm;//get ceiling normal
				ch.CVel = vel;//get ceiling velocity
				ch.CFric = fric;//get ceiling friction
				ch.CBounce = bounce;//get ceiling bounce
			}
			else
			//Floor if neither of those
			{
				ch.FNorm = norm;//get floor normal
				ch.FVel = vel;//get floor velocity
				ch.FFric = fric;//get floor friction
				ch.FBounce = bounce;//get floor bounce
				if(!ch.OnSlope && norm != Vector2.Up) ch.OnSlope = true;//get if you're on a slope, based on if the floor is straight
				
				////////////////////////////////////////////////////////////////////////////////////////////////
				//check if we can fall through the platform
				////////////////////////////////////////////////////////////////////////////////////////////////
				
				if(ch.OnSemiSolid) continue;//character IS on semi solid, so skip checking for this collision body
				
				var fallthrough = body.GetProp<bool?>("FallThroughPlatform", null);//get if can fall through the platform
				
				//the body doesn't have a FallThroughPlatform property, so go the long route and check if it has one way collision
				if(fallthrough is null && body is CollisionObject2D col)
				{
					//go over the shape owners and check for each if it has one way collision enabled
					foreach(var sh in col.GetShapeOwners())//check each collision shape owners
					{
						uint shid = Convert.ToUInt32(sh);//convert int to uint
						if(col.IsShapeOwnerOneWayCollisionEnabled(shid))//name goes brrr
						{
							fallthrough = true;//set that shape is one way
							break;//finished checking
						}
					}
				}
				
				ch.OnSemiSolid = fallthrough??false;//set on semi solid
			}
		}
		
		if(ch.Grounded || ch.Walled || ch.Ceilinged) ch.Aerial = false;
		//collision exists, so character isnt aerial
		//the check ensures that non clingable walls wont mark the character as non aerial
	}
	
	public string VerySecretMethod() => base.ToString();
}
