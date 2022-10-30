using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Character : KinematicBody2D,
	IStunnable, IKnockable, IHitPausable, IDamagable,
	IAttacker, IHitLaggable, IProjectileEmitter,
	IResourceUser
{
	public const int DROP_THRU_BIT = 0b1;
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
	public float FallSpeed{get; set;} = 800f;//max fall speed
	public float FastFallSpeed{get; set;} = 2000f;//max fastfall speed
	public float WallFallSpeed{get; set;} = 250f;//max wall fall speeed
	public float WallFastFallSpeed{get; set;} = 450f;//max wall fastfall speed
	public float StunFallSpeed{get; set;} = 800f;//max stun fall speed
	////////////////////////////////////////////
	public float Gravity{get; set;} = 30f;//how fast you normally fall
	public float FastFallGravity{get; set;} = 80f;//how fast you fastfall
	public float WallGravity{get; set;} = 10f;//how fast you fall on a wall
	public float WallFastFallGravity{get; set;} = 20f;//how fast you fastfall on a wall
	public float StunGravity{get; set;} = 40f;//how fast you fall during stun
	////////////////////////////////////////////
	public float GroundSpeed{get; set;} = 600f;//max ground speed
	public float AirSpeed{get; set;} = 700f;//max air speed
	public float CrawlSpeed{get; set;} = 400f;//crawling speed
	////////////////////////////////////////////
	public float GroundAcceleration{get; set;} = 50f;//how fast you reach groundSpeed
	public float AirAcceleration{get; set;} = 55f;//how fast you reach airSpeed
	////////////////////////////////////////////
	public float JumpHeight{get; set;} = 600f;//how high you jump
	public float ShorthopHeight{get; set;} = 400f;//how high you shorthop
	public float CrouchJumpHeight{get; set;} = 600f;
	public float CrouchShorthopHeight{get; set;} = 400f;
	public float AirJumpHeight{get; set;} = 800f;//how high you jump in the air
	public float HorizontalWallJump{get; set;} = 800f;//horizontal velocity from wall jumping
	public float VerticalWallJump{get; set;} = 600f;//vertical velocity from wall jumping
	public float FastfallMargin{get; set;} = -400f;
	////////////////////////////////////////////
	public int MaxClingsAllowed{get; set;} = 4;
	public int MaxAirJumpsAllowed{get; set;} = 2;
	public int MaxDodgesAllowed{get; set;} = 1;
	
	public int GivenClingsOnHitting{get; set;} = 2;
	public int GivenClingsOnGettingHit{get; set;} = 1;
	
	public int GivenAirJumpsOnHitting{get; set;} = 1;
	public int GivenAirJumpsOnGettingHit{get; set;} = 1;
	
	public int GivenDodgesOnHitting{get; set;} = 1;
	public int GivenDodgesOnGettingHit{get; set;} = 1;
	////////////////////////////////////////////
	public int ForwardRollStartup{get; set;} = 3;
	public int ForwardRollLength{get; set;} = 10;
	public float ForwardRollSpeed{get; set;} = 2000;
	public int ForwardRollEndlag{get; set;} = 4;
	public int ForwardRollCooldown{get; set;} = 30;
	
	public int BackRollStartup{get; set;} = 3;
	public int BackRollLength{get; set;} = 10;
	public float BackRollSpeed{get; set;} = 2000;
	public int BackRollEndlag{get; set;} = 4;
	public int BackRollCooldown{get; set;} = 30;
	////////////////////////////////////////////
	public int SpotAirDodgeStartup{get; set;} = 3;
	public int SpotAirDodgeLength{get; set;} = 20;
	public int SpotAirDodgeEndlag{get; set;} = 8;
	public int SpotAirDodgeCooldown{get; set;} = 120;
	
	public int DirectionalAirDodgeStartup{get; set;} = 2;
	public int DirectionalAirDodgeLength{get; set;} = 10;
	public float DirectionalAirDodgeSpeed{get; set;} = 1500;
	public int DirectionalAirDodgeEndlag{get; set;} = 5;
	public int DirectionalAirDodgeCooldown{get; set;} = 120;
	
	public int SpotGroundedDodgeStartup{get; set;} = 2;
	public int SpotGroundedDodgeLength{get; set;} = 15;
	public int SpotGroundedDodgeEndlag{get; set;} = 10;
	public int SpotGroundedDodgeCooldown{get; set;} = 120;
	////////////////////////////////////////////
	public float WavedashVelocityMutliplier{get; set;} = 1;
	public float WavedashFrictionMultiplier{get; set;} = 0.5f;
	////////////////////////////////////////////
	public int RunStartup{get; set;} = 10;
	public float RunInitialSpeed{get; set;} = 800f;
	public float RunAcceleration{get; set;} = 100f;
	public float RunSpeed{get; set;} = 700f;
	public int RunTurn{get; set;} = 5;
	public int RunJumpSquat{get; set;} = 2;
	public float RunJumpHeight{get; set;} = 600f;
	public float RunJumpSpeed{get; set;} = 1000f;
	////////////////////////////////////////////
	public bool RestoreDodgeOnGroundTouch{get; set;} = true;
	public bool RestoreDodgeOnWallTouch{get; set;} = false;
	public bool RestoreDodgeOnHitting{get; set;} = true;
	public bool RestoreDodgeOnGettingHit{get; set;} = false;
	////////////////////////////////////////////
	public float CeilingBonkBounce{get; set;} = 0.25f;//how much speed is conserved when bonking
	public float CeilingBounce{get; set;} = 0.95f;//how much speed is conserved when hitting a ceiling
	public float WallBounce{get; set;} = 0.95f;//how much speed is conserved when hitting a wall
	public float FloorBounce{get; set;} = 0.95f;//how much speed is conserved when hitting the floor
	////////////////////////////////////////////
	public float GroundFriction{get; set;} = 0.1f;//how much speed is removed over time when not moving on the ground
	public float AirFriction{get; set;} = 0.07f;//how much speed is removed over time when not moving in the air
	public float WallFriction{get; set;} = 0.6f;//how much speed is removed upon touching a wall
	public float SlopeFriction{get; set;} = 0.1f;//how much speed is removed over time when not moving on a slope
	////////////////////////////////////////////
	public int ImpactLand{get; set;} = 3;//how many frames of inactionability there are after touching the ground
	public int JumpSquat{get; set;} = 4;//how many frames before a ground jump comes out
	public int CrouchJumpSquat{get; set;} = 4;
	public int AirJumpSquat{get; set;} = 0;
	public int WallLand{get; set;} = 2;//how many frames of inactionability there are after touching a wall
	public int WallJumpSquat{get; set;} = 2;//how many frames before a wall jump comes out
	public int WalkTurn{get; set;} = 3;
	public int DuckLength{get; set;} = 2;
	public int GetupLength{get; set;} = 2;
	////////////////////////////////////////////
	public float ClashStun{get; set;} = 20f;
	public float ClashForce{get; set;} = 1000f;
	public float ClashHitLag{get; set;} = 4f;
	////////////////////////////////////////////
	public float Damage{get; set;} = 0f;
	////////////////////////////////////////////
	public float DamageTakenMult{get; set;} = 1f;
	public float KnockbackTakenMult{get; set;} = 1f;
	public float StunTakenMult{get; set;} = 1f;
	////////////////////////////////////////////
	public float DamageDoneMult{get; set;} = 1f;
	public float KnockbackDoneMult{get; set;} = 1f;
	public float StunDoneMult{get; set;} = 1f;
	////////////////////////////////////////////
	public int Direction{get; set;} = 1;//1 for right -1 for left
	
	public int TeamNumber{get; set;} = 0;
	public bool FriendlyFire{get; set;} = false;
	public int Stocks{get; set;} = 3;
	
	public float Weight{get; set;} = 100f;
	
	public int FramesSinceLastHit{get; set;} = 0;
	public int ComboCount{get; set;} = 0;
	
	public Vector2[] Velocities{get;set;} = new Vector2[8]{Vector2.Zero,Vector2.Zero,Vector2.Zero,Vector2.Zero,Vector2.Zero,Vector2.Zero,Vector2.Zero,Vector2.Zero};
	public Vector2 vec;//normal velocity from movement
	public Vector2 vac;//speed transferred from moving platforms
	public Vector2 vic;//momentary force
	public Vector2 voc;//knockback
	public Vector2 vuc;//burst movements that should only have friction applied
	public Vector2 vyc;
	public Vector2 vwc;
	public Vector2 vvc;
	
	public Vector2 Momentum{get => vec; set => vec = value;}
	public Vector2 BurstMomentum{get => vuc; set => vuc = value;}
	
	public Vector2 Velocity => Velocities.Aggregate(Vector2.Zero, (a,v)=>a+v);
	public Vector2 RoundedVelocity => Velocity.Round();
	public Vector2 RoundedPosition => Position.Round();
	
	public Vector2 FNorm{get; set;} = Vector2.Zero;//floor normal
	public Vector2 FVel{get; set;} = Vector2.Zero;//floor velocity
	public float FFric{get; set;} = 1f;//floor friction
	public float FBounce{get; set;} = 0f;//floor bounce
	
	public Vector2 WNorm{get; set;} = Vector2.Zero;//wall normal
	public Vector2 WVel{get; set;} = Vector2.Zero;//wall velocity
	public float WFric{get; set;} = 1f;//wall friction
	public float WBounce{get; set;} = 0f;//wall bounce
	
	public Vector2 CNorm{get; set;} = Vector2.Zero;//ceiling normal
	public Vector2 CVel{get; set;} = Vector2.Zero;//ceiling velocity
	public float CFric{get; set;} = 1f;//ceiling friction
	public float CBounce{get; set;} = 1f;//ceiling bounce
	
	public Vector2 Norm => Grounded?FNorm:Walled?WNorm:Ceilinged?CNorm:Vector2.Zero;
	public float CharFric => OnSlope?SlopeFriction:Grounded?GroundFriction:WallClinging?WallFriction:AirFriction;
	public float CharBounce => Grounded?FloorBounce:WallClinging?WallBounce:Ceilinged?CeilingBounce:0f;
	public Vector2 PlatVel => Grounded?FVel:WallClinging?WVel:Ceilinged?CVel:Vector2.Zero;
	public float PlatFric => Grounded?FFric:WallClinging?WFric:Ceilinged?CFric:1f;
	public float PlatBounce => Grounded?FBounce:WallClinging?WBounce:Ceilinged?CBounce:0f;
	
	public float AppropriateFriction => PlatFric * CharFric;
	public float AppropriateBounce => PlatBounce * CharBounce;
	public float AppropriateAcceleration => (Grounded?GroundAcceleration*FFric:AirAcceleration);
	public float AppropriateSpeed => (Crouching?CrawlSpeed:Grounded?(GroundSpeed*(2f-FFric)):AirSpeed);
	public float AppropriateGravity => (CurrentAttack?.CurrentPart?.GravityMultiplier ?? 1f)*((States.Current is StunState)?StunGravity:Fastfalling?WallClinging?WallFastFallGravity:FastFallGravity:WallClinging?WallGravity*(2f-WFric):Gravity);
	public float AppropriateFallingSpeed => (CurrentAttack?.CurrentPart?.GravityMultiplier ?? 1f)*((States.Current is StunState)?StunFallSpeed:Fastfalling?WallClinging?WallFastFallSpeed:FastFallSpeed:WallClinging?WallFallSpeed*(2f-WFric):FallSpeed);
	
	public bool LeftPressed => Inputs.IsActionJustPressed("Left")||Inputs.IsActionJustPressed("LLight")||Inputs.IsActionJustPressed("LSpecial");
	public bool RightPressed => Inputs.IsActionJustPressed("Right")||Inputs.IsActionJustPressed("RLight")||Inputs.IsActionJustPressed("RSpecial");
	public bool DownPressed => Inputs.IsActionJustPressed("Down")||Inputs.IsActionJustPressed("DLight")||Inputs.IsActionJustPressed("DSpecial");
	public bool UpPressed => Inputs.IsActionJustPressed("Up")||Inputs.IsActionJustPressed("ULight")||Inputs.IsActionJustPressed("USpecial");
	
	public bool LeftHeld => Inputs.IsActionPressed("Left")||Inputs.IsActionPressed("LLight")||Inputs.IsActionPressed("LSpecial");
	public bool RightHeld => Inputs.IsActionPressed("Right")||Inputs.IsActionPressed("RLight")||Inputs.IsActionPressed("RSpecial");
	public bool DownHeld => Inputs.IsActionPressed("Down")||Inputs.IsActionPressed("DLight")||Inputs.IsActionPressed("DSpecial");
	public bool UpHeld => Inputs.IsActionPressed("Up")||Inputs.IsActionPressed("ULight")||Inputs.IsActionPressed("USpecial");
	
	public bool LeftReleased => Inputs.IsActionJustReleased("Left")||Inputs.IsActionJustReleased("LLight")||Inputs.IsActionJustReleased("LSpecial");
	public bool RightReleased => Inputs.IsActionJustReleased("Right")||Inputs.IsActionJustReleased("RLight")||Inputs.IsActionJustReleased("RSpecial");
	public bool DownReleased => Inputs.IsActionJustReleased("Down")||Inputs.IsActionJustReleased("DLight")||Inputs.IsActionJustReleased("DSpecial");
	public bool UpReleased => Inputs.IsActionJustReleased("Up")||Inputs.IsActionJustReleased("ULight")||Inputs.IsActionJustReleased("USpecial");
	
	public int InputDirection => RightInput?(LeftInput?0:1):(LeftInput?-1:0);
	public int MovementDirection => (HoldingStrafe && InputDirection != 0)?InputDirection:Direction;
	public int FutureDirection => (InputDirection == 0)?Direction:InputDirection;
	public Vector2 InputVector => new Vector2((RightInput?1:LeftInput?-1:0),(DownInput?1:UpInput?-1:0)).Normalized();
	
	public bool InputtingTurn => !HoldingStrafe && (FutureDirection != Direction);
	public bool InputtingHorizontalDirection => LeftInput||RightInput;
	public bool NowInputtingHorizontalDirection => Inputs.IsActionJustPressed("Left")||Inputs.IsActionJustPressed("Right");
	public bool InputtingVerticalDirection => UpInput||DownInput;
	public bool NowInputtingVerticalDirection => Inputs.IsActionJustPressed("Up")||Inputs.IsActionJustPressed("Down");
	public bool InputtingDirection => InputtingHorizontalDirection||InputtingVerticalDirection;
	public bool NowInputtingDirection => NowInputtingHorizontalDirection||NowInputtingVerticalDirection;
	
	public bool Idle => (Math.Truncate(Velocity.x / 100f) == 0);
	public bool Still => (Idle && !InputtingHorizontalDirection);
	
	public string AttackDirPrefix => UpInput?"U":DownInput?"D":LeftInput?"L":RightInput?"R":"N";
	
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
	
	public static readonly string[] INPUT_DIRS = {"U", "D", "L", "R", "N", ""};
	public static readonly string[] ATTACK_TYPES = {"Light", "Special"};
	public bool InputtingAttack(string type) => INPUT_DIRS.Any(s => Inputs.IsActionJustPressed($"{s}{type}"));
	public bool InputtingLight => INPUT_DIRS.Any(s => Inputs.IsActionJustPressed($"{s}Light"));
	public bool InputtingSpecial => INPUT_DIRS.Any(s => Inputs.IsActionJustPressed($"{s}Special"));
	public bool InputtingTaunt => Inputs.IsActionJustPressed("Taunt");
	public bool InputtingAnyAttack => ATTACK_TYPES.Any(InputtingAttack) || InputtingTaunt;
	
	public string AttackInputDir(string type)
	{
		var input = INPUT_DIRS.FirstOrDefault(s => Inputs.IsActionJustPressed($"{s}{type}"), "_");
		if(input == "_") return "";
		if(input == "") input = AttackDirPrefix;
		return input;
	}
	
	public bool Fastfalling{get; set;} = false;//wether or not fastfalling
	
	public bool Grounded{get; set;} = false;//is on ground
	public bool Walled{get; set;} = false;//is on wall
	public bool WallClinging{get; set;} = false;//is clinging to a wall
	public bool Ceilinged{get; set;} = false;//is touching ceiling
	public bool Aerial{get; set;} = false;//is in air
	public bool OnSemiSolid{get; set;} = false;//is currently on a semi solid platform
	public bool OnSlope{get; set;} = false;//is currently on a slope
	
	public bool LeftInput{get; set;} = false;//is left currently held
	public bool RightInput{get; set;} = false;//is right currently held
	public bool DownInput{get; set;} = false;//is down currently held
	public bool UpInput{get; set;} = false;//is up currently held
	
	public bool Crouching{get; set;} = false;//is currently crouching
	
	public StateMachine States{get; set;}
	
	public List<Hurtbox> Hurtboxes{get; set;} = new List<Hurtbox>();
	
	public string CurrentCollisionSetting{get; set;}
	public CharacterCollision Collision{get; set;}
	
	public Attack CurrentAttack{get; set;}
	
	public bool Hitting{get; set;}
	public IHittable LastHitee{get; set;}
	public bool GettingHit{get; set;}
	public IHitter LastHitter{get; set;}
	
	public bool Clashing{get; set;}
	public HitData ClashData{get; set;}
	
	public int StunFrames{get; set;} = 0;
	public int HitPauseFrames{get; set;} = 0;
	public int HitLagFrames{get; set;} = 0;
	public Vector2 Knockback{get; set;} = Vector2.Zero;
	
	public Dictionary<string, Attack> Attacks{get; set;} = new Dictionary<string, Attack>();
	
	public CooldownManager Cooldowns{get; set;} = new CooldownManager();
	
	public InvincibilityManager IFrames{get; set;} = new InvincibilityManager();
	public bool Invincible => IFrames.Count > 0;
	
	public ResourceManager Resources{get; set;} = new ResourceManager();
	
	public TimedActionsManager TimedActions{get; set;} = new TimedActionsManager();
	
	public string[] RelevantInputTags{get; set;}
	public StateTagsManager Tags{get; set;} = new StateTagsManager();
	
	public Dictionary<string, HashSet<Projectile>> ActiveProjectiles{get; set;} = new Dictionary<string, HashSet<Projectile>>();
	public ProjectilePool ProjPool{get; set;}
	
	public List<string> StatList{get; set;} = new List<string>();
	public PropertyMap prop = new PropertyMap();
	
	public InputManager Inputs{get; set;}
	
	public AnimationSprite CharacterSprite{get; set;} = new AnimationSprite();
	public Color SpriteModulate{get; set;}
	
	public string AudioPrefix => Name;
	public AudioManager Audio{get; set;}
	
	public AnimationPlayer HitboxAnimator{get; set;} = new AnimationPlayer();
	
	public Character()
	{
		ProjPool = new ProjectilePool(this);
		States = new StateMachine(CreateStates());
	}
	
	public override void _Ready()
	{
		HitboxAnimator.PlaybackProcessMode = AnimationPlayer.AnimationProcessMode.Physics;
		HitboxAnimator.Name = "HitboxAnimator";
		AddChild(HitboxAnimator);
		
		CharacterSprite.Name = "CharacterSprite";
		AddChild(CharacterSprite);
		
		Inputs.Name = "InputManager";
		AddChild(Inputs);
		
		CollisionLayer = 0b100;
		CollisionMask = 0b011;
		
		RelevantInputTags = InputMap.GetActions().ToEnumerable<string>().LeftQuotient(Inputs.InputPrefix).ToArray();
		
		ProjPool.LoadInitialProjectiles();
	}
	
	private static readonly Color Black = new Color(1,1,1,1);
	public override void _PhysicsProcess(float delta)
	{
		if(Clashing) HandleClashing(ClashData);
		Clashing = false;
		Hitting = false;
		GettingHit = false;
		
		++FramesSinceLastHit;
		StoreVelocities();
		TruncateVelocityIfInsignificant();
		
		ProjPool.Update();
		Cooldowns.Update();
		IFrames.Update();
		TimedActions.Update();
		Tags.Update();
		UpdateTags();
		States.Update(delta);
		
		if(Input.IsActionJustPressed("reset")) Respawn();
		
		CharacterSprite.FlipH = (Direction == -1);
		CharacterSprite.SelfModulate = Invincible?Black:SpriteModulate;
	}
	
	public override void _ExitTree()
	{
		ProjPool.Clear();
	}
	
	#region Animation
	public void PlayAnimation(string anm, bool overwriteQueue) => CharacterSprite.Play(anm, overwriteQueue);
	public void QueueAnimation(string anm, bool goNext, bool overwriteQueue) => CharacterSprite.Queue(anm, goNext, overwriteQueue);
	public bool AnimationLooping => CharacterSprite.Looping;
	public void PauseAnimation() => CharacterSprite.Pause();
	public void ResumeAnimation() => CharacterSprite.Resume();

	#endregion
	
	#region Sound
	public void PlaySound(string sound, Vector2 pos) => Audio.Play(AudioPrefix, sound, pos);
	public void PlaySound(AudioStream sound, Vector2 pos) => Audio.Play(sound, pos);

	#endregion
	
	#region States
	private State[] CreateStates() => new State[]
	{
		new AirState(this),
		new AirJumpState(this),
		
		new WallState(this),
		new WallLandState(this),
		new WallJumpState(this),
		
		new GroundedState(this),
		new LandState(this),
		new JumpState(this),
		
		new IdleState(this),
		new WalkState(this),
		new WalkTurnState(this),
		new WalkStopState(this),
		new WalkWallState(this),
		
		new GetupState(this),
		new DuckState(this),
		new CrouchState(this),
		new CrawlState(this),
		new CrawlWallState(this),
		new CrouchJumpState(this),
		
		new StunState(this),
		new HitPauseState(this),
		new HitLagState(this),
		new AttackState(this),
		new EndlagState(this),
		
		new SpotAirDodgeState(this),
		new DirectionalAirDodgeState(this),
		new SpotGroundedDodgeState(this),
		
		new RunStartupState(this),
		new RunState(this),
		new RunStopState(this),
		new RunJumpState(this),
		new RunTurnState(this),
		new RunWallState(this),
		
		new WavedashState(this)
	};
	#endregion

	#region Tags
	public void UpdateTags()
	{
		foreach(var inputTag in RelevantInputTags)
		{
			if(Inputs.IsActionJustPressed(inputTag)) Tags[inputTag] = StateTag.Starting;
			if(Inputs.IsActionJustReleased(inputTag)) Tags[inputTag] = StateTag.Ending;
		}
		
		
		UpdateTag("Aerial", Aerial);
		UpdateTag("Grounded", Grounded);
		UpdateTag("Walled", Walled);
		UpdateTag("WallClinging", WallClinging);
		UpdateTag("Ceilinged", Ceilinged);
		UpdateTag("SemiSolid", OnSemiSolid);
		UpdateTag("Slope", OnSlope);
		
		
		UpdateTag("Turning", InputtingTurn);
		
		UpdateTag("LeftInput", LeftInput);
		UpdateTag("RightInput", RightInput);
		UpdateTag("DownInput", DownInput);
		UpdateTag("UpInput", UpInput);
		
		UpdateTag("HorizontalInput", InputtingHorizontalDirection);
		UpdateTag("VerticalInput", InputtingVerticalDirection);
		UpdateTag("DirectionalInput", InputtingDirection);
		
		
		UpdateTag("Fastfalling", Fastfalling);
	}
	
	public void UpdateTag(string name, bool property)
	{
		if(property && !Tags.Active(name)) Tags[name] = StateTag.Starting;
		if(!property && Tags.Active(name)) Tags[name] = StateTag.Ending;
	}
	#endregion

	#region Misc	

	public override string ToString() => Name;
	
	public void AttachEffect(Effect e)
	{
		AddChild(e);
		//TODO: make the effect inserted into a list
		//so that other scripts can reference it
		//just make sure to remove
	}
	public void GiveTemporaryResource(string resource, int amount, int forFrames)
	{
		Resources.Give(resource, amount);
		TimedActions.Add($"Resource{resource}Expire", forFrames, () => {Resources[resource] = 0;});
	}
	
	public void Die()
	{
		GD.Print($"Character {this} died with {Stocks} stocks");
		--Stocks;
		if(Stocks <= 0)
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
		if(CurrentAttack != null) CurrentAttack.Active = false;
		ApplySettings("Default");
		States.Change("Air");
		Position = 10f * Vector2.Left * TeamNumber;//cheap way to offset spawning a bit
		IFrames.Clear();//todo: respawn i-frames
		Fastfalling = false;
		Crouching = false;
		ResetVelocity();
		Direction = 1;
		Damage = 0f;
		FramesSinceLastHit = 0;
		ComboCount = 0;
		DisableAllProjectiles();
	}
	#endregion
	
	#region Projectiles
	public virtual void DisableAllProjectiles()
	{
		foreach(var activeProjectileSet in ActiveProjectiles.Values)
			activeProjectileSet.ToList().ForEach(p => p.Destruct());
	}
	public virtual void EmitProjectile(string proj)
	{
		//get pooled projectile
		var generatedProjectile = ProjPool.GetProjectile(proj);
		if(generatedProjectile is null)
		{
			GD.PushError($"Failed to emit projectile {proj} because the object pool returned a null");
			return;
		}
		
		ActiveProjectiles.TryAdd(proj, new HashSet<Projectile>());
		//set direction
		generatedProjectile.Direction = Direction;
		//connect destruction signal
		generatedProjectile.Connect("ProjectileDied", this, nameof(HandleProjectileDestruction));
		//store as active
		ActiveProjectiles[proj].Add(generatedProjectile);
		//request that _Ready will be called
		generatedProjectile.RequestReady();
		//add to scene
		GetParent().AddChild(generatedProjectile);
	}
	
	public virtual void HandleProjectileDestruction(Projectile who)
	{
		var identifier = who.Identifier;
		ActiveProjectiles[identifier].Remove(who);
		ProjPool.InsertProjectile(who);
		who.Disconnect("ProjectileDied", this, nameof(HandleProjectileDestruction));
	}
	#endregion

	#region Resources
	public virtual void RestoreOptionsOnGroundTouch()
	{
		EmitSignal(nameof(OptionsRestoredFromGroundTouch));
		
		Resources["Clings"] = MaxClingsAllowed;
		Resources["AirJumps"] = MaxAirJumpsAllowed;
		Resources["OnHittingOptionRestoration"] = 1;
		Resources["OnGettingHitOptionRestoration"] = 1;
		if(RestoreDodgeOnGroundTouch) Resources["Dodge"] = MaxDodgesAllowed;
	}
	
	public virtual void RestoreOptionsOnWallTouch()
	{
		EmitSignal(nameof(OptionsRestoredFromWallTouch));
		
		Resources.Give("Clings", -1);
		Resources["AirJumps"] = MaxAirJumpsAllowed;
		if(RestoreDodgeOnWallTouch) Resources["Dodge"] = MaxDodgesAllowed;
	}
	
	public virtual void RestoreOptionsOnHitting()
	{
		EmitSignal(nameof(OptionsRestoredFromHitting));
		
		if(Resources.Has("OnHittingOptionRestoration"))
		{
			Resources.Give("Clings", GivenClingsOnHitting, MaxClingsAllowed);
			Resources.Give("AirJumps", GivenAirJumpsOnHitting, MaxAirJumpsAllowed);
			if(RestoreDodgeOnHitting) Resources.Give("Dodge", GivenDodgesOnHitting, MaxDodgesAllowed);
			Resources.Give("OnHittingOptionRestoration", -1);
		}
	}
	
	public virtual void RestoreOptionsOnGettingHit()
	{
		EmitSignal(nameof(OptionsRestoredFromGettingHit));
		
		if(Resources.Has("OnGettingHitOptionRestoration"))
		{
			Resources.Give("Clings", GivenClingsOnGettingHit, MaxClingsAllowed);
			Resources.Give("AirJumps", GivenAirJumpsOnGettingHit, MaxAirJumpsAllowed);
			if(RestoreDodgeOnGettingHit) Resources.Give("Dodge", GivenDodgesOnGettingHit, MaxDodgesAllowed);
			Resources.Give("OnGettingHitOptionRestoration", -1);
		}
	}
	#endregion
	
	#region Velocity
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
	#endregion
	
	#region Collision

	public void ApplySettings(string setting)
	{
		CurrentCollisionSetting = setting;
		Collision.ChangeState(setting);
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
		
		var trav = new Vector2(0, (FVel.y < 0)?FVel.y:0);
		
		query.Transform = query.Transform.Translated(CF*FNorm + trav);
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
		Crouching = true;
	}
	
	//uncrouches
	public void Uncrouch()
	{
		ApplySettings("Default");
		Crouching = false;
	}

	#endregion
	
	#region Inputs
	
	public void Turn() => Direction *= -1;
	
	public bool TurnConditional()
	{
		if(InputtingTurn) Turn();
		else return false;
		
		return true;
	}

	#endregion

	#region Attacks	
	public virtual bool CanGenerallyHit(IHittable hitObject) => !hitObject.Invincible;
	public virtual bool CanGenerallyBeHitBy(IHitter hitter) => !Invincible;
	public virtual bool CanGenerallyBeHitBy(IAttacker attacker) => !Invincible;
	
	public bool CanHit(IHittable hitObject) => CanGenerallyHit(hitObject)&&hitObject.CanGenerallyBeHitBy(this);
	public bool CanBeHitBy(IHitter hitter) => CanGenerallyBeHitBy(hitter)&&hitter.CanGenerallyHit(this);
	public bool CanBeHitBy(IAttacker attacker) => CanGenerallyBeHitBy(attacker)&&attacker.CanGenerallyHit(this);
	
	public virtual void HandleGettingHit(HitData data)
	{
		#if DEBUG_ATTACKS
		GD.Print($"{this} runs Handle Getting Hit");
		#endif
		
		var skb = data.SKB;
		var vkb = data.VKB;
		var d = data.Damage;
		var sstun = data.SStun;
		var vstun = data.VStun;
		var shp = data.SHitPause;
		var vhp = data.VHitPause;
		Hitbox hitbox = data.Hitter;
		Hurtbox hurtbox = data.Hitee;
		var hitSound = hitbox.HitSound;
		
		Damage += d * DamageTakenMult;
		RestoreOptionsOnGettingHit();
		
		#if DEBUG_ATTACKS
		GD.Print($"{this} checks if clashing");
		#endif
		
		if(Clashing)
		{
			#if DEBUG_ATTACKS
			GD.Print($"{this} is clashing. records data");
			#endif
			
			ClashData = data;
			return;
		}
		
		#if DEBUG_ATTACKS
		GD.Print($"{this} is not clashing. doing hit stuff");
		#endif
		
		StunFrames = (int)((sstun + Damage*vstun/100f) * StunTakenMult);
		Knockback = (skb + Damage*vkb/100f) * KnockbackTakenMult * (100f/Weight);
		HitPauseFrames = (int)(shp + Damage*vhp/100f);
		
		if(HitPauseFrames > 0) States.Change<HitPauseState>();
		else if(StunFrames > 0) States.Change<StunState>();
		
		if(Knockback.x != 0f) Direction = Math.Sign(Knockback.x);
		
		if(FramesSinceLastHit <= 0) ++ComboCount;
		else ComboCount = 0;
		
		FramesSinceLastHit = 0;
		
		PlaySound(hitSound, Position);
	}
	
	public virtual void HandleHitting(HitData data)
	{
		#if DEBUG_ATTACKS
		GD.Print($"{this} runs Handle Hitting");
		#endif
		
		Hitbox hitbox = data.Hitter;
		Hurtbox hurtbox = data.Hitee;
		
		RestoreOptionsOnHitting();
		
		#if DEBUG_ATTACKS
		GD.Print($"{this} ensures not clashing");
		#endif
		
		if(Clashing) return;
		
		#if DEBUG_ATTACKS
		GD.Print($"{this} not currently clashing");
		#endif
		
		if(hurtbox.OwnerObject is IAttacker attackerOwner && attackerOwner.Hitting && attackerOwner.LastHitee == this)
		{
			#if DEBUG_ATTACKS
			GD.Print($"{this} detected a clash and is setting the clashing paramaters");
			#endif
			
			Clashing = true;
			attackerOwner.Clashing = true;
			return;
		}
		
		#if DEBUG_ATTACKS
		GD.Print($"{this} is really not clashing so is entering Hit Lag State");
		#endif
		
		var dam = ((hurtbox.OwnerObject is IDamagable damagable)?damagable.Damage:0) + data.Damage;
		HitLagFrames = (int)(data.SHitLag + dam*data.VHitLag/100f);
		
		if(HitLagFrames > 0) States.Change<HitLagState>();
	}
	
	public virtual void HandleClashing(HitData data)
	{
		#if DEBUG_ATTACKS
		GD.Print($"{this} gets Handle Clashing called and calls self's attack part's Handle Hits");
		#endif
		
		GD.Print("CLASH");
		CurrentAttack?.CurrentPart?.HandleHits();
		
		Knockback = (data.SKB + Damage*data.VKB/100f);
		Knockback = Knockback.NormalizedSafe() * ClashForce;
		
		StunFrames = (int)ClashStun;
		HitPauseFrames = (int)ClashHitLag;
		if(HitPauseFrames > 0) States.Change<HitPauseState>();
		else if(StunFrames > 0) States.Change<StunState>();
		
		if(Knockback.x != 0f) Direction = Math.Sign(Knockback.x);
	}
	
	public virtual bool AttackInCooldown(Attack a) => a.SharesCooldownWith.Append(a.Name).Any(Cooldowns.InCooldown);
	
	public virtual bool ExecuteAttack(Attack a)
	{
		if(a is null || !a.CanActivate() || AttackInCooldown(a)) return false;
		
		ResetCurrentAttack(CurrentAttack);
		
		CurrentAttack = a;
		CurrentAttack.Connect("AttackEnds", this, nameof(ResetCurrentAttack));
		States.Change("Attack");
		
		CurrentAttack.Active = true;
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
		if(!GettingHit) SetAttackCooldowns();
		CurrentAttack = null;
	}
	public virtual void SetAttackCooldowns()
	{
		if(CurrentAttack is null) return;
		Cooldowns[CurrentAttack.Name] = CurrentAttack.FinalCooldown;
	}

	#endregion
}
