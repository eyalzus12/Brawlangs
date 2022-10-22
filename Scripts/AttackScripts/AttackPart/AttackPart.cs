using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class AttackPart : Node2D, IHitter
{
	public long FrameCount{get; set;} = 0;
	
	public List<Hitbox> Hitboxes{get; set;} = new List<Hitbox>();
	public HashSet<IHittable> HitIgnoreList{get; set;} = new HashSet<IHittable>();
	public Dictionary<Hurtbox, Hitbox> HitList{get; set;} = new Dictionary<Hurtbox, Hitbox>();
	
	public Dictionary<string, ParamRequest> LoadExtraProperties{get; set;} = new Dictionary<string, ParamRequest>();
	
	public int Direction {get => OwnerObject.Direction; set => OwnerObject.Direction = value;}
	public int TeamNumber {get => OwnerObject.TeamNumber; set => OwnerObject.TeamNumber = value;}
	
	protected bool _active = false;
	public virtual bool Active{get => _active; set
	{
		SetPhysicsProcess(value);
		if(_active && !value) Stop();
		else if(!_active && value) Activate();
		_active = value;
	}}
	
	public int Startup{get; set;}
	public long Length{get; set;}
	public Vector2 Movement{get; set;}
	public Vector2 MomentumPreservation{get; set;}
	public Vector2 BurstMomentumPreservation{get; set;}
	public bool MakeMomentumMatchDirection{get; set;}
	public bool MakeBurstMomentumMatchDirection{get; set;}
	public int Cooldown{get; set;}
	public float GravityMultiplier{get; set;}
	public string AttackAnimation{get; set;}
	public string AttackSound{get; set;}
	public float DriftForwardAcceleration{get; set;}
	public float DriftForwardSpeed{get; set;}
	public float DriftBackwardsAcceleration{get; set;}
	public float DriftBackwardsSpeed{get; set;}
	public bool SlowOnWalls{get; set;}
	public bool FastFallLocked{get; set;}
	
	public float DamageDoneMult{get; set;}
	public float KnockbackDoneMult{get; set;}
	public float StunDoneMult{get; set;}
	
	public bool FriendlyFire{get => OwnerObject.FriendlyFire; set => OwnerObject.FriendlyFire = value;}
	
	public List<string> EmittedProjectiles{get; set;}
	
	public bool HasHit{get; set;}
	
	public Attack OwnerAttack{get; set;}
	
	public AttackPartFramePropertyManager FramePropertyManager{get; set;} = new AttackPartFramePropertyManager();
	
	public AttackPartConditionManager TagConditionManager{get; set;} = new AttackPartConditionManager();
	public AttackPartConditionManager TransConditionManager{get; set;} = new AttackPartConditionManager();
	public AttackPartConditionManager StateConditionManager{get; set;} = new AttackPartConditionManager();
	
	public IAttacker OwnerObject{get; set;}
	
	public override void _Ready()
	{
		SetPhysicsProcess(false);
		FrameCount = 0;
		
		foreach(var h in Hitboxes) h.Connect("HitboxHit", this, nameof(HandleInitialHit));
		
		FramePropertyManager.Add(Length+Startup, long.MaxValue, "End");
		
		BuildHitboxAnimator();
		Init();
	}
	
	public void LoadExtraProperty<T>(string s, T @default = default(T))
	{
		var toAdd = new ParamRequest(typeof(T), s, @default);
		LoadExtraProperties.Add(s, toAdd);
	}
	
	public virtual void LoadProperties() {}
	
	public virtual void Init() {}
	
	protected virtual void Activate()
	{
		HasHit = false;
		FrameCount = 0;
		
		OwnerObject.HitboxAnimator.Connect("animation_finished", this, "ChangeToNextOnEnd");
		
		OwnerObject.CharacterSprite.Play(AttackAnimation, true);
		OwnerObject.Audio.Play(OwnerObject.AudioPrefix, AttackSound, Position);
		
		OwnerObject.Momentum *= MomentumPreservation;
		OwnerObject.Momentum += Movement * new Vector2(OwnerObject.Direction, 1);
		OwnerObject.BurstMomentum *= BurstMomentumPreservation;
		if(MakeMomentumMatchDirection) OwnerObject.Momentum = new Vector2(OwnerObject.Momentum.x.CopySign(OwnerObject.Direction), OwnerObject.Momentum.y);
		if(MakeBurstMomentumMatchDirection) OwnerObject.BurstMomentum = new Vector2(OwnerObject.BurstMomentum.x.CopySign(OwnerObject.Direction), OwnerObject.BurstMomentum.y);
		
		OnStart();
		
		HitList.Clear();
		HitIgnoreList.Clear();
		
		OwnerObject.HitboxAnimator.Play($"{Name}HitboxActivation");
	}
	
	public override void _PhysicsProcess(float delta)
	{
		if(FrameCount == Startup) OnStartupFinish();
		Loop();
		HandleHits();
		
		if(!(OwnerObject is Character sh && sh.States.Current is HitLagState))
		{
			CheckConditions();
			++FrameCount;
		}
	}
	
	public virtual void CheckConditions(bool end = false)
	{
		foreach(var result in TagConditionManager.Get(OwnerObject.Tags, FramePropertyManager, FrameCount))
			OwnerObject.Tags[result.Item1] = OwnerObject.Tags[result.Item1].Apply(result.Item2?StateTag.Starting:StateTag.Ending);
		
		var trans = TransConditionManager.Get(OwnerObject.Tags, FramePropertyManager, FrameCount).Where(c => c.Item2).Select(c => c.Item1).FirstOrDefault("");
		if(trans != "") ChangePart(trans);
		else
		{
			if(OwnerObject is Character sh)
			{
				var state = StateConditionManager.Get(OwnerObject.Tags, FramePropertyManager, FrameCount).Where(c => c.Item2).Select(c => c.Item1).FirstOrDefault("");
				if(state != "") sh.States.Change(state);
				else if(end) ChangePart("");
			}
			else if(end) ChangePart("");
		}
	}
	
	public virtual void OnStartupFinish()
	{
		if(OwnerObject is IProjectileEmitter pe) foreach(var p in EmittedProjectiles) pe.EmitProjectile(p);
	}
	
	const float FPS = 60f;
	public void BuildHitboxAnimator()
	{
		var anm = new Animation();
		anm.Length = (Startup+Length+1)/FPS;
		OwnerObject.HitboxAnimator.AddAnimation($"{Name}HitboxActivation", anm);
		foreach(var h in Hitboxes)
		{
			int trc = anm.AddTrack(Animation.TrackType.Value);
			var path = h.GetPath() + ":Active";
			anm.TrackSetPath(trc, path);
			
			foreach(var v in h.ActiveFrames)
			{
				anm.TrackInsertKey(trc, (Startup+v.x)/FPS, true);
				anm.TrackInsertKey(trc, (Startup+v.y)/FPS, false);
			}
		}
		
		#if DEBUG_HITBOX_PLAYER
		GD.Print("Animation hitbox player debug ahead");
		GD.Print(anm.GetTrackCount());
		for(int i = 0; i < anm.GetTrackCount(); ++i)
		{
			GD.Print(anm.TrackGetPath(i));
			GD.Print(anm.TrackGetKeyCount(i));
			for(int j = 0; j < anm.TrackGetKeyCount(i); ++j)
			{
				GD.Print(anm.TrackGetKeyTime(i, j) + " " + anm.TrackGetKeyValue(i, j));
			}
		}
		GD.Print("-------------------------------------------");
		#endif
	}
	
	protected virtual void Stop()
	{
		HandleHits();
		OnEnd();
		OwnerObject.HitboxAnimator.Stop(true);
		
		foreach(var h in Hitboxes) h.Active = false;
		
		OwnerObject.Tags["Hit"] = StateTag.Ending;
		HitList.Clear();
		HitIgnoreList.Clear();
		OwnerObject.HitboxAnimator.Disconnect("animation_finished", this, "ChangeToNextOnEnd");
	}
	
	public virtual void Pause() {OwnerObject.HitboxAnimator.Stop(); OwnerObject.CharacterSprite.Pause();}
	public virtual void Resume() {OwnerObject.HitboxAnimator.Play(); OwnerObject.CharacterSprite.Resume();}
	public virtual void Loop() {}
	public virtual void OnStart() {}
	public virtual void OnEnd() {}
	public virtual void HitEvent(Hitbox hitbox, Hurtbox hurtbox) {}
	
	public void ChangeToNextOnEnd(string dummy = "") => CheckConditions(true);
	public virtual void ChangePart(string part) => OwnerAttack.SetPart(part);
	
	public virtual bool CanGenerallyHit(IHittable hitObject) => OwnerObject.CanGenerallyHit(hitObject) && !HitIgnoreList.Contains(hitObject);
	public bool CanHit(IHittable hitObject) => CanGenerallyHit(hitObject)&&hitObject.CanGenerallyBeHitBy(this);
	
	public virtual void HandleInitialHit(Hitbox hitbox, Hurtbox hurtbox)
	{
		#if DEBUG_ATTACKS
		GD.Print($"{OwnerObject} attack part is signaled Handle Initial Hit");
		#endif
		
		if(!hitbox.Active) return;
		
		#if DEBUG_ATTACKS
		GD.Print($"{OwnerObject} attack part Has Hit = true");
		#endif
		
		HasHit = true;
		
		#if DEBUG_ATTACKS
		GD.Print($"{OwnerObject} Hitting = true");
		#endif
		
		OwnerObject.Hitting = true;
		OwnerObject.Tags["Hit"] = StateTag.Starting;
		
		#if DEBUG_ATTACKS
		GD.Print($"{OwnerObject} Last Hitee = {hurtbox.OwnerObject}");
		#endif
		
		OwnerObject.LastHitee = hurtbox.OwnerObject;
		
		#if DEBUG_ATTACKS
		GD.Print($"{hurtbox.OwnerObject} Getting Hit = true");
		GD.Print($"{hurtbox.OwnerObject} Last Hitter = {this.OwnerObject}");
		#endif
		
		hurtbox.OwnerObject.GettingHit = true;
		hurtbox.OwnerObject.LastHitter = this;
		
		var hitChar = hurtbox.OwnerObject;
		
		Hitbox current;
		if(HitList.TryGetValue(hurtbox, out current))
		{
			if(hitbox.HitPriority > current.HitPriority)
				HitList[hurtbox] = hitbox;
		}
		else
		{
			#if DEBUG_ATTACKS
			GD.Print($"{OwnerObject} attack part adds hitbox to Hit List");
			#endif
			
			HitList.Add(hurtbox, hitbox);
		}
	}
	
	public virtual void HandleHits()
	{
		#if DEBUG_ATTACKS
		GD.Print($"{OwnerObject} attack part runs Handle Hits");
		#endif
		
		if(!Active) return;
		var velocity = OwnerObject.Velocity;
		var copy = new Dictionary<Hurtbox, Hitbox>(HitList);//make copy to prevent this one crash
		foreach(var entry in copy)
		{
			#if DEBUG_ATTACKS
			GD.Print($"{OwnerObject} attack part iterates Hit List");
			#endif
			
			Hitbox hitbox = entry.Value;
			Hurtbox hurtbox = entry.Key;
			var hitChar = hurtbox.OwnerObject;
			
			#if DEBUG_ATTACKS
			GD.Print($"{OwnerObject} attack part runs Hit Event");
			#endif
			
			HitEvent(hitbox, hurtbox);
			
			var kmult = OwnerObject.KnockbackDoneMult*KnockbackDoneMult*OwnerAttack.KnockbackDoneMult*hitbox.GetKnockbackMultiplier(hitChar);
			var dmult = OwnerObject.DamageDoneMult*DamageDoneMult*OwnerAttack.DamageDoneMult*hitbox.GetDamageMultiplier(hitChar);
			var smult = OwnerObject.StunDoneMult*StunDoneMult*OwnerAttack.StunDoneMult*hitbox.GetStunMultiplier(hitChar);
			
			var dirvec = hitbox.KnockbackDir(hitChar)*kmult;
			var skb = dirvec*hitbox.SetKnockback + hitbox.MomentumCarry*velocity;
			var vkb = dirvec*hitbox.VarKnockback;
			var damage = hitbox.Damage*dmult;
			var sstun = hitbox.SetStun*smult;
			var vstun = hitbox.VarStun*smult;
 			
			var data = new HitData(skb, vkb, damage, sstun, vstun, hitbox.SetHitPause, hitbox.VarHitPause, hitbox.SetHitLag, hitbox.VarHitLag, hitbox, hurtbox);
			
			#if DEBUG_ATTACKS
			GD.Print($"{OwnerObject} attack part adds {hitChar} to Hit Ignore List");
			#endif
			
			HitIgnoreList.Add(hitChar);
			
			#if DEBUG_ATTACKS
			GD.Print($"{OwnerObject} attack parts calls attack's On Hit");
			#endif
			
			OwnerAttack.OnHit(hitbox, hurtbox);
			
			#if DEBUG_ATTACKS
			GD.Print($"{OwnerObject} attack part calls {OwnerObject} Handle Hitting");
			#endif
			
			OwnerObject.HandleHitting(data);
			
			#if DEBUG_ATTACKS
			GD.Print($"{OwnerObject} attack part calls {hitChar} Handle Getting Hit");
			#endif
			
			hitChar.HandleGettingHit(data);
		}
		
		#if DEBUG_ATTACKS
		GD.Print($"{OwnerObject} attack part clears Hit List");
		#endif
		
		HitList.Clear();
	}
	
	public virtual int FinalCooldown => Cooldown;
}
