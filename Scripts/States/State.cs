using Godot;
using System;

public class State : Node
{
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
		//SetupInputManager();
	}
	
	public virtual bool IsActionable() => true;
	
	//public void SetupInputManager() => Inputs = (InputManager)ch.GetRootNode(ch.dummy?"Dummy":"Buffer" + "Input");
	
	public override string ToString() => GetType().Name.Replace("State", "");
	
	public virtual void DoPhysics(float delta)
	{
		if(this != ch.currentState) return;
		
		if(!justInit) LoopActions();
		else justInit = false;
		
		frameCount++;
		RepeatActions();
		DoMovement();
		DoGravity();
		
		if(Inputs.IsActionJustPressed("player_jump"))
			DoJump();
		if(Inputs.IsActionJustPressed("player_light_attack"))
			LightAttack();
		
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
	public virtual void SetInputs() {}
	protected virtual bool CalcStateChange() => false;
	protected virtual void LoopActions() {}
	protected virtual void RepeatActions() {}
	public virtual void OnChange() {}
	protected virtual void LightAttack() {}
	protected virtual void CalculateAnimation() {}
	
	protected void SetHorizontalAlternatingInputs()
	{
		//TODO: probably find a way to optimize this. or atleast make it cleaner
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
	
	protected void SetDownHoldingInput()
	{
		/*if(Inputs.IsActionJustPressed("player_down"))
			ch.downHeld = true;
		if(Inputs.IsActionJustReleased("player_down"))
			ch.downHeld = false;
		if(Inputs.IsActionPressed("player_down") && !ch.downHeld)
			ch.downHeld = true;
		if(!Inputs.IsActionPressed("player_down") && ch.downHeld)
			ch.downHeld = false;*/
		ch.downHeld = Inputs.IsActionPressed("player_down");
	}
	
	protected void SetUpHoldingInput()
	{
		ch.upHeld = Inputs.IsActionPressed("player_up");
	}
	
	public void Unsnap() => snapVector = Vector2.Zero;
		
	public void MarkForDeletion(string action, bool now = false) => Inputs.MarkForDeletion(action, now);
	
	public void SetupCollisionParamaters()
	//does all the needed calculations on collision
	{
		ch.onSemiSolid = false;//reset semi solid
		ch.onSlope = false;//reset slope
		
		for(int i = 0; i < ch.GetSlideCount(); ++i)
		{
			var collision = ch.GetSlideCollision(i);//get current collision
			var vel = collision.ColliderVelocity;//get collision velocity
			var norm = collision.Normal;//get the collision normal
			
			var body = collision.Collider;//get body
			var fric = (float)(body.Get("PlatformFriction")??1f);//get friction
			var bounce = (float)(body.Get("PlatformBounce")??0f);//get bounce
			var cling = (bool)(body.Get("Clingable")??true);//get if clingable
			
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
					
				if(!ch.onSemiSolid && body is CollisionObject2D col)//if the collider IS a collider
				{
					foreach(var sh in col.GetShapeOwners())//check each collision shape owners
					{
						uint shid = Convert.ToUInt32(sh);//convert int to uint
						if(col.IsShapeOwnerOneWayCollisionEnabled(shid))//name goes brrr
						{
							ch.onSemiSolid = true;//set that shape is one way
							break;
						}
					}
				}
			}
		}
	}
	
	public string VerySecretMethod() => base.ToString();
}
