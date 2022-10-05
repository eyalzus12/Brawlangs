using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class State
{
	[Signal]
	public delegate void StateEnd(State s);
	
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
			if(ch.InputtingAttack("Light")) LightAttack();
			if(this != ch.States.Current) return;
			if(ch.InputtingAttack("Special")) SpecialAttack();
			if(this != ch.States.Current) return;
			if(ch.InputtingAttack("Taunt")) Taunt();
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
		ch.TurnConditional();
		ch.ExecuteAttack(ch.AttackInputDir(baseInput) + attackType);
		Character.INPUT_DIRS.ForEach(s => MarkForDeletion(s + baseInput, true));
	}
	
	protected void SetHorizontalAlternatingInputs()
	{
		if(Inputs.IsActionJustPressed("Left"))
		{
			if(ch.RightHeld && !ch.LeftHeld)
				ch.RightHeld = false;
			ch.LeftHeld = true;
		}
		
		if(Inputs.IsActionJustReleased("Left"))
		{
			ch.LeftHeld = false;
			if(Inputs.IsActionPressed("Right"))
				ch.RightHeld = true;
		}
		
		if(Inputs.IsActionPressed("Left") && !ch.LeftHeld && !ch.RightHeld)
			ch.LeftHeld = true;
		
		if(!Inputs.IsActionPressed("Left") && ch.LeftHeld)
			ch.LeftHeld = false;
		
		if(Inputs.IsActionJustPressed("Right"))
		{
			if(ch.LeftHeld && !ch.RightHeld) 
				ch.LeftHeld = false;
			ch.RightHeld = true;
		}
		
		if(Inputs.IsActionJustReleased("Right"))
		{
			ch.RightHeld = false;
			if(Inputs.IsActionPressed("Left")) 
				ch.LeftHeld = true;
		}
		
		if(Inputs.IsActionPressed("Right") && !ch.LeftHeld && !ch.RightHeld)
			ch.RightHeld = true;
		
		if(!Inputs.IsActionPressed("Right") && ch.RightHeld)
			ch.RightHeld = false;
	}
	
	protected void SetVerticalAlternatingInputs()
	{
		if(Inputs.IsActionJustPressed("Up"))
		{
			if(ch.DownHeld && !ch.UpHeld)
				ch.DownHeld = false;
			ch.UpHeld = true;
		}
		
		if(Inputs.IsActionJustReleased("Up"))
		{
			ch.UpHeld = false;
			if(Inputs.IsActionPressed("Down"))
				ch.DownHeld = true;
		}
		
		if(Inputs.IsActionPressed("Up") && !ch.UpHeld && !ch.DownHeld)
			ch.UpHeld = true;
		
		if(!Inputs.IsActionPressed("Up") && ch.UpHeld)
			ch.UpHeld = false;
		
		if(Inputs.IsActionJustPressed("Down"))
		{
			if(ch.UpHeld && !ch.DownHeld) 
				ch.UpHeld = false;
			ch.DownHeld = true;
		}
		
		if(Inputs.IsActionJustReleased("Down"))
		{
			ch.DownHeld = false;
			if(Inputs.IsActionPressed("Up")) 
				ch.UpHeld = true;
		}
		
		if(Inputs.IsActionPressed("Down") && !ch.UpHeld && !ch.DownHeld)
			ch.DownHeld = true;
		
		if(!Inputs.IsActionPressed("Down") && ch.DownHeld)
			ch.DownHeld = false;
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
	
	protected void SetDownHoldingInput() => ch.DownHeld = Inputs.IsActionPressed("Down");
	protected void SetUpHoldingInput() => ch.UpHeld = Inputs.IsActionPressed("Up");
	
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
