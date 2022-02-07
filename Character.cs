using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Character : KinematicBody2D, IHittable, IAttacker
{
	public const int DROP_THRU_BIT = 1;
	protected const int CF = 30;
	
	public string statConfigPath = "res://";
	public string statSectionName = "Stats";
	
	[Signal]
	public delegate void Dead(Node2D who);
	[Signal]
	public delegate void JumpsRestored();
	
	////////////////////////////////////////////
	[Export]
	public string name = "Unnamed";//name used for debuging
	////////////////////////////////////////////
	public float fallSpeed = 800f;//max fall speed
	public float fastFallSpeed = 1200f;//max fastfall speed
	public float wallFallSpeed = 250f;//max wall fall speeed
	public float wallFastFallSpeed = 450f;//max wall fastfall speed
	public float gravity = 20f;//how fast you normally fall
	public float fastFallGravity = 60f;//how fast you fastfall
	public float wallGravity = 10f;//how fast you fall on a wall
	public float wallFastFallGravity = 20f;//how fast you fastfall on a wall
	////////////////////////////////////////////
	public float groundSpeed = 650f;//max ground speed
	public float airSpeed = 600f;//max air speed
	public float crawlSpeed = 200f;//crawling speed
	public float groundAcceleration = 75f;//how fast you reach groundSpeed
	public float airAcceleration = 45f;//how fast you reach airSpeed
	////////////////////////////////////////////
	public float jumpHeight = 400f;//how high you jump
	public float shorthopHeight = 200f;//how high you shorthop
	public float doubleJumpHeight = 600f;//how high you jump in the air
	public float horizontalWallJump = 400f;//horizontal velocity from wall jumping
	public float verticalWallJump = 400f;//vertical velocity from wall jumping
	public float fastfallMargin = -100f;
	////////////////////////////////////////////
	public uint jumpNum = 3;//how many air jump you have
	public uint wallJumpNum = 5;//how many wall jumps you have
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
	public uint impactLand = 2;//how many frames of inactionability there are after touching the ground
	public uint jumpSquat = 4;//how many frames before a ground jump comes out
	public uint wallLand = 2;//how many frames of inactionability there are after touching a wall
	public uint wallJumpSquat = 2;//how many frames before a wall jump comes out
	public uint walkTurn = 3;
	public uint duckLength = 3;
	public uint getupLength = 4;
	////////////////////////////////////////////
	public bool dummy = false;
	////////////////////////////////////////////
	public float damage = 0f;
	////////////////////////////////////////////
	private float _damageTakenMult = 1f;
	public float DamageTakenMult{get => _damageTakenMult; set => _damageTakenMult = value;}
	
	private float _knockbackTakenMult = 1f;
	public float KnockbackTakenMult{get => _knockbackTakenMult; set => _knockbackTakenMult = value;}
	
	private int _stunTakenMult = 1;
	public int StunTakenMult{get => _stunTakenMult; set => _stunTakenMult = value;}
	////////////////////////////////////////////
	private float _damageDoneMult = 1f;
	public float DamageDoneMult{get => _damageDoneMult; set => _damageDoneMult = value;}
	
	private float _knockbackDoneMult = 1f;
	public float KnockbackDoneMult{get => _knockbackDoneMult; set => _knockbackDoneMult = value;}
	
	public int _stunDoneMult = 1;
	public int StunDoneMult{get => _stunDoneMult; set => _stunDoneMult = value;}
	////////////////////////////////////////////
	public int direction = 1;//1 for right -1 for left
	
	private int _teamNumber = 0;
	public int TeamNumber{get => _teamNumber; set => _teamNumber=value;}
	public bool friendlyFire = false;
	public int stocks = 3;
	
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
	public float PlatBounce => grounded?ffric:walled?wfric:ceilinged?cfric:1f;
	
	public bool fastfalling = false;//wether or not fastfalling
	public uint jumpCounter = 0;//how many air jumps have been used
	public uint wallJumpCounter = 0;//how many wall jumps have been used
	
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
	
	public Attack currentAttack;
	
	public Dictionary<string, State> states = new Dictionary<string, State>();
		
	public CollisionSettings currentSetting = default;
	public CollisionShape2D collision;
	public Hurtbox hurtbox;
	public Dictionary<string, CollisionSettings> settings = new Dictionary<string, CollisionSettings>();
	
	public List<Attack> attacks = new List<Attack>();
	public Dictionary<string, Attack> attackDict = new Dictionary<string, Attack>();
	public Dictionary<Attack, int> attackCooldowns = new Dictionary<Attack, int>();
	
	public Dictionary<string, PackedScene> projectiles = new Dictionary<string, PackedScene>();
	public Dictionary<string, HashSet<Projectile>> activeProjectiles = new Dictionary<string, HashSet<Projectile>>();
	
	public List<string> StatList = new List<string>();
	public PropertyMap prop = new PropertyMap();
	
	public Vector2 OriginalCollisionPosition = default;
		
	public InputManager Inputs;
	
	public AnimationSprite sprite;
	public CharacterAudioManager audioManager;
	
	public Character() {}
	public Character(bool dummy) {this.dummy = dummy;}
	
	public override void _Ready()
	{
		SetupStates();
		SetupAttacks();
	}
	
	public bool ReadStats()
	{
		prop.ConfigFileToPropertyList(this, 
		statConfigPath+name+".ini", statSectionName, StatList);
		prop.LoadProperties(this);
		return true;
	}

	public override void _PhysicsProcess(float delta)
	{
		++framesSinceLastHit;
		StoreVelocities();
		TruncateVelocityIfInsignificant();
		UpdateAttackCooldowns();
		if(Input.IsActionJustPressed("reset")) Respawn();
		currentState?.SetInputs();
		currentState?.DoPhysics(delta);
			
		sprite.FlipH = DirectionToBool();
		
		UpdateCollision();
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
		try
		{
			return states[state];
		}
		catch (KeyNotFoundException)
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
		//AddState(new CrouchJumpState(this));
		
		AddState(new StunState(this));
		AddState(new HitPauseState(this));
		AddState(new HitLagState(this));
		AddState(new AttackState(this));
		AddState(new EndlagState(this));
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
		ApplySettings("Normal");
		ChangeState("Air");
		Position = Vector2.Zero;
		fastfalling = false;
		crouching = false;
		jumpCounter = 0;
		wallJumpCounter = 0;
		ResetVelocity();
		direction = 1;
		SetCollisionMaskBit(DROP_THRU_BIT, true);
		damage = 0f;
		framesSinceLastHit = 0;
		comboCount = 0;
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
	
	public virtual void UpdateCollision()
	{
		collision?.SetDeferred("position", OriginalCollisionPosition*new Vector2(direction, 1));
	}
	
	public bool ApplySettings(string setting)
	{
		try
		{
			return ApplySettings(settings[setting]);
		}
		catch(KeyNotFoundException)
		{
			try
			{
				return ApplySettings(settings["Normal"]);
			}
			catch(KeyNotFoundException) {return false;}
		}
	}
	
	public bool ApplySettings(CollisionSettings setting)
	{
		currentSetting = setting;
		(collision.Shape as RectangleShape2D).Extents = setting.CollisionExtents;
		var posval = setting.CollisionPosition*new Vector2(direction, 1);
		collision?.SetDeferred("position", posval);
		OriginalCollisionPosition = setting.CollisionPosition;
		
		hurtbox.Radius = setting.HurtboxRadius;
		hurtbox.Height = setting.HurtboxHeight;
		hurtbox.CollisionPosition = setting.HurtboxPosition;
		hurtbox.CollisionRotation = setting.HurtboxRotation;
		
		return true;
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
		ApplySettings("Normal");
		crouching = false;
	}
	
	///////////////////////////////////////////
	/////////////////Inputs////////////////////
	///////////////////////////////////////////
	
	public void Turn() => direction *= -1;
	
	public bool InputingTurn() => (GetFutureDirection() != direction);
	public bool InputingDirection() => leftHeld||rightHeld;
	
	public bool TurnConditional()
	{
		if(InputingTurn()) Turn();
		else return false;
		
		return true;
	}
	
	public bool IsIdle() => (Math.Truncate(GetVelocity().x) == 0);
	public bool IsStill() => (IsIdle() && !InputingDirection());
	
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
	
	///////////////////////////////////////////
	////////////////Attacks////////////////////
	///////////////////////////////////////////
	
	public virtual void SetupAttacks()
	{
		attacks = GetChildren().FilterType<Attack>().ToList();
		
		foreach(var a in attacks)
		{
			attackDict.Add(a.Name, a);
			attackCooldowns.Add(a, 0);
		}
	}
	
	public virtual bool CanHit(IHittable hitObject) => (hitObject != this)&&(hitObject.TeamNumber!=_teamNumber||friendlyFire);
	
	public void HandleGettingHit(HitData data)
	{
		var skb = data.Skb;
		var vkb = data.Vkb;
		var d = data.Damage;
		var stun = data.Stun;
		var hp = data.Hitpause;
		
		damage += d * DamageTakenMult;
		var force = (skb + damage*vkb/100f) * KnockbackTakenMult;
		var stunlen = stun * StunTakenMult;
		
		if(hp > 0)
		{
			var s = ChangeState<HitPauseState>();
			s.force = force;
			s.stunLength = stunlen;
			s.hitPauseLength = hp;
		}
		else if(stunlen > 0)
		{
			var s = ChangeState<StunState>();
			s.Force = force;
			s.stunLength = stunlen;
		}
		
		if(force.x != 0f) direction = Math.Sign(force.x);
		
		GD.Print($"\nLast hit was {framesSinceLastHit} frames ago");
		
		if(framesSinceLastHit <= 0) ++comboCount;
		else comboCount = 0;
		GD.Print($"Combo count is {comboCount+1}");
		
		framesSinceLastHit = 0;
		
		var HitSoundToPlay = data.Hitter.hitSound;
		PlaySound(HitSoundToPlay);
	}
	
	public void HandleHitting(HitData data)
	{
		Hitbox hitbox = data.Hitter;
		Hurtbox hurtbox = data.Hitee;
		if(hitbox.hitlag > 0)
		{
			var s = ChangeState<HitLagState>();
			s.hitLagLength = hitbox.hitlag;
		}
	}
	
	public virtual bool IsAttackInCooldown(Attack a)
	{
		var cd = GetAttackCooldown(a);
		if(cd is null) return false;
		else return (cd>0);
	}
	
	public virtual bool ExecuteAttack(Attack a)
	{
		if(a is null || !a.CanActivate() || IsAttackInCooldown(a)) return false;
		
		if(currentAttack != null) ResetCurrentAttack(null);
		
		currentAttack = a;
		currentAttack.Connect("AttackEnds", this, nameof(ResetCurrentAttack));
		var s = ChangeState<AttackState>();
		s.att = currentAttack;
		
		currentAttack.Start();
		return true;
	}
	
	public bool ExecuteAttack(string s) => ExecuteAttack(GetAttack(s));
	
	public Attack GetAttack(string s)
	{
		try
		{
			return attackDict[s];
		}
		catch(KeyNotFoundException)
		{
			GD.Print($"No attack {s} found. You probably forgot to add it to the loading list");
			return null;
		}
	}
	
	public int? GetAttackCooldown(string s) => GetAttackCooldown(GetAttack(s));
	
	public int? GetAttackCooldown(Attack a)
	{
		if(a is null)
		{
			GD.Print("Could not get cooldown for given attack as it is null");
			return null;
		}
		else
		{
			try
			{
				return attackCooldowns[a];
			}
			catch(KeyNotFoundException)
			{
				GD.Print($"Could not get cooldown for attack {a.Name} as it does not exist");
				return null;
			}
		}
	}
	
	public void UpdateAttackCooldowns()
	{
		var keys = new List<Attack>(attackCooldowns.Keys);
		foreach(var k in keys) attackCooldowns[k] = Math.Max(0, attackCooldowns[k] - 1);
	}
	
	public void ResetCurrentAttack(Attack a)
	{
		if(currentAttack is null) return;
		currentAttack.Disconnect("AttackEnds", this, nameof(ResetCurrentAttack));
		SetAttackCooldowns();
		currentAttack = null;
	}
	
	/*public virtual void EmitProjectile(Projectile2D proj)
	{
		GetParent().AddChild(proj);
		proj.Active = true;
	}*/
	
	public virtual void SetAttackCooldowns()
	{
		var cd = currentAttack.GetEndlag() + currentAttack.GetCooldown();
		attackCooldowns[currentAttack] = cd;
	}
	
	///////////////////////////////////////////
}
