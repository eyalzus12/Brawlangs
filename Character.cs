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
	public float clashStun = 10f;
	public float clashForce = 100f;
	////////////////////////////////////////////
	public float damage = 0f;
	////////////////////////////////////////////
	public float DamageTakenMult{get; set;} = 1f;
	public float KnockbackTakenMult{get; set;} = 1f;
	public float StunTakenMult{get; set;} = 1f;
	////////////////////////////////////////////
	public float DamageDoneMult{get; set;} = 1f;
	public float KnockbackDoneMult{get; set;} = 1f;
	public float StunDoneMult{get; set;} = 1f;
	////////////////////////////////////////////
	public int Direction{get; set;}//1 for right -1 for left
	
	public int TeamNumber{get; set;}
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
	
	public Vector2 Velocity => vs.Aggregate(Vector2.Zero, (a,v)=>a+v);
	public Vector2 RoundedVelocity => Velocity.Round();
	public Vector2 RoundedPosition => Position.Round();
	
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
	public float AppropriateGravity => (CurrentAttack?.currentPart?.gravityMultiplier ?? 1f)*((States.Current is StunState)?stunGravity:fastfalling?walled?wallFastFallGravity:fastFallGravity:walled?wallGravity:gravity);
	public float AppropriateFallingSpeed => (CurrentAttack?.currentPart?.gravityMultiplier ?? 1f)*((States.Current is StunState)?stunFallSpeed:fastfalling?walled?wallFastFallSpeed:fastFallSpeed:walled?wallFallSpeed:fallSpeed);
	
	public int InputDirection => rightHeld?(leftHeld?0:1):(leftHeld?-1:0);
	public int FutureDirection => rightHeld?(leftHeld?0:1):(leftHeld?-1:Direction);
	public Vector2 InputVector => new Vector2((rightHeld?1:leftHeld?-1:0),(downHeld?1:upHeld?-1:0)).Normalized();
	
	public bool InputtingTurn => (FutureDirection != Direction);
	public bool InputtingHorizontalDirection => leftHeld||rightHeld;
	public bool InputtingVerticalDirection => upHeld||downHeld;
	public bool InputtingDirection => InputtingHorizontalDirection||InputtingVerticalDirection;
	public bool Idle => (Math.Truncate(Velocity.x / 100f) == 0);
	public bool Still => (Idle && !InputtingHorizontalDirection);
	
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
	
	public StateMachine States{get; set;}
	
	public List<Hurtbox> Hurtboxes{get; set;} = new List<Hurtbox>();
	
	public string currentCollisionSetting;
	public CharacterCollision collision;
	public PlatformDropDetector DropDetector;
	
	private Attack currentAttack;
	public Attack CurrentAttack{get => currentAttack; set => currentAttack = value;}
	
	public bool Hitting{get; set;}
	public IHittable LastHitee{get; set;}
	public bool GettingHit{get; set;}
	public IHitter LastHitter{get; set;}
	
	public bool Clashing{get; set;}
	public HitData ClashData{get; set;}
	
	private Dictionary<string, Attack> attackDict = new Dictionary<string, Attack>();
	public Dictionary<string, Attack> Attacks{get => attackDict; set => attackDict = value;}
	
	public CooldownManager Cooldowns{get; set;} = new CooldownManager();
	
	public InvincibilityManager IFrames{get; set;} = new InvincibilityManager();
	public bool Invincible => IFrames.Count > 0;
	
	public ResourceManager Resources{get; set;} = new ResourceManager();
	
	public Dictionary<string, PackedScene> projectiles = new Dictionary<string, PackedScene>();
	public Dictionary<string, HashSet<Projectile>> activeProjectiles = new Dictionary<string, HashSet<Projectile>>();
	public ProjectilePool projPool;
	
	public List<string> StatList = new List<string>();
	public PropertyMap prop = new PropertyMap();
	
	public InputManager Inputs;
	
	public AnimationSprite CharacterSprite{get; set;}
	public AudioManager Audio{get; set;}
	
	public Character() {}
	
	public override void _Ready()
	{
		ZIndex = 4;
		States = new StateMachine(CreateStates());
	}

	public override void _PhysicsProcess(float delta)
	{
		if(Clashing) HandleClashing(ClashData);
		Clashing = false;
		Hitting = false;
		GettingHit = false;
		
		++framesSinceLastHit;
		StoreVelocities();
		TruncateVelocityIfInsignificant();
		Cooldowns.Update();
		IFrames.Update();
		States.Update(delta);
		if(Input.IsActionJustPressed("reset")) Respawn();
		
		CharacterSprite.FlipH = (Direction == -1);
		Update();
	}
	
	public override void _Draw()
	{
		if(!this.GetRootNode<UpdateScript>("UpdateScript").debugCollision) return;
		DrawCircle(Vector2.Zero, 5, new Color(0,0,0,1));
	}
	
	public void PlayAnimation(string anm) => CharacterSprite.Play(anm);
	public void QueueAnimation(string anm) => CharacterSprite.Queue(anm);
	public void PlaySound(string sound) => Audio.Play(sound);
	public void PlaySound(AudioStream sound) => Audio.Play(sound);
	
	///////////////////////////////////////////
	///////////////States//////////////////////
	///////////////////////////////////////////
	
	private IEnumerable<State> CreateStates()
	{
		yield return new AirState(this);
		yield return new AirJumpState(this);
		
		yield return new WallState(this);
		yield return new WallLandState(this);
		yield return new WallJumpState(this);
		
		yield return new GroundedState(this);
		yield return new LandState(this);
		yield return new JumpState(this);
		
		yield return new IdleState(this);
		yield return new WalkState(this);
		yield return new WalkTurnState(this);
		yield return new WalkStopState(this);
		yield return new WalkWallState(this);
		
		yield return new GetupState(this);
		yield return new DuckState(this);
		yield return new CrouchState(this);
		yield return new CrawlState(this);
		yield return new CrawlWallState(this);
		yield return new CrouchJumpState(this);
		
		yield return new StunState(this);
		yield return new HitPauseState(this);
		yield return new HitLagState(this);
		yield return new AttackState(this);
		yield return new EndlagState(this);
		
		yield return new SpotAirDodgeState(this);
		yield return new DirectionalAirDodgeState(this);
		
		yield return new WavedashState(this);
	}
	
	///////////////////////////////////////////
	///////////////Misc////////////////////////
	///////////////////////////////////////////
	
	public void Die()
	{
		GD.Print($"Character {this} died with {stocks} stocks");
		--stocks;
		if(stocks <= 0)
		{
			GD.Print($"Character {this} took the L and is eliminated");
			EmitSignal(nameof(Dead), this);
			QueueFree();
		}
		else Respawn();
	}
	
	public void Respawn()
	{
		GD.Print("\nRespawning...");
		Inputs?.MarkAllForDeletion();
		CurrentAttack?.Stop();
		ApplySettings("Default");
		States.Change("Air");
		Position = Vector2.Zero;
		IFrames.Clear();//todo: respawn i-frames
		fastfalling = false;
		crouching = false;
		ResetVelocity();
		Direction = 1;
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
		Resources["Clings"] = maxClingsAllowed;
		Resources["AirJumps"] = maxAirJumpsAllowed;
		Resources["OnHittingOptionRestoration"] = 1;
		Resources["OnGettingHitOptionRestoration"] = 1;
		if(restoreDodgeOnGroundTouch) Resources["Dodge"] = maxDodgesAllowed;
		EmitSignal(nameof(OptionsRestoredFromGroundTouch));
	}
	
	public virtual void RestoreOptionsOnWallTouch()
	{
		Resources.Give("Clings", -1);
		Resources["AirJumps"] = maxAirJumpsAllowed;
		if(restoreDodgeOnWallTouch) Resources["Dodge"] = maxDodgesAllowed;
		EmitSignal(nameof(OptionsRestoredFromWallTouch));
	}
	
	public virtual void RestoreOptionsOnHitting()
	{
		if(!Resources.Has("OnHittingOptionRestoration")) return;
		Resources.Give("Clings", givenClingsOnHitting, maxClingsAllowed);
		Resources.Give("AirJumps", givenAirJumpsOnHitting, maxAirJumpsAllowed);
		if(restoreDodgeOnHitting) Resources.Give("Dodge", givenDodgesOnHitting, maxDodgesAllowed);
		EmitSignal(nameof(OptionsRestoredFromHitting));
		Resources.Give("OnHittingOptionRestoration", -1);
	}
	
	public virtual void RestoreOptionsOnGettingHit()
	{
		if(!Resources.Has("OnGettingHitOptionRestoration")) return;
		Resources.Give("Clings", givenClingsOnGettingHit, maxClingsAllowed);
		Resources.Give("AirJumps", givenAirJumpsOnGettingHit, maxAirJumpsAllowed);
		if(restoreDodgeOnGettingHit) Resources.Give("Dodge", givenDodgesOnGettingHit, maxDodgesAllowed);
		EmitSignal(nameof(OptionsRestoredFromGettingHit));
		Resources.Give("OnGettingHitOptionRestoration", -1);
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
	
	public override string ToString() => Name;
	
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
	
	public void Turn() => Direction *= -1;
	
	public bool TurnConditional()
	{
		if(InputtingTurn) Turn();
		else return false;
		
		return true;
	}
	
	///////////////////////////////////////////
	////////////////Attacks////////////////////
	///////////////////////////////////////////
	
	public virtual bool CanHit(IHittable hitObject) => (hitObject != this)&&(!hitObject.Invincible)&&(hitObject.TeamNumber!=TeamNumber||friendlyFire);
	
	public virtual void HandleGettingHit(HitData data)
	{
		//GD.Print($"{this} runs Handle Getting Hit");
		var skb = data.Skb;
		var vkb = data.Vkb;
		var d = data.Damage;
		var stun = data.Stun;
		var hp = data.Hitpause;
		Hitbox hitbox = data.Hitter;
		Hurtbox hurtbox = data.Hitee;
		var hitSound = hitbox.hitSound;
		
		damage += d * DamageTakenMult;
		RestoreOptionsOnGettingHit();
		
		//GD.Print($"{this} checks if clashing");
		if(Clashing)
		{
			//GD.Print($"{this} is clashing. records data");
			ClashData = data;
			return;
		}
		
		//GD.Print($"{this} is not clashing. doing hit stuff");
		var force = (skb + damage*vkb/100f) * KnockbackTakenMult * (100f/weight);
		var stunlen = stun * StunTakenMult;
		
		if(hp > 0)
		{
			var s = States.Change<HitPauseState>();
			s.force = force;
			s.stunLength = (int)stunlen;
			s.hitPauseLength = hp;
		}
		else if(stunlen > 0)
		{
			var s = States.Change<StunState>();
			s.Force = force;
			s.stunLength = (int)stunlen;
		}
		
		if(force.x != 0f) Direction = Math.Sign(force.x);
		
		//GD.Print($"\nLast hit was {framesSinceLastHit} frames ago");
		
		if(framesSinceLastHit <= 0) ++comboCount;
		else comboCount = 0;
		//GD.Print($"Combo count is {comboCount+1}");
		
		framesSinceLastHit = 0;
		
		PlaySound(hitSound);
	}
	
	public virtual void HandleHitting(HitData data)
	{
		//GD.Print($"{this} runs Handle Hitting");
		
		Hitbox hitbox = data.Hitter;
		Hurtbox hurtbox = data.Hitee;
		
		RestoreOptionsOnHitting();
		
		//GD.Print($"{this} ensures not clashing");
		if(Clashing) return;
		//GD.Print($"{this} not currently clashing");
		
		if(hurtbox.OwnerObject is IAttacker attackerOwner && attackerOwner.Hitting && attackerOwner.LastHitee == this)
		{
			//GD.Print($"{this} detected a clash and is setting the clashing paramaters");
			Clashing = true;
			attackerOwner.Clashing = true;
			return;
		}
		
		//GD.Print($"{this} is really not clashing so is entering Hit Lag State");
		if(hitbox.hitlag > 0)
		{
			var s = States.Change<HitLagState>();
			s.hitLagLength = hitbox.hitlag;
		}
	}
	
	public virtual void HandleClashing(HitData data)
	{
		//GD.Print($"{this} gets Handle Clashing called and calls self's attack part's Handle Hits");
		GD.Print("CLASH");
		CurrentAttack?.currentPart?.HandleHits();
		var skb = data.Skb;
		var vkb = data.Vkb;
		
		var force = (skb + damage*vkb/100f) * KnockbackTakenMult * (100f/weight);
		if(force != Vector2.Zero) force = force.Normalized() * clashForce;
		var stun = clashStun;
		
		if(stun > 0)
		{
			var s = States.Change<StunState>();
			s.Force = force;
			s.stunLength = (int)stun;
		}
		
		if(force.x != 0f) Direction = Math.Sign(force.x);
		
		//PlaySound("Clash");
	}
	
	public virtual bool AttackInCooldown(Attack a) => a.sharesCooldownWith.Concat(a.Name).Any(Cooldowns.InCooldown);
	
	public virtual bool ExecuteAttack(Attack a)
	{
		if(a is null || !a.CanActivate() || AttackInCooldown(a)) return false;
		
		if(CurrentAttack != null) ResetCurrentAttack(null);
		
		CurrentAttack = a;
		CurrentAttack.Connect("AttackEnds", this, nameof(ResetCurrentAttack));
		var s = States.Get<AttackState>();
		s.att = CurrentAttack;
		States.Change("Attack");
		
		CurrentAttack.Start();
		return true;
	}
	
	public bool ExecuteAttack(string s) => ExecuteAttack(GetAttack(s));
	
	public Attack GetAttack(string s)
	{
		if(Attacks.ContainsKey(s)) return Attacks[s];
		else
		{
			GD.Print($"No attack {s} found on character {Name}");
			return null;
		}
	}
	
	public void ResetCurrentAttack(Attack a)
	{
		if(CurrentAttack is null) return;
		CurrentAttack.Disconnect("AttackEnds", this, nameof(ResetCurrentAttack));
		SetAttackCooldowns();
		CurrentAttack = null;
	}
	
	public virtual void EmitProjectile(string proj)
	{
		//get pooled projectile
		var generatedProjectile = projPool.GetProjectile(proj);
		if(generatedProjectile is null)
		{
			GD.Print($"Failed to emit projectile {proj} because the object pool returned a null");
		}
		else
		{
			if(!activeProjectiles.ContainsKey(proj))//projectile havent been used yet. create storage
				activeProjectiles.Add(proj, new HashSet<Projectile>());
			//set direction
			generatedProjectile.Direction = Direction;
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
	
	public virtual void HandleProjectileDestruction(Projectile who)
	{
		var identifier = who.identifier;
		
		if(!activeProjectiles.ContainsKey(identifier))
			throw new Exception($"Projectile {identifier} died but was never reported as active");
		
		activeProjectiles[identifier].Remove(who);
		projPool.InsertProjectile(who);
		who.Disconnect("ProjectileDied", this, nameof(HandleProjectileDestruction));
	}
	
	public virtual void SetAttackCooldowns()
	{
		Cooldowns[CurrentAttack.Name] = CurrentAttack.GetEndlag() + CurrentAttack.GetCooldown();
	}
}
