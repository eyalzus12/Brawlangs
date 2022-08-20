using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using PartDir = System.Collections.Generic.Dictionary<string, AttackPart>;

public class AttackPart : Node2D, IHitter
{
	public PartDir dir = new PartDir();
	public int frameCount = 0;
	
	public List<Hitbox> Hitboxes{get; set;}
	public HashSet<IHittable> HitIgnoreList{get; set;} = new HashSet<IHittable>();
	public Dictionary<Hurtbox, Hitbox> HitList{get; set;} = new Dictionary<Hurtbox, Hitbox>();
	
	public Dictionary<string, ParamRequest> LoadExtraProperties = new Dictionary<string, ParamRequest>();
	
	public int Direction {get => ch.Direction; set => ch.Direction = value;}
	public int TeamNumber {get => ch.TeamNumber; set => ch.TeamNumber = value;}
	
	public bool active = false;
	public int Startup = 0;
	public int Endlag = 0;
	public int Length = 0;
	public Vector2 Movement{get; set;}
	public bool OverwriteXMovement{get; set;}
	public bool OverwriteYMovement{get; set;}
	public int MissEndlag{get; set;}
	public int Cooldown{get; set;}
	public int MissCooldown{get; set;}
	public float GravityMultiplier{get; set;}
	public float DamageMult{get; set;}
	public float KnockbackMult{get; set;}
	public float StunMult{get; set;}
	public string StartupAnimation{get; set;}
	public string AttackAnimation{get; set;}
	public string EndlagAnimation{get; set;}
	public string AttackSound{get; set;}
	public float DriftForwardAcceleration{get; set;}
	public float DriftForwardSpeed{get; set;}
	public float DriftBackwardsAcceleration{get; set;}
	public float DriftBackwardsSpeed{get; set;}
	public bool SlowOnWalls{get; set;}
	
	public List<string> EmittedProjectiles{get; set;}
	
	public bool HasHit{get; set;}
	
	public AnimationPlayer hitboxPlayer;
	public Attack att;
	
	//needed for: states, animations
	protected Character ch;
	public IAttacker OwnerObject{get => ch; set
		{
			if(value is Character c) ch = c;
		}
	}
	
	public override void _Ready()
	{
		frameCount = 0;
		if(Startup == 0) OnStartupFinish();
		
		Hitboxes = GetChildren().FilterType<Hitbox>().ToList();
		ConnectSignals();
		BuildHitboxAnimator();
		Init();
	}
	
	public void ConnectSignals()
	{
		Hitboxes.ForEach(h => h.Connect("HitboxHit", this, nameof(HandleInitialHit)));
	}
	
	public void LoadExtraProperty<T>(string s, T @default = default(T))
	{
		var toAdd = new ParamRequest(typeof(T), s, @default);
		LoadExtraProperties.Add(s, toAdd);
	}
	
	public virtual void LoadProperties() {}
	
	public void Connect(AttackPart ap)
	{
		dir.Add(ap.Name, ap);
	}
	
	public void Connect(string name, AttackPart ap)
	{
		dir.Add(name, ap);
	}
	
	public virtual void Init() {}
	
	public virtual void Activate()
	{
		HasHit = false;
		
		ch.PlayAnimation(StartupAnimation);
		ch.PlaySound(AttackSound);
		active = true;
		frameCount = 0;
		
		if(OverwriteXMovement) ch.vec.x = 0;
		if(OverwriteYMovement) ch.vec.y = 0;
		
		if(Movement != Vector2.Zero)
			ch.vec = Movement * new Vector2(ch.Direction, 1);
		
		OnStart();
		hitboxPlayer.Play("HitboxActivation");
		
		HitList.Clear();
		HitIgnoreList.Clear();
	}
	
	public override void _PhysicsProcess(float delta)
	{
		if(!active) return;
		++frameCount;
		if(frameCount == Startup) OnStartupFinish();
		else if(frameCount > Startup) ch.PlayAnimation(AttackAnimation);
		Loop();
		HandleHits();
	}
	
	public virtual void OnStartupFinish()
	{
		EmittedProjectiles?.ForEach(s=>ch.EmitProjectile(s));
	}
	
	const float FPS = 60f;
	public void BuildHitboxAnimator()
	{
		hitboxPlayer = new AnimationPlayer();
		hitboxPlayer.PlaybackProcessMode = AnimationPlayer.AnimationProcessMode.Physics;
		hitboxPlayer.Name = "AttackPlayer";
		AddChild(hitboxPlayer);
		var anm = new Animation();
		anm.Length = (Startup+Length/*+endlag*/)/FPS;
		hitboxPlayer.AddAnimation("HitboxActivation", anm);
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
		
		hitboxPlayer.Connect("animation_finished", this, "ChangeToNext");
	}
	
	public virtual void Stop()
	{
		HandleHits();
		hitboxPlayer.Stop(true);
		Hitboxes.ForEach(h => h.Active = false);
		OnEnd();
		HitList.Clear();
		HitIgnoreList.Clear();
		active = false;
	}
	
	public virtual void Pause() {hitboxPlayer.Stop();}
	public virtual void Resume() {hitboxPlayer.Play();}
	public virtual void Loop() {}
	public virtual void OnStart() {}
	public virtual void OnEnd() {}
	public virtual void HitEvent(Hitbox hitbox, Hurtbox hurtbox) {}
	
	public virtual void ChangeToNext(string dummy="")
	{
		if(!active) return;
		ChangePart(GetNextPart());
	}
	
	private IEnumerable<string> Describe(Character c)
	{
		if(c.ceilinged) yield return "Ceiling";
		if(c.walled) yield return "Wall";
		yield return c.grounded?"Grounded":"Aerial";
		yield return "";
	}
	
	public virtual string GetNextPart()
	{
		foreach(var property in Describe(ch))
		{
			if(HasHit && dir.ContainsKey($"{property}Hit")) return $"{property}Hit";
			if(dir.ContainsKey($"{property}Miss")) return $"{property}Miss";
			if(dir.ContainsKey(property)) return property;
		}
		
		return "Next";
	}
	
	public virtual void ChangePart(string part)
	{
		if(!active || part == "") return;
		var changeTo = GetConnectedPart(part);
		att.SetPart(changeTo);
	}
	
	public AttackPart GetConnectedPart(string name)
	{
		if(dir.ContainsKey(name)) return dir[name];
		else return null;
	}
	
	public bool CanHit(IHittable h) => ch.CanHit(h) && !HitIgnoreList.Contains(h);
	
	public virtual void HandleInitialHit(Hitbox hitbox, Hurtbox hurtbox)
	{
		//GD.Print($"{OwnerObject} attack part is signaled Handle Initial Hit");
		if(!hitbox.Active) return;
		
		//GD.Print($"{OwnerObject} attack part Has Hit = true");
		HasHit = true;
		
		//GD.Print($"{OwnerObject} Hitting = true");
		ch.Hitting = true;
		//GD.Print($"{OwnerObject} Last Hitee = {hurtbox.OwnerObject}");
		ch.LastHitee = hurtbox.OwnerObject;
		
		//GD.Print($"{hurtbox.OwnerObject} Getting Hit = true");
		hurtbox.OwnerObject.GettingHit = true;
		//GD.Print($"{hurtbox.OwnerObject} Last Hitter = {this.OwnerObject}");
		hurtbox.OwnerObject.LastHitter = this;
		
		var hitChar = hurtbox.OwnerObject;
		
		var current = new Hitbox();
		if(HitList.TryGetValue(hurtbox, out current))
		{
			if(hitbox.HitPriority > current.HitPriority)
				HitList[hurtbox] = hitbox;
		}
		else
		{
			//GD.Print($"{OwnerObject} attack part adds hitbox to Hit List");
			HitList.Add(hurtbox, hitbox);
		}
	}
	
	public virtual void HandleHits()
	{
		//GD.Print($"{OwnerObject} attack part runs Handle Hits");
		if(!active) return;
		var velocity = ch.Velocity;
		foreach(var entry in HitList)
		{
			//GD.Print($"{OwnerObject} attack part iterates Hit List");
			Hitbox hitbox = entry.Value;
			Hurtbox hurtbox = entry.Key;
			var hitChar = hurtbox.OwnerObject;
			
			//GD.Print($"{OwnerObject} attack part runs Hit Event");
			HitEvent(hitbox, hurtbox);
			
			var kmult = ch.KnockbackDoneMult*KnockbackMult*att.KnockbackMult*hitbox.GetKnockbackMultiplier(hitChar);
			var dmult = ch.DamageDoneMult*DamageMult*att.DamageMult*hitbox.GetDamageMultiplier(hitChar);
			var smult = ch.StunDoneMult*StunMult*att.StunMult*hitbox.GetStunMultiplier(hitChar);
			
			var dirvec = hitbox.KnockbackDir(hitChar)*kmult;
			var skb = dirvec*hitbox.SetKnockback + hitbox.MomentumCarry*velocity;
			var vkb = dirvec*hitbox.VarKnockback;
			var damage = hitbox.Damage*dmult;
			var stun = hitbox.Stun*smult;
 			
			var data = new HitData(skb, vkb, damage, stun, hitbox.Hitpause, hitbox, hurtbox);
			
			//GD.Print($"{OwnerObject} attack part adds {hitChar} to Hit Ignore List");
			HitIgnoreList.Add(hitChar);
			//GD.Print($"{OwnerObject} attack parts calls attack's On Hit");
			att.OnHit(hitbox, hurtbox);
			//GD.Print($"{OwnerObject} attack part calls {OwnerObject} Handle Hitting");
			ch.HandleHitting(data);
			//GD.Print($"{OwnerObject} attack part calls {hitChar} Handle Getting Hit");
			hitChar.HandleGettingHit(data);
		}
		
		//GD.Print($"{OwnerObject} attack part clears Hit List");
		HitList.Clear();
	}
	
	public virtual int GetEndlag() => Endlag + (HasHit?0:MissEndlag);
	public virtual int GetCooldown() => Cooldown + (HasHit?0:MissCooldown);
}
