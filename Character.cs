using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Character : KinematicBody2D, IHittable, IAttacker
{
	public const int DROP_THRU_BIT = 1;
	protected const int CF = 30;
	
	public string statConfigPath = "res://";
	public string statSectionName = "Stats";
	
	[Signal]
	public delegate void Dead(Node2D who);
	[Signal]
	public delegate void OptionsRestoredFromGroundTouch();
	[Signal]
	public delegate void OptionsRestoredFromWallTouch();
	[Signal]
	public delegate void OptionsRestoredFromHitting();
	[Signal]
	public delegate void OptionsRestoredFromGettingHit();
	
	////////////////////////////////////////////
	[Export]
	public string name = "Unnamed";//name used for debuging
	////////////////////////////////////////////
	public float fallSpeed = 800f;//max fall speed
	public float fastFallSpeed = 1200f;//max fastfall speed
	public float wallFallSpeed = 250f;//max wall fall speeed
	public float wallFastFallSpeed = 450f;//max wall fastfall speed
	public float stunFallSpeed = 800f;//max stun fall speed
	public float gravity = 20f;//how fast you normally fall
	public float fastFallGravity = 60f;//how fast you fastfall
	public float wallGravity = 10f;//how fast you fall on a wall
	public float wallFastFallGravity = 20f;//how fast you fastfall on a wall
	public float stunGravity = 20f;//how fast you fall during stun
	////////////////////////////////////////////
	public float groundSpeed = 650f;//max ground speed
	public float airSpeed = 600f;//max air speed
	public float crawlSpeed = 200f;//crawling speed
	public float groundAcceleration = 75f;//how fast you reach groundSpeed
	public float airAcceleration = 45f;//how fast you reach airSpeed
	////////////////////////////////////////////
	public float jumpHeight = 600f;//how high you jump
	public float shorthopHeight = 400f;//how high you shorthop
	public float crouchJumpHeight = 600f;
	public float crouchShorthopHeight = 400f;
	public float doubleJumpHeight = 800f;//how high you jump in the air
	public float horizontalWallJump = 400f;//horizontal velocity from wall jumping
	public float verticalWallJump = 500f;//vertical velocity from wall jumping
	public float fastfallMargin = -400f;
	////////////////////////////////////////////
	public int maxClingsAllowed = 2;
	public int maxAirJumpsAllowed = 2;
	public int maxDodgesAllowed = 1;
	
	public int givenClingsOnHitting = 2;
	public int givenClingsOnGettingHit = 1;
	
	public int givenAirJumpsOnHitting = 1;
	public int givenAirJumpsOnGettingHit = 1;
	
	public int givenDodgesOnHitting = 1;
	public int givenDodgesOnGettingHit = 1;
	
	////////////////////////////////////////////
	public int forwardRollStartup;
	public int forwardRollLength;
	public float forwardRollSpeed;
	public int forwardRollEndlag;
	
	public int backRollStartup;
	public int backRollLength;
	public float backRollSpeed;
	public int backRollEndlag;
	
	public int dashStartup;
	public int dashLength;
	public int dashSpeed;
	public int dashCancelWindow;
	
	public int spotAirDodgeStartup = 3;
	public int spotAirDodgeLength = 15;
	public int spotAirDodgeEndlag = 3;
	
	public int directionalAirDodgeStartup = 3;
	public int directionalAirDodgeLength = 15;
	public float directionalAirDodgeSpeed = 1500;
	public int directionalAirDodgeEndlag = 3;
	
	public bool restoreDodgeOnGroundTouch = true;
	public bool restoreDodgeOnWallTouch = false;
	public bool restoreDodgeOnHitting = true;
	public bool restoreDodgeOnGettingHit = false;
	
	////////////////////////////////////////////
	public float ceilingBonkBounce = 0.25f;//how much speed is conserved when bonking
	public float ceilingBounce = 0.95f;//how much speed is conserved when hitting a ceiling
	public float wallBounce = 0.95f;//how much speed is conserved when hitting a wall
	public float floorBounce = 0.95f;//how much speed is conserved when hitting the floor
	////////////////////////////////////////////
	public float groundFriction = 0.2f;//how much speed is removed over time when not moving on the ground
	public float airFriction = 0.1f;//how much speed is removed over time when not moving in the air
	public float wallFriction = 0.4f;//how much speed is removed upon touching a wall
	public float slopeFriction = 0.1f;//how much speed is removed over time when not moving on a slope
	////////////////////////////////////////////
	public int impactLand = 2;//how many frames of inactionability there are after touching the ground
	public int jumpSquat = 4;//how many frames before a ground jump comes out
	public int crouchJumpSquat = 4;
	public int airJumpSquat = 0;
	public int wallLand = 2;//how many frames of inactionability there are after touching a wall
	public int wallJumpSquat = 2;//how many frames before a wall jump comes out
	public int walkTurn = 3;
	public int duckLength = 3;
	public int getupLength = 4;
	////////////////////////////////////////////
	public float damage = 0f;
	////////////////////////////////////////////
	private float _damageTakenMult = 1f;
	public float DamageTakenMult{get => _damageTakenMult; set => _damageTakenMult = value;}
	
	private float _knockbackTakenMult = 1f;
	public float KnockbackTakenMult{get => _knockbackTakenMult; set => _knockbackTakenMult = value;}
	
	private float _stunTakenMult = 1f;
	public float StunTakenMult{get => _stunTakenMult; set => _stunTakenMult = value;}
	////////////////////////////////////////////
	private float _damageDoneMult = 1f;
	public float DamageDoneMult{get => _damageDoneMult; set => _damageDoneMult = value;}
	
	private float _knockbackDoneMult = 1f;
	public float KnockbackDoneMult{get => _knockbackDoneMult; set => _knockbackDoneMult = value;}
	
	public float _stunDoneMult = 1f;
	public float StunDoneMult{get => _stunDoneMult; set => _stunDoneMult = value;}
	////////////////////////////////////////////
	public int direction = 1;//1 for right -1 for left
	public int Direction{get => direction; set => direction = value;}
	
	private int _teamNumber = 0;
	public int TeamNumber{get => _teamNumber; set => _teamNumber=value;}
	public bool friendlyFire = false;
	public int stocks = 3;
	
	public float weight = 100f;
	
	public int framesSinceLastHit = 0;
	public int comboCount = 0;
	
	public Vector2[] vs = new Vector2[7]{Vector2.Zero,Vector2.Zero,Vector2.Zero,Vector2.Zero,Vector2.Zero,Vector2.Zero,Vector2.Zero};
	public Vector2 vec = new Vector2();//normal velocity from movement
	public Vector2 vac = new Vector2();//speed transferred from moving platforms
	public Vector2 vic = new Vector2();//momentary force
	public Vector2 voc = new Vector2();//knockback
	public Vector2 vuc = new Vector2();
	public Vector2 vyc = new Vector2();
	public Vector2 vwc = new Vector2();
	
	public Vector2 fnorm = new Vector2();//floor normal
	public Vector2 fvel = new Vector2();//floor velocity
	public float ffric = 1f;//floor friction
	public float fbounce = 0f;//floor bounce
	
	public Vector2 wnorm = new Vector2();//wall normal
	public Vector2 wvel = new Vector2();//wall velocity
	public float wfric = 1f;//wall friction
	public float wbounce = 0f;//wall bounce
	
	public Vector2 cnorm = new Vector2();//ceiling normal
	public Vector2 cvel = new Vector2();//ceiling velocity
	public float cfric = 1f;//ceiling friction
	public float cbounce = 1f;//ceiling bounce
	
	public Vector2 Norm => grounded?fnorm:walled?wnorm:ceilinged?cnorm:Vector2.Zero;
	public float CharBounce => grounded?floorBounce:walled?wallBounce:ceilinged?ceilingBounce:0f;
	public Vector2 PlatVel => grounded?fvel:walled?wvel:ceilinged?cvel:Vector2.Zero;
	public float PlatFric => grounded?ffric:walled?wfric:ceilinged?cfric:1f;
	public float PlatBounce => grounded?fbounce:walled?wbounce:ceilinged?cbounce:0f;
	
	public float AppropriateFriction => PlatFric * (onSlope?slopeFriction:grounded?groundFriction:walled?wallFriction:airFriction);
	public float AppropriateBounce => PlatBounce * (grounded?floorBounce:walled?wallBounce:ceilinged?ceilingBounce:0f);
	public float AppropriateAcceleration => (grounded?groundAcceleration:airAcceleration);
	public float AppropriateSpeed => (crouching?crawlSpeed:grounded?groundSpeed:airSpeed);
	public float AppropriateGravity => (currentAttack?.currentPart.gravityMultiplier ?? 1f)*((currentState is StunState)?stunGravity:fastfalling?walled?wallFastFallGravity:fastFallGravity:walled?wallGravity:gravity);
	public float AppropriateFallingSpeed => (currentAttack?.currentPart.gravityMultiplier ?? 1f)*((currentState is StunState)?stunFallSpeed:fastfalling?walled?wallFastFallSpeed:fastFallSpeed:walled?wallFallSpeed:fallSpeed);
	
	public bool InputtingTurn => (GetFutureDirection() != direction);
	public bool InputtingHorizontalDirection => leftHeld||rightHeld;
	public bool InputtingVerticalDirection => upHeld||downHeld;
	public bool InputtingDirection => InputtingHorizontalDirection||InputtingVerticalDirection;
	public bool IsIdle => (Math.Truncate(GetVelocity().x / 100f) == 0);
	public bool IsStill => (IsIdle && !InputtingHorizontalDirection);
	
	public bool fastfalling = false;//wether or not fastfalling
	
	public bool grounded = false;//is on ground
	public bool walled = false;//is on wall
	public bool ceilinged = false;//is touching ceiling
	public bool aerial = false;//is in air
	public bool crouching = false;//is currently crouching
	public bool onSemiSolid = false;//is currently on a semi solid platform
	public bool onSlope = false;//is currently on a slope
	
	public bool leftHeld = false;//is left currently held
	public bool rightHeld = false;//is right currently held
	public bool downHeld = false;//is down currently held
	public bool upHeld = false;//is up currently held
	
	public State currentState;//currently used state
	public State prevState;//previously used state
	public State prevPrevState;//the one before that
	
	public Dictionary<string, State> states = new Dictionary<string, State>();
	
	private List<Hurtbox> _hurtboxes = new List<Hurtbox>();
	public List<Hurtbox> Hurtboxes{get => _hurtboxes; set => _hurtboxes=value;}
	
	public string currentCollisionSetting;
	public CharacterCollision collision;
	public PlatformDropDetector DropDetector;
	
	public Attack currentAttack;
	public Attack CurrentAttack{get => currentAttack; set => currentAttack = value;}
	
	private bool hasHit;
	public bool IsHitting{get => hasHit; set => hasHit = value;}
	private IHittable lastHit;
	public IHittable LastHit{get => lastHit; set => lastHit = value;}
	
	public Dictionary<string, Attack> attackDict = new Dictionary<string, Attack>();
	public Dictionary<string, Attack> Attacks{get => attackDict; set => attackDict = value;}
	
	public Dictionary<string, int> cooldowns = new Dictionary<string, int>();
	public Dictionary<string, int> invincibilityTimers = new Dictionary<string, int>();
	public Dictionary<string, int> resources = new Dictionary<string, int>();
	
	public Dictionary<string, PackedScene> projectiles = new Dictionary<string, PackedScene>();
	public Dictionary<string, HashSet<Projectile>> activeProjectiles = new Dictionary<string, HashSet<Projectile>>();
	public ProjectilePool objectPool;
	
	public List<string> StatList = new List<string>();
	public PropertyMap prop = new PropertyMap();
	
	public InputManager Inputs;
	
	public AnimationSprite sprite;
	protected AudioManager audioManager;
	public AudioManager Audio{get => audioManager; set => audioManager = value;}
	
	public Character() {}
	
	public override void _Ready()
	{
		ZIndex = 4;
		SetupStates();
	}
	
	public bool ReadStats()
	{
		prop.ConfigFileToPropertyList(this, statConfigPath+name+".ini", statSectionName, StatList);
		prop.LoadProperties(this);
		return true;
	}

	public override void _PhysicsProcess(float delta)
	{
		IsHitting = false;
		++framesSinceLastHit;
		StoreVelocities();
		TruncateVelocityIfInsignificant();
		UpdateCooldowns();
		UpdateInvincibilityTimers();
		if(Input.IsActionJustPressed("reset")) Respawn();
		currentState?.SetInputs();
		currentState?.DoPhysics(delta);
		
		sprite.FlipH = DirectionToBool();
		Update();
	}
	
	public override void _Draw()
	{
		if(!this.GetRootNode<UpdateScript>("UpdateScript").debugCollision) return;
		ZIndex = 10;
		DrawCircle(Vector2.Zero, 5, new Color(0,0,0,1));
	}
	
	public void PlayAnimation(string anm) => sprite.Play(anm);
	public void QueueAnimation(string anm) => sprite.Queue(anm);
	public void PlaySound(string sound) => audioManager.Play(sound);
	public void PlaySound(AudioStream sound) => audioManager.Play(sound);
	
	///////////////////////////////////////////
	///////////////States//////////////////////
	///////////////////////////////////////////
	
	//this function returns the appropriate state
	public State GetState(string state)
	{
		if(states.ContainsKey(state)) return states[state];
		else
		{
			GD.Print($"{name}: Could not find {state}State");
			return null;
		}
	}
	
	public string GetStateName<T>() where T : State => typeof(T).Name.Replace("State", "");
	
	public T GetState<T>() where T : State => (T)GetState(GetStateName<T>());
	
	public bool HasState(string state) => states.ContainsKey(state);
	
	public bool AddState(State state)
	{
		try
		{
			states.Add(state.ToString(), state);
			return true;
		}
		catch (ArgumentException)
		{
			GD.Print($"{name}: Attempt to add state {state}State failed because that state already exists");
			return false;
		}
	}
	
	public T ChangeState<T>() where T : State => (T)ChangeState(GetStateName<T>());
	
	//this function handles state changes
	public State ChangeState(string state)
	{
		var tempState = GetState(state);
		
		if(tempState is null)
		{
			GD.Print($"{name}: Failed to switch to {state}State because that state is null or doesn't exist");
			return null;
		}
		
		currentState?.OnChange(tempState);
		currentState?.EmitSignal("StateEnd", currentState);
		
		if(currentState == tempState)
		{
			currentState.ForcedInit();
			return currentState;
		}
		
		#if DEBUG_STATES
		GD.Print('\n');
		GD.Print("Character num " + teamNumber);
		GD.Print("New state is " + tempState?.ToString() ?? "null");
		GD.Print("Current state is " + currentState?.ToString() ?? "null");
		GD.Print("Prev state is " + prevState?.ToString() ?? "null");
		GD.Print("Prev prev state is " + prevPrevState?.ToString() ?? "null");
		#endif
		
		prevPrevState = prevState;
		prevState = currentState;
		currentState = tempState;
		currentState.ForcedInit();
		
		return currentState;
	}
	
	//initializes the states
	private void SetupStates()
	{
		AddState(new AirState(this));
		//AddState(new AirTurnState(this));
		AddState(new AirJumpState(this));
		
		AddState(new WallState(this));
		AddState(new WallLandState(this));
		AddState(new WallJumpState(this));
		
		AddState(new GroundedState(this));
		AddState(new LandState(this));
		AddState(new JumpState(this));
		
		AddState(new IdleState(this));
		AddState(new WalkState(this));
		AddState(new WalkTurnState(this));
		AddState(new WalkStopState(this));
		AddState(new WalkWallState(this));
		
		AddState(new GetupState(this));
		AddState(new DuckState(this));
		AddState(new CrouchState(this));
		AddState(new CrawlState(this));
		AddState(new CrawlWallState(this));
		AddState(new CrouchJumpState(this));
		
		AddState(new StunState(this));
		AddState(new HitPauseState(this));
		AddState(new HitLagState(this));
		AddState(new AttackState(this));
		AddState(new EndlagState(this));
		
		//AddState(new ForwardRollState(this));
		//AddState(new BackRollState(this));
		AddState(new SpotAirDodgeState(this));
		AddState(new DirectionalAirDodgeState(this));
		
		AddState(new WavedashState(this));
	}
	
	///////////////////////////////////////////
	///////////////Misc////////////////////////
	///////////////////////////////////////////
	
	public void Die()
	{
		GD.Print($"Character {Name} died with {stocks} stocks");
		--stocks;
		if(stocks <= 0)
		{
			GD.Print($"Character {Name} took the L and is eliminated");
			EmitSignal(nameof(Dead), this);
			QueueFree();
		}
		else Respawn();
	}
	
	public void Respawn()
	{
		GD.Print("\nRespawning...");
		Inputs?.MarkAllForDeletion();
		currentAttack?.Stop();
		ApplySettings("Default");
		ChangeState("Air");
		Position = Vector2.Zero;
		invincibilityTimers.Clear();//todo: respawn i-frames
		fastfalling = false;
		crouching = false;
		ResetVelocity();
		direction = 1;
		SetCollisionMaskBit(DROP_THRU_BIT, true);
		damage = 0f;
		framesSinceLastHit = 0;
		comboCount = 0;
		DisableAllProjectiles();
	}
	
	public virtual void DisableAllProjectiles()
	{
		var activeProjectilesCopy = new Dictionary<string, HashSet<Projectile>>(activeProjectiles);
		foreach(var entry in activeProjectilesCopy)
			entry.Value.ToList().ForEach(p => p.Destruct());
	}
	
	public virtual void RestoreOptionsOnGroundTouch()
	{
		SetResource("Clings", maxClingsAllowed);
		SetResource("AirJumps", maxAirJumpsAllowed);
		SetResource("OnHittingOptionRestoration", 1);
		SetResource("OnGettingHitOptionRestoration", 1);
		if(restoreDodgeOnGroundTouch) SetResource("Dodge", maxDodgesAllowed);
		EmitSignal(nameof(OptionsRestoredFromGroundTouch));
	}
	
	public virtual void RestoreOptionsOnWallTouch()
	{
		GiveResource("Clings", -1);
		SetResource("AirJumps", maxAirJumpsAllowed);
		if(restoreDodgeOnWallTouch) SetResource("Dodge", maxDodgesAllowed);
		EmitSignal(nameof(OptionsRestoredFromWallTouch));
	}
	
	public virtual void RestoreOptionsOnHitting()
	{
		if(!HasResource("OnHittingOptionRestoration")) return;
		GiveResource("Clings", givenClingsOnHitting, maxClingsAllowed);
		GiveResource("AirJumps", givenAirJumpsOnHitting, maxAirJumpsAllowed);
		if(restoreDodgeOnHitting) GiveResource("Dodge", givenDodgesOnHitting, maxDodgesAllowed);
		EmitSignal(nameof(OptionsRestoredFromHitting));
		GiveResource("OnHittingOptionRestoration", -1);
	}
	
	public virtual void RestoreOptionsOnGettingHit()
	{
		if(!HasResource("OnGettingHitOptionRestoration")) return;
		GiveResource("Clings", givenClingsOnGettingHit, maxClingsAllowed);
		GiveResource("AirJumps", givenAirJumpsOnGettingHit, maxAirJumpsAllowed);
		if(restoreDodgeOnGettingHit) GiveResource("Dodge", givenDodgesOnGettingHit, maxDodgesAllowed);
		EmitSignal(nameof(OptionsRestoredFromGettingHit));
		GiveResource("OnGettingHitOptionRestoration", -1);
	}
	
	public virtual void StoreVelocities()
	{
		vs[0] = vec;
		vs[1] = vac;
		vs[2] = vic;
		vs[3] = voc;
		vs[4] = vuc;
		vs[5] = vyc;
		vs[6] = vwc;
	}
	
	public virtual void LoadVelocities()
	{
		vec = vs[0];
		vac = vs[1];
		vic = vs[2];
		voc = vs[3];
		vuc = vs[4];
		vyc = vs[5];
		vwc = vs[6];
	}
	
	public virtual void ResetVelocity()
	{
		for(int i = 0; i < vs.Length; ++i)
			vs[i] = Vector2.Zero;
		LoadVelocities();
	}
	
	public virtual void TruncateVelocityIfInsignificant()
	{
		for(int i = 0; i < vs.Length; ++i)
			vs[i].TruncateIfInsignificant();
		LoadVelocities();
	}
	
	public override string ToString() => name;
	
	public Vector2 GetVelocity() => vs.Aggregate(Vector2.Zero, (a,v)=>a+v);
	public Vector2 GetRoundedVelocity() => GetVelocity().Round();
	public Vector2 GetRoundedPosition() => Position.Round();
	
	public string GetWallDirection()
	{
		if(walled) switch(direction)
		{
			case 1: return "Right";
			case -1: return "Left";
			default: return "WTF";
		}
		else return "None";
	}
	
	//FIX: this doesnt account for inheritence
	//TODO: just use some function like IsActionable
	public readonly static Type[] ignoreTypes = new Type[]{typeof(AttackState), typeof(EndlagState), typeof(StunState)};
	
	public virtual void OnSemiSolidLeave(Godot.Object body) 
	{
		foreach(var t in ignoreTypes) if(currentState.GetType() == t) return;
		SetCollisionMaskBit(DROP_THRU_BIT, true);
	}
	
	public void AttachEffect(Effect e)
	{
		AddChild(e);
		//TODO: make the effect inserted into a list
		//so that other scripts can reference it
		//just make sure to remove
	}
	
	///////////////////////////////////////////
	////////////////Collision//////////////////
	///////////////////////////////////////////
	
	public void ApplySettings(string setting)
	{
		currentCollisionSetting = setting;
		collision.ChangeState(setting);
		DropDetector.UpdateBasedOnCollisionShape();
		Hurtboxes.ForEach(h=>h.ChangeState(setting));
	}
	
	//checks if changing to a practicular collision shape is safe
	//unused
	public bool CanChange(CollisionShape2D collShape)
	{
		var spaceState = GetWorld2d().DirectSpaceState;
		var query = new Physics2DShapeQueryParameters();
		query.SetShape(collShape.Shape);
		query.Transform = collShape.GlobalTransform;
		
		var trav = new Vector2(0, (fvel.y < 0)?fvel.y:0);
		
		query.Transform = query.Transform.Translated(CF*fnorm + trav);
		var temp = GetCollisionMaskBit(DROP_THRU_BIT);
		SetCollisionMaskBit(DROP_THRU_BIT, false);
		query.CollisionLayer = CollisionMask;
		SetCollisionMaskBit(DROP_THRU_BIT, temp);
		query.CollideWithAreas = false;
		query.CollideWithBodies = true;
		
		var result = spaceState.IntersectShape(query);
		
		for(int i = 0; i < result.Count; ++i)
		{
			var res = result[i] as Godot.Collections.Dictionary;
			var collider = res["collider"];
			
			if(collider == this) result.RemoveAt(i);
		}
		
		return (result?.Count == 0);
	}
	
	//crouches
	public void Crouch()
	{
		ApplySettings("Crouch");
		crouching = true;
	}
	
	//uncrouches
	public void Uncrouch()
	{
		ApplySettings("Default");
		crouching = false;
	}
	
	///////////////////////////////////////////
	/////////////////Inputs////////////////////
	///////////////////////////////////////////
	
	public void Turn() => direction *= -1;
	
	public bool TurnConditional()
	{
		if(InputtingTurn) Turn();
		else return false;
		
		return true;
	}
	
	public int GetInputDirection()
	{
		if(leftHeld && rightHeld) return 0;
		else if(rightHeld) return 1;
		else if(leftHeld) return -1;
		else return 0;
	}
	
	public int GetFutureDirection()
	{
		if(rightHeld && leftHeld) return 0;
		else if(rightHeld) return 1;
		else if(leftHeld) return -1;
		else return direction;
	}
	
	public bool DirectionToBool() => (direction != 1);
	//false = right, left = true. used for flipping sprite;
	
	public Vector2 GetInputVector()
	{
		var x = rightHeld?1:leftHeld?-1:0;
		var y = downHeld?1:upHeld?-1:0;
		return new Vector2(x,y).Normalized();
	}
	
	///////////////////////////////////////////
	////////////////Attacks////////////////////
	///////////////////////////////////////////
	
	public virtual bool CanHit(IHittable hitObject) => (hitObject != this)&&(!hitObject.IsInvincible())&&(hitObject.TeamNumber!=_teamNumber||friendlyFire);
	
	public void HandleGettingHit(HitData data)
	{
		GD.Print($"Character {Name} got hit on {Engine.GetPhysicsFrames()}");
		var skb = data.Skb;
		var vkb = data.Vkb;
		var d = data.Damage;
		var stun = data.Stun;
		var hp = data.Hitpause;
		
		RestoreOptionsOnGettingHit();
		
		damage += d * DamageTakenMult;
		var force = (skb + damage*vkb/100f) * KnockbackTakenMult * (100f/weight);
		var stunlen = stun * StunTakenMult;
		
		if(hp > 0)
		{
			var s = ChangeState<HitPauseState>();
			s.force = force;
			s.stunLength = (int)stunlen;
			s.hitPauseLength = hp;
		}
		else if(stunlen > 0)
		{
			var s = ChangeState<StunState>();
			s.Force = force;
			s.stunLength = (int)stunlen;
		}
		
		if(force.x != 0f) direction = Math.Sign(force.x);
		
		GD.Print($"\nLast hit was {framesSinceLastHit} frames ago");
		
		if(framesSinceLastHit <= 0) ++comboCount;
		else comboCount = 0;
		GD.Print($"Combo count is {comboCount+1}");
		
		framesSinceLastHit = 0;
		
		PlaySound(data.Hitter.hitSound);
	}
	
	public void HandleHitting(HitData data)
	{
		GD.Print($"Character {Name} hit something on {Engine.GetPhysicsFrames()}");
		Hitbox hitbox = data.Hitter;
		Hurtbox hurtbox = data.Hitee;
		
		if(hurtbox.OwnerObject is IAttacker attackerOwner && attackerOwner.IsHitting && attackerOwner.LastHit == this)
		{
			GD.Print("CLASH");//TODO: decide what should happen
		}
		
		RestoreOptionsOnHitting();
		
		if(hitbox.hitlag > 0)
		{
			var s = ChangeState<HitLagState>();
			s.hitLagLength = hitbox.hitlag;
		}
	}
	
	public virtual bool InCooldown(string s) => GetCooldown(s) > 0;
	
	public virtual bool AttackInCooldown(Attack a) => a.sharesCooldownWith.Concat(a.Name).Any(InCooldown);
	
	public virtual bool ExecuteAttack(Attack a)
	{
		if(a is null || !a.CanActivate() || AttackInCooldown(a)) return false;
		
		if(currentAttack != null) ResetCurrentAttack(null);
		
		currentAttack = a;
		currentAttack.Connect("AttackEnds", this, nameof(ResetCurrentAttack));
		var s = GetState<AttackState>();
		s.att = currentAttack;
		ChangeState<AttackState>();
		
		currentAttack.Start();
		return true;
	}
	
	public bool ExecuteAttack(string s) => ExecuteAttack(GetAttack(s));
	
	public Attack GetAttack(string s)
	{
		if(attackDict.ContainsKey(s)) return attackDict[s];
		else
		{
			GD.Print($"No attack {s} found on character {Name}");
			return null;
		}
	}
	
	public int GetCooldown(string s)
	{
		if(cooldowns.ContainsKey(s)) return cooldowns[s];
		else return 0;
	}
	
	public void SetCooldown(string s, int cd)
	{
		if(cooldowns.ContainsKey(s)) cooldowns[s] = cd;
		else cooldowns.Add(s, cd);
	}
	
	public void UpdateCooldowns()
	{
		var keys = new List<string>(cooldowns.Keys);
		foreach(var k in keys)
		{
			cooldowns[k]--;
			if(cooldowns[k] <= 0) cooldowns.Remove(k);
		}
	}
	
	public void ResetCurrentAttack(Attack a)
	{
		if(currentAttack is null) return;
		currentAttack.Disconnect("AttackEnds", this, nameof(ResetCurrentAttack));
		SetAttackCooldowns();
		currentAttack = null;
	}
	
	public virtual void EmitProjectile(string proj)
	{
		try
		{
			//get pooled projectile
			var generatedProjectile = objectPool.GetProjectile(proj);
			if(generatedProjectile is null)
			{
				GD.Print($"Failed to emit projectile {proj} because the object pool returned a null");
			}
			else
			{
				if(!activeProjectiles.ContainsKey(proj))//projectile havent been used yet. create storage
					activeProjectiles.Add(proj, new HashSet<Projectile>());
				//set direction
				generatedProjectile.direction = direction;
				//add owner
				//generatedProjectile.OwnerObject = this;
				//connect destruction signal
				generatedProjectile.Connect("ProjectileDied", this, nameof(HandleProjectileDestruction));
				//store as active
				activeProjectiles[proj].Add(generatedProjectile);
				//request that _Ready will be called
				generatedProjectile.RequestReady();
				//add to scene
				GetParent().AddChild(generatedProjectile);
			}
		}
		catch(KeyNotFoundException)
		{
			GD.Print($"Character {Name} does not have projectile {proj} defined, you idiot");
		}
	}
	
	public virtual void HandleProjectileDestruction(Projectile who)
	{
		var identifier = who.identifier;
		try
		{
			activeProjectiles[identifier].Remove(who);
			objectPool.InsertProjectile(who);
			who.Disconnect("ProjectileDied", this, nameof(HandleProjectileDestruction));
		}
		catch(KeyNotFoundException)
		{
			GD.Print($"Projectile {identifier} died but was never reported as active. TF");
		}
	}
	
	public virtual void SetAttackCooldowns()
	{
		var cd = currentAttack.GetEndlag() + currentAttack.GetCooldown();
		SetCooldown(currentAttack.Name, cd);
	}
	
	///////////////////////////////////////////
	/////////////Invincibility/////////////////
	///////////////////////////////////////////
	
	public void AddInvincibility(string source, int length) => invincibilityTimers.Add(source, length);
	public void RemoveInvincibility(string source) => invincibilityTimers.Remove(source);
	public void HasInvincibilityFrom(string source) => invincibilityTimers.ContainsKey(source);
	public int InvincibilityLeftFrom(string source) => invincibilityTimers[source];
	
	public void UpdateInvincibilityTimers()
	{
		var keys = new List<string>(invincibilityTimers.Keys);
		foreach(var k in keys)
		{
			invincibilityTimers[k]--;
			if(invincibilityTimers[k] <= 0) invincibilityTimers.Remove(k);
		}
	}
	
	public bool IsInvincible() => invincibilityTimers.Count > 0;
	
	///////////////////////////////////////////
	/////////////////Resources/////////////////
	///////////////////////////////////////////
	
	public void GiveResource(string resource, int amount, int min, int max)
	{
		if(resources.ContainsKey(resource))
		{
			int desired = Math.Max(Math.Min(resources[resource] + amount, max), Math.Max(min,0));
			if(desired == 0) resources.Remove(resource);
			else resources[resource] = desired;
		}
		else if(amount > 0) resources.Add(resource, amount);
	}
	
	public void GiveResource(string resource, int amount, int max) => GiveResource(resource, amount, 0, max);
	public void GiveResource(string resource, int amount) => GiveResource(resource, amount, int.MaxValue);
	
	public void SetResource(string resource, int amount)
	{
		if(amount <= 0) RemoveResource(resource);
		else if(resources.ContainsKey(resource)) resources[resource] = amount;
		else resources.Add(resource, amount);
	}
	
	public void RemoveResource(string resource)
	{
		if(resources.ContainsKey(resource)) resources.Remove(resource);
	}
	
	public int ResourceCount(string resource) => resources.ContainsKey(resource)?resources[resource]:0;
	public bool HasResource(string resource) => HasResourceBeyondThreshold(resource, 0);
	public bool HasResourceBeyondThreshold(string resource, int threshold) => resources.ContainsKey(resource) && (resources[resource] > threshold);
}
