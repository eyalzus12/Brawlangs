using Godot;
using System;
using System.Collections.Generic;

public class State : Node
{
	[Signal]
	public delegate void StateEnd(State s);
	
	public bool actionable => IsActionable();
	
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
	
	public virtual bool IsActionable() => true;
	
	public override string ToString() => GetType().Name.Replace("State", "");
	
	public virtual void DoPhysics(float delta)
	{
		if(this != ch.currentState) return;
		
		if(!justInit) LoopActions();
		else justInit = false;
		
		if(frameCount == 0) FirstFrameAfterInit();
		
		frameCount++;
		RepeatActions();
		DoMovement();
		DoGravity();
		
		if(Inputs.IsActionJustPressed("player_jump"))
			DoJump();
		
		if(actionable && ch.currentAttack is null)
		{
			if(Inputs.IsActionJustPressed("player_light_attack"))
				LightAttack();
			else if(Inputs.IsActionJustPressed("player_heavy_attack"))
				HeavyAttack();
			else if(Inputs.IsActionJustPressed("player_special_attack"))
				SpecialAttack();
			else if(Inputs.IsActionJustPressed("player_taunt"))
				Taunt();
		}
		
		if(this != ch.currentState) return;
		
		var norm = ch.grounded?ch.fnorm:Vector2.Zero;
		var v = ch.GetVelocity().TiltToNormal(norm);
		
		var moveRes = ch.
			MoveAndSlideWithSnap(v, snapVector, Vector2.Up,
			false, 4, FLOOR_ANGLE);
		
		ch.grounded = ch.IsOnFloor();
		ch.walled = ch.IsOnWall();
		ch.ceilinged = ch.IsOnCeiling();
		
		SetupCollisionParamaters();
		CalcStateChange();
		
		ch.vic = Vector2.Zero;
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
	protected virtual void LightAttack() {}
	protected virtual void HeavyAttack() {}
	protected virtual void SpecialAttack() {}
	protected virtual void Taunt() {}
	
	protected void SetHorizontalAlternatingInputs()
	{
		if(Inputs.IsActionJustPressed("player_left"))
		{
			if(ch.rightHeld && !ch.leftHeld)
				ch.rightHeld = false;
			ch.leftHeld = true;
		}
		
		if(Inputs.IsActionJustReleased("player_left"))
		{
			ch.leftHeld = false;
			if(Inputs.IsActionPressed("player_right"))
				ch.rightHeld = true;
		}
		
		if(Inputs.IsActionPressed("player_left") && !ch.leftHeld && !ch.rightHeld)
			ch.leftHeld = true;
		
		if(!Inputs.IsActionPressed("player_left") && ch.leftHeld)
			ch.leftHeld = false;
		
		if(Inputs.IsActionJustPressed("player_right"))
		{
			if(ch.leftHeld && !ch.rightHeld) 
				ch.leftHeld = false;
			ch.rightHeld = true;
		}
		
		if(Inputs.IsActionJustReleased("player_right"))
		{
			ch.rightHeld = false;
			if(Inputs.IsActionPressed("player_left")) 
				ch.leftHeld = true;
		}
		
		if(Inputs.IsActionPressed("player_right") && !ch.leftHeld && !ch.rightHeld)
			ch.rightHeld = true;
		
		if(!Inputs.IsActionPressed("player_right") && ch.rightHeld)
			ch.rightHeld = false;
	}
	
	protected void SetVerticalAlternatingInputs()
	{
		if(Inputs.IsActionJustPressed("player_up"))
		{
			if(ch.downHeld && !ch.upHeld)
				ch.downHeld = false;
			ch.upHeld = true;
		}
		
		if(Inputs.IsActionJustReleased("player_up"))
		{
			ch.upHeld = false;
			if(Inputs.IsActionPressed("player_down"))
				ch.downHeld = true;
		}
		
		if(Inputs.IsActionPressed("player_up") && !ch.upHeld && !ch.downHeld)
			ch.upHeld = true;
		
		if(!Inputs.IsActionPressed("player_up") && ch.upHeld)
			ch.upHeld = false;
		
		if(Inputs.IsActionJustPressed("player_down"))
		{
			if(ch.upHeld && !ch.downHeld) 
				ch.upHeld = false;
			ch.downHeld = true;
		}
		
		if(Inputs.IsActionJustReleased("player_down"))
		{
			ch.downHeld = false;
			if(Inputs.IsActionPressed("player_up")) 
				ch.upHeld = true;
		}
		
		if(Inputs.IsActionPressed("player_down") && !ch.upHeld && !ch.downHeld)
			ch.downHeld = true;
		
		if(!Inputs.IsActionPressed("player_down") && ch.downHeld)
			ch.downHeld = false;
	}
	
	protected void SetFastFallInput()
	{
		if(Inputs.IsActionJustPressed("player_down")
		&& ch.GetVelocity().y >= ch.fastfallMargin)
		{
			ch.fastfalling = true;
		}
		
		if(!Inputs.IsActionPressed("player_down")
		|| ch.GetVelocity().y < ch.fastfallMargin)
		{
			ch.fastfalling = false;
		}
	}
	
	
	protected void SetDownHoldingInput() => ch.downHeld = Inputs.IsActionPressed("player_down");
	protected void SetUpHoldingInput() => ch.upHeld = Inputs.IsActionPressed("player_up");
	
	public void Unsnap() => snapVector = Vector2.Zero;
	
	public void MarkForDeletion(string action, bool now = false) => Inputs.MarkForDeletion(action, now);
	
	public void SetupCollisionParamaters()
	//does all the needed calculations on collision
	{
		ch.onSemiSolid = false;//reset semi solid detection
		ch.onSlope = false;//reset slope detection
		ch.aerial = true;//assume no collision for the start
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
			var cling = body.GetProp<bool>("Clingable", true);//get if clingable (wall property)
			
			
			//TODO: this entire check should probably only be done for floors, since it isnt used in walls and ceilings anyways
			var oneway = false;//init check if collided body is one way (only checks collision in a specific direction)
			//this goes over all of the "shape owners" and checks if they have one way collision enabeled
			if(!ch.onSemiSolid && body is CollisionObject2D col)//if the collider IS a collidable body, cuz trust is overrated
			{
				foreach(var sh in col.GetShapeOwners())//check each collision shape owners
				{
					uint shid = Convert.ToUInt32(sh);//convert int to uint
					if(col.IsShapeOwnerOneWayCollisionEnabled(shid))//name goes brrr
					{
						oneway = true;//set that shape is one way
						break;//finished checking
					}
				}
			}
			
			var fallthrough = body.GetProp<bool>("FallThroughPlatform", oneway);//get if can fall through the platform
			
			////////////////////////////////////////////////////////////////////////////////////////////////
			//now we set the paramaters related to the type of collision itself (ground, wall, ceiling, etc)
			////////////////////////////////////////////////////////////////////////////////////////////////
			
			if(norm == Vector2.Right || norm == Vector2.Left)
			//Wall if straight right or left
			{
				ch.walled = cling;//if not clingable, ignore being on a wall
				
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
				ch.onSlope = (norm != Vector2.Up);//get if you're on a slope, based on if the floor is straight
				ch.ffric = fric;//get floor friction
				ch.fbounce = bounce;//get floor bounce
				ch.onSemiSolid = fallthrough;//set fall through
			}
			
			if(ch.grounded || ch.walled || ch.ceilinged) ch.aerial = false;
			//collision exists, so character isnt aerial
			//the check ensures that non clingable walls wont mark the character as non aerial
		}
	}
	
	private IEnumerable<KinematicCollision2D> GetSlideCollisions()
	{
		for(int i = 0; i < ch.GetSlideCount(); ++i) yield return ch.GetSlideCollision(i);
	}
	
	public string VerySecretMethod() => base.ToString();
}
