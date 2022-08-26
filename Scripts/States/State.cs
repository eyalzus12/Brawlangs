using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class State : Node
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
	
	public State(Character link)
	{
		ch = link;
	}
	
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
		
		var norm = ch.grounded?ch.fnorm:Vector2.Zero;
		var v = ch.Velocity.TiltToNormal(norm);
		
		var moveRes = ch.
			MoveAndSlideWithSnap(v, snapVector, Vector2.Up,
			false, 4, FLOOR_ANGLE);
		
		ch.grounded = ch.IsOnFloor();
		ch.walled = ch.IsOnWall();
		ch.ceilinged = ch.IsOnCeiling();
		
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
			if(ch.rightHeld && !ch.leftHeld)
				ch.rightHeld = false;
			ch.leftHeld = true;
		}
		
		if(Inputs.IsActionJustReleased("Left"))
		{
			ch.leftHeld = false;
			if(Inputs.IsActionPressed("Right"))
				ch.rightHeld = true;
		}
		
		if(Inputs.IsActionPressed("Left") && !ch.leftHeld && !ch.rightHeld)
			ch.leftHeld = true;
		
		if(!Inputs.IsActionPressed("Left") && ch.leftHeld)
			ch.leftHeld = false;
		
		if(Inputs.IsActionJustPressed("Right"))
		{
			if(ch.leftHeld && !ch.rightHeld) 
				ch.leftHeld = false;
			ch.rightHeld = true;
		}
		
		if(Inputs.IsActionJustReleased("Right"))
		{
			ch.rightHeld = false;
			if(Inputs.IsActionPressed("Left")) 
				ch.leftHeld = true;
		}
		
		if(Inputs.IsActionPressed("Right") && !ch.leftHeld && !ch.rightHeld)
			ch.rightHeld = true;
		
		if(!Inputs.IsActionPressed("Right") && ch.rightHeld)
			ch.rightHeld = false;
	}
	
	protected void SetVerticalAlternatingInputs()
	{
		if(Inputs.IsActionJustPressed("Up"))
		{
			if(ch.downHeld && !ch.upHeld)
				ch.downHeld = false;
			ch.upHeld = true;
		}
		
		if(Inputs.IsActionJustReleased("Up"))
		{
			ch.upHeld = false;
			if(Inputs.IsActionPressed("Down"))
				ch.downHeld = true;
		}
		
		if(Inputs.IsActionPressed("Up") && !ch.upHeld && !ch.downHeld)
			ch.upHeld = true;
		
		if(!Inputs.IsActionPressed("Up") && ch.upHeld)
			ch.upHeld = false;
		
		if(Inputs.IsActionJustPressed("Down"))
		{
			if(ch.upHeld && !ch.downHeld) 
				ch.upHeld = false;
			ch.downHeld = true;
		}
		
		if(Inputs.IsActionJustReleased("Down"))
		{
			ch.downHeld = false;
			if(Inputs.IsActionPressed("Up")) 
				ch.upHeld = true;
		}
		
		if(Inputs.IsActionPressed("Down") && !ch.upHeld && !ch.downHeld)
			ch.downHeld = true;
		
		if(!Inputs.IsActionPressed("Down") && ch.downHeld)
			ch.downHeld = false;
	}
	
	protected void SetFastFallInput()
	{
		if(ch.InputtingRun && ch.Velocity.y >= ch.fastfallMargin)
			ch.fastfalling = true;
		if(!ch.HoldingRun || ch.Velocity.y < ch.fastfallMargin)
			ch.fastfalling = false;
	}
	
	protected virtual void SetPlatformDropping()
	{
		ch.SetCollisionMaskBit(DROP_THRU_BIT, !ShouldDrop);
	}
	
	protected void SetDownHoldingInput() => ch.downHeld = Inputs.IsActionPressed("Down");
	protected void SetUpHoldingInput() => ch.upHeld = Inputs.IsActionPressed("Up");
	
	public void Unsnap() => snapVector = Vector2.Zero;
	
	public void MarkForDeletion(string action, bool now = false) => Inputs.MarkForDeletion(action, now);
	
	public void SetupCollisionParamaters()
	//does all the needed calculations on collision
	{
		ch.onSemiSolid = false;//reset semi solid detection
		ch.onSlope = false;//reset slope detection
		ch.aerial = true;//assume no collision for the start
		////////////////////////////////////////////////////////////////////////////////////////////////
		//FIX: for some reason, when on a slope multiple collsiions are given. this is the cause for fnorm not properly updating:
		//the (0,-1) normal is also given
		////////////////////////////////////////////////////////////////////////////////////////////////
		foreach(var collision in GetSlideCollisions())
		{
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
				if(ch.walled && !cling) ch.walled = false;//if not clingable, ignore being on a wall
				
				ch.wnorm = norm;//get wall normal
				ch.wvel = vel;//get wall velocity
				ch.wfric = fric;//get wall friction
				ch.wbounce = bounce;//get wall bounce
			}
			else if(norm.Angle() > Vector2.Right.Angle()
				 && norm.Angle() < Vector2.Left.Angle())
			//Ceiling if steeper than a straight wall
			{
				ch.cnorm = norm;//get ceiling normal
				ch.cvel = vel;//get ceiling velocity
				ch.cfric = fric;//get ceiling friction
				ch.cbounce = bounce;//get ceiling bounce
			}
			else
			//Floor if neither of those
			{
				ch.fnorm = norm;//get floor normal
				ch.fvel = vel;//get floor velocity
				ch.ffric = fric;//get floor friction
				ch.fbounce = bounce;//get floor bounce
				if(!ch.onSlope && norm != Vector2.Up) ch.onSlope = true;//get if you're on a slope, based on if the floor is straight
				
				////////////////////////////////////////////////////////////////////////////////////////////////
				//check if we can fall through the platform
				////////////////////////////////////////////////////////////////////////////////////////////////
				
				if(ch.onSemiSolid) continue;//character IS on semi solid, so skip checking for this collision body
				
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
				
				ch.onSemiSolid = fallthrough??false;//set on semi solid
			}
		}
		
		if(ch.grounded || ch.walled || ch.ceilinged) ch.aerial = false;
		//collision exists, so character isnt aerial
		//the check ensures that non clingable walls wont mark the character as non aerial
	}
	
	private IEnumerable<KinematicCollision2D> GetSlideCollisions()
	{
		for(int i = 0; i < ch.GetSlideCount(); ++i) yield return ch.GetSlideCollision(i);
	}
	
	public string VerySecretMethod() => base.ToString();
}
