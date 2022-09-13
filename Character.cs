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
	////////////////////////////////////////////
	public int spotAirDodgeStartup;
	public int spotAirDodgeLength;
	public int spotAirDodgeEndlag;
	
	public int directionalAirDodgeStartup;
	public int directionalAirDodgeLength;
	public float directionalAirDodgeSpeed;
	public int directionalAirDodgeEndlag;
	
	public int spotGroundedDodgeStartup;
	public int spotGroundedDodgeLength;
	public int spotGroundedDodgeEndlag;
	public int spotGroundedDodgeCooldown;
	////////////////////////////////////////////
	public float wavedashVelocityMutliplier = 1.5f;
	public float wavedashFrictionMultiplier = 0.5f;
	////////////////////////////////////////////
	public int runStartup = 2;
	public float runInitialSpeed = 600f;
	public float runAcceleration = 60f;
	public float runSpeed = 700f;
	public int runTurn = 3;
	public int runJumpSquat = 1;
	public float runJumpHeight = 450f;
	public float runJumpSpeed = 550f;
	////////////////////////////////////////////
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
	
	public Vector2[] Velocities{get;set;} = new Vector2[8]{Vector2.Zero,Vector2.Zero,Vector2.Zero,Vector2.Zero,Vector2.Zero,Vector2.Zero,Vector2.Zero,Vector2.Zero};
	public Vector2 vec;//normal velocity from movement
	public Vector2 vac;//speed transferred from moving platforms
	public Vector2 vic;//momentary force
	public Vector2 voc;//knockback
	public Vector2 vuc;//burst movements that should only have friction applied
	public Vector2 vyc;
	public Vector2 vwc;
	public Vector2 vvc;
	
	public Vector2 Velocity => Velocities.Aggregate(Vector2.Zero, (a,v)=>a+v);
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
	public float AppropriateAcceleration => (grounded?groundAcceleration*ffric:airAcceleration);
	public float AppropriateSpeed => (crouching?crawlSpeed:grounded?(groundSpeed*(2-ffric)):airSpeed);
	public float AppropriateGravity => (CurrentAttack?.CurrentPart?.GravityMultiplier ?? 1f)*((States.Current is StunState)?stunGravity:fastfalling?walled?wallFastFallGravity:fastFallGravity:walled?wallGravity*(2-wfric):gravity);
	public float AppropriateFallingSpeed => (CurrentAttack?.CurrentPart?.GravityMultiplier ?? 1f)*((States.Current is StunState)?stunFallSpeed:fastfalling?walled?wallFastFallSpeed:fastFallSpeed:walled?wallFallSpeed*(2-wfric):fallSpeed);
	
	public int InputDirection => rightHeld?(leftHeld?0:1):(leftHeld?-1:0);
	public int MovementDirection => (HoldingStrafe && InputDirection != 0)?InputDirection:Direction;
	public int FutureDirection => (InputDirection == 0)?Direction:InputDirection;
	public Vector2 InputVector => new Vector2((rightHeld?1:leftHeld?-1:0),(downHeld?1:upHeld?-1:0)).Normalized();
	
	public bool InputtingTurn => !HoldingStrafe && (FutureDirection != Direction);
	public bool InputtingHorizontalDirection => leftHeld||rightHeld;
	public bool NowInputtingHorizontalDirection => Inputs.IsActionJustPressed("Left")||Inputs.IsActionJustPressed("Right");
	public bool InputtingVerticalDirection => upHeld||downHeld;
	public bool NowInputtingVerticalDirection => Inputs.IsActionJustPressed("Up")||Inputs.IsActionJustPressed("Down");
	public bool InputtingDirection => InputtingHorizontalDirection||InputtingVerticalDirection;
	public bool NowInputtingDirection => NowInputtingHorizontalDirection||NowInputtingVerticalDirection;
	
	public bool Idle => (Math.Truncate(Velocity.x / 100f) == 0);
	public bool Still => (Idle && !InputtingHorizontalDirection);
	public string AttackDirPrefix => upHeld?"U":downHeld?"D":leftHeld?"S":rightHeld?"S":"N";
	public bool InputtingDodge => Inputs.IsActionJustPressed("NDodge") || Inputs.IsActionJustPressed("Dodge");
	public bool InputtingNatDodge => Inputs.IsActionJustPressed("NDodge") || (!InputtingDirection && Inputs.IsActionJustPressed("Dodge"));
	public bool InputtingJump => Inputs.IsActionJustPressed("Jump");
	public bool HoldingJump => Inputs.IsActionPressed("Jump");
	public bool InputtingRun => Inputs.IsActionJustPressed("Run");
	public bool HoldingRun => Inputs.IsActionPressed("Run");
	public bool ReleasingRun => Inputs.IsActionJustReleased("Run");
	public bool InputtingStrafe => Inputs.IsActionJustPressed("Strafe");
	public bool HoldingStrafe => Inputs.IsActionPressed("Strafe");
	public bool ReleasingStrafe => Inputs.IsActionJustReleased("Strafe");
	
	public bool ShouldInitiateRun => (HoldingRun&&NowInputtingHorizontalDirection) || (InputtingHorizontalDirection&&InputtingRun);
	
	public static readonly string[] INPUT_DIRS = {"U", "D", "S", "N", ""};
	public static readonly string[] ATTACK_TYPES = {"Light", "Special", "Taunt"};
	public bool InputtingAttack(string type) => INPUT_DIRS.Any(s => Inputs.IsActionJustPressed($"{s}{type}"));
	public bool InputtingLight => INPUT_DIRS.Any(s => Inputs.IsActionJustPressed($"{s}Light"));
	public bool InputtingSpecial => INPUT_DIRS.Any(s => Inputs.IsActionJustPressed($"{s}Heavy"));
	public bool InputtingTaunt => INPUT_DIRS.Any(s => Inputs.IsActionJustPressed($"{s}Taunt"));
	public bool InputtingAnyAttack => ATTACK_TYPES.Any(InputtingAttack);
	
	public string AttackInputDir(string type)
	{
		var input = INPUT_DIRS.FirstOrDefault(s => Inputs.IsActionJustPressed($"{s}{type}"), "_");
		if(input == "_") return "";
		if(input == "") input = AttackDirPrefix;
		return input;
	}
	
	public bool fastfalling = false;//wether or not fastfalling
	
	public bool grounded = false;//is on ground
	public bool walled = false;//is on wall
	public bool ceilinged = false;//is touching ceiling
	public bool aerial = false;//is in air
	public bool onSemiSolid = false;//is currently on a semi solid platform
	public bool onSlope = false;//is currently on a slope
	
	public bool leftHeld = false;//is left currently held
	public bool rightHeld = false;//is right currently held
	public bool downHeld = false;//is down currently held
	public bool upHeld = false;//is up currently held
	
	public bool crouching = false;//is currently crouching
	
	
	public StateMachine States{get; set;}
	
	public List<Hurtbox> Hurtboxes{get; set;} = new List<Hurtbox>();
	
	public string currentCollisionSetting;
	public CharacterCollision collision;
	//public PlatformDropDetector DropDetector;
	
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
	
	public TimedActionsManager TimedActions{get; set;} = new TimedActionsManager();
	
	public StateTagsManager Tags{get; set;} = new StateTagsManager();
	
	public Dictionary<string, PackedScene> projectiles = new Dictionary<string, PackedScene>();
	public Dictionary<string, HashSet<Projectile>> activeProjectiles = new Dictionary<string, HashSet<Projectile>>();
	public ProjectilePool projPool;
	
	public List<string> StatList = new List<string>();
	public PropertyMap prop = new PropertyMap();
	
	public InputManager Inputs;
	
	public AnimationSprite CharacterSprite{get; set;}
	public Color SpriteModulate{get; set;}
	
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
		TimedActions.Update();
		Tags.Update();
		UpdateInputTags();
		States.Update(delta);
		if(Input.IsActionJustPressed("reset")) Respawn();
		
		CharacterSprite.FlipH = (Direction == -1);
		CharacterSprite.SelfModulate = Invincible?new Color(1,1,1,1):SpriteModulate;
		Update();
	}
	
	public override void _Draw()
	{
		if(!this.GetRootNode<UpdateScript>("UpdateScript").debugCollision) return;
		DrawCircle(Vector2.Zero, 5, new Color(0,0,0,1));
	}
	
	public void PlayAnimation(string anm, bool overwriteQueue) => CharacterSprite.Play(anm, overwriteQueue);
	public void QueueAnimation(string anm, bool goNext, bool overwriteQueue) => CharacterSprite.Queue(anm, goNext, overwriteQueue);
	public bool AnimationLooping => CharacterSprite.Looping;
	public void PauseAnimation() => CharacterSprite.Pause();
	public void ResumeAnimation() => CharacterSprite.Resume();
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
		yield return new SpotGroundedDodgeState(this);
		
		yield return new RunStartupState(this);
		yield return new RunState(this);
		yield return new RunStopState(this);
		yield return new RunJumpState(this);
		yield return new RunTurnState(this);
		yield return new RunWallState(this);
		
		yield return new WavedashState(this);
	}
	
	///////////////////////////////////////////
	///////////////Misc////////////////////////
	///////////////////////////////////////////
	
	public void UpdateInputTags()
	{
		foreach(var inputTag in InputMap.GetActions().Enumerable<string>().Where(s=>s.StartsWith($"{TeamNumber}_")).Select(s=>s.Substring($"{TeamNumber}_".Length)))
		{
			if(Inputs.IsActionJustPressed(inputTag)) Tags[inputTag] = StateTag.Starting;
			if(Inputs.IsActionJustReleased(inputTag)) Tags[inputTag] = StateTag.Ending;
		}
	}
	
	public void GiveTemporaryResource(string resource, int amount, int forFrames)
	{
		Resources.Give(resource, amount);
		TimedActions.Add($"Resource{resource}Expire", forFrames, () => {Resources[resource] = 0;});
	}
	
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
		Position = 10f * Vector2.Left * TeamNumber;
		IFrames.Clear();//todo: respawn i-frames
		fastfalling = false;
		crouching = false;
		ResetVelocity();
		Direction = 1;
		damage = 0f;
		framesSinceLastHit = 0;
		comboCount = 0;
		DisableAllProjectiles();
	}
	
	public virtual void DisableAllProjectiles()
	{
		foreach(var activeProjectileSet in activeProjectiles.Values)
			activeProjectileSet.ToList().ForEach(p => p.Destruct());
	}
	
	public virtual void RestoreOptionsOnGroundTouch()
	{
		EmitSignal(nameof(OptionsRestoredFromGroundTouch));
		
		Resources["Clings"] = maxClingsAllowed;
		Resources["AirJumps"] = maxAirJumpsAllowed;
		Resources["OnHittingOptionRestoration"] = 1;
		Resources["OnGettingHitOptionRestoration"] = 1;
		if(restoreDodgeOnGroundTouch) Resources["Dodge"] = maxDodgesAllowed;
	}
	
	public virtual void RestoreOptionsOnWallTouch()
	{
		EmitSignal(nameof(OptionsRestoredFromWallTouch));
		
		Resources.Give("Clings", -1);
		Resources["AirJumps"] = maxAirJumpsAllowed;
		if(restoreDodgeOnWallTouch) Resources["Dodge"] = maxDodgesAllowed;
	}
	
	public virtual void RestoreOptionsOnHitting()
	{
		EmitSignal(nameof(OptionsRestoredFromHitting));
		
		if(Resources.Has("OnHittingOptionRestoration"))
		{
			Resources.Give("Clings", givenClingsOnHitting, maxClingsAllowed);
			Resources.Give("AirJumps", givenAirJumpsOnHitting, maxAirJumpsAllowed);
			if(restoreDodgeOnHitting) Resources.Give("Dodge", givenDodgesOnHitting, maxDodgesAllowed);
			Resources.Give("OnHittingOptionRestoration", -1);
		}
	}
	
	public virtual void RestoreOptionsOnGettingHit()
	{
		EmitSignal(nameof(OptionsRestoredFromGettingHit));
		
		if(Resources.Has("OnGettingHitOptionRestoration"))
		{
			Resources.Give("Clings", givenClingsOnGettingHit, maxClingsAllowed);
			Resources.Give("AirJumps", givenAirJumpsOnGettingHit, maxAirJumpsAllowed);
			if(restoreDodgeOnGettingHit) Resources.Give("Dodge", givenDodgesOnGettingHit, maxDodgesAllowed);
			Resources.Give("OnGettingHitOptionRestoration", -1);
		}
	}
	
	public virtual void StoreVelocities()
	{
		Velocities[0] = vec; Velocities[1] = vac; Velocities[2] = vic; Velocities[3] = voc;
		Velocities[4] = vuc; Velocities[5] = vyc; Velocities[6] = vwc; Velocities[7] = vvc;
	}
	
	public virtual void LoadVelocities()
	{
		vec = Velocities[0]; vac = Velocities[1]; vic = Velocities[2]; voc = Velocities[3];
		vuc = Velocities[4]; vyc = Velocities[5]; vwc = Velocities[6]; vvc = Velocities[7];
	}
	
	public virtual void ResetVelocity()
	{
		for(int i = 0; i < Velocities.Length; ++i) Velocities[i] = Vector2.Zero;
		LoadVelocities();
	}
	
	public virtual void TruncateVelocityIfInsignificant()
	{
		for(int i = 0; i < Velocities.Length; ++i) Velocities[i].TruncateIfInsignificant();
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
		//DropDetector.UpdateBasedOnCollisionShape();
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
		var skb = data.SKB;
		var vkb = data.VKB;
		var d = data.Damage;
		var sstun = data.SStun;
		var vstun = data.VStun;
		var shp = data.SHitpause;
		var vhp = data.VHitpause;
		Hitbox hitbox = data.Hitter;
		Hurtbox hurtbox = data.Hitee;
		var hitSound = hitbox.HitSound;
		
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
		var stunlen = (sstun + damage*vstun/100f) * StunTakenMult;
		var hp = (shp + damage*vhp/100f);
		
		if(hp > 0)
		{
			var s = States.Change<HitPauseState>();
			s.force = force;
			s.stunLength = (int)stunlen;
			s.hitPauseLength = (int)hp;
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
		if(hitbox.Hitlag > 0)
		{
			var s = States.Change<HitLagState>();
			s.hitLagLength = (int)hitbox.Hitlag;
		}
	}
	
	public virtual void HandleClashing(HitData data)
	{
		//GD.Print($"{this} gets Handle Clashing called and calls self's attack part's Handle Hits");
		GD.Print("CLASH");
		CurrentAttack?.CurrentPart?.HandleHits();
		var skb = data.SKB;
		var vkb = data.VKB;
		
		var force = (skb + damage*vkb/100f);
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
	
	public virtual bool AttackInCooldown(Attack a) => a.SharesCooldownWith.Append(a.Name).Any(Cooldowns.InCooldown);
	
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
			GD.PushError($"No attack {s} found on character {Name}");
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
			GD.PushError($"Failed to emit projectile {proj} because the object pool returned a null");
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
		var identifier = who.Identifier;
		
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
