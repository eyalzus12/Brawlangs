using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Projectile : Node2D, IHitter, IHittable
{
	[Signal]
	public delegate void ProjectileDied(Projectile who);
	
	public string Identifier{get; set;}
	public Vector2 SpawningPosition{get; set;}
	public int MaxLifetime{get; set;}
	
	public int Direction{get; set;}
	
	public ProjectileMovementFunction Movement{get; set;}
	public long FrameCount{get; set;} = 0;
	
	public List<Hitbox> Hitboxes{get; set;} = new List<Hitbox>();
	
	public Dictionary<Hurtbox, Hitbox> HitList{get; set;} = new Dictionary<Hurtbox, Hitbox>();
	public HashSet<IHittable> HitIgnoreList{get; set;} = new HashSet<IHittable>();
	
	public bool HasHit{get; set;}
	public bool GettingHit{get; set;}
	public IHitter LastHitter{get; set;}
	
	public IAttacker OwnerObject{get; set;}
	
	public string CurrentCollisionSetting{get; set;}
	
	public InvincibilityManager IFrames{get; set;} = new InvincibilityManager();
	public bool Invincible => IFrames.Count > 0;
	
	public List<Hurtbox> Hurtboxes{get; set;} = new List<Hurtbox>();
	
	public int TeamNumber{get => OwnerObject.TeamNumber; set => OwnerObject.TeamNumber = value;}
	public bool FriendlyFire{get => OwnerObject.FriendlyFire; set => OwnerObject.FriendlyFire = value;}

	public float DamageDoneMult{get => OwnerObject.DamageDoneMult; set => OwnerObject.DamageDoneMult = value;}
	public float KnockbackDoneMult{get => OwnerObject.KnockbackDoneMult; set => OwnerObject.KnockbackDoneMult = value;}
	public float StunDoneMult{get => OwnerObject.StunDoneMult; set => OwnerObject.StunDoneMult = value;}
	
	public string AudioPrefix => OwnerObject.AudioPrefix;
	public AudioManager Audio{get; set;}
	
	public StateTagsManager Tags{get; set;} = new StateTagsManager();
	
	public Dictionary<string, ParamRequest> LoadExtraProperties{get; set;} = new Dictionary<string, ParamRequest>();
	public virtual void LoadProperties() {}
	public void LoadExtraProperty<T>(string s, T @default = default(T))
	{
		var toAdd = new ParamRequest(typeof(T), s, @default);
		LoadExtraProperties.Add(s, toAdd);
	}
	
	private bool _active = false;
	public bool Active
	{
		get => _active;
		set
		{
			_active = value;
			Visible = value;
			if(value) OnSpawn();
		}
	}
	
	public Projectile() {}
	public Projectile(IAttacker owner)
	{
		OwnerObject = owner;
		Audio = OwnerObject.Audio;
	}
	
	public override void _Ready()
	{
		FrameCount = 0;
		Position = SpawningPosition + OwnerObject.Position;
		
		foreach(var h in Hitboxes)
		{
			h.Active = true;
			h.Connect("HitboxHit", this, nameof(HandleInitialHit));
		}
		
		HasHit = false;
		GettingHit = false;
		Init();
		Active = true;
		
		ApplySettings("Default");
	}
	
	public override void _PhysicsProcess(float delta)
	{
		if(!Active) return;
		GettingHit = false;
		++FrameCount;
		HandleHits();
		Loop();
		Position = Movement.GetNext(this);
		
		if(FrameCount >= MaxLifetime)
			Destruct();
	}
	
	public virtual void Init() {}
	public virtual void OnSpawn() {}
	public virtual void Loop() {}
	public virtual void OnRemove() {}
	
	public virtual void UpdateTags()
	{
		
	}
	
	public void Destruct()
	{
		OnRemove();
		GettingHit = false;
		
		foreach(var h in Hitboxes)
		{
			h.Active = false;
			h.Disconnect("HitboxHit", this, nameof(HandleInitialHit));
		}
		
		Active = false;
		HitList.Clear();
		HitIgnoreList.Clear();
		EmitSignal(nameof(ProjectileDied), this);
	}
	
	public void HandleInitialHit(Hitbox hitbox, Hurtbox hurtbox)
	{
		if(!hitbox.Active) return;
		var hitChar = hurtbox.OwnerObject;
		if(!CanHit(hitChar) || !hitbox.CanHit(hitChar) || !hurtbox.CanBeHitBy(this)) return;
		
		Hitbox current;
		if(HitList.TryGetValue(hurtbox, out current))
		{
			if(hitbox.HitPriority > current.HitPriority)
				HitList[hurtbox] = hitbox;
		}
		else
		{
			HitList.Add(hurtbox, hitbox);
		}
	}
	
	public virtual void HandleHits()
	{
		foreach(var entry in HitList)
		{
			Hitbox hitbox = entry.Value;
			Hurtbox hurtbox = entry.Key;
			var hitChar = hurtbox.OwnerObject;
			if(!CanHit(hitChar)) continue;//already hit or cant hit
			HitEvent(hitbox, hurtbox);
			
			var kmult = OwnerObject.KnockbackDoneMult*KnockbackDoneMult*hitbox.GetKnockbackMultiplier(hitChar);
			var dmult = OwnerObject.DamageDoneMult*DamageDoneMult*hitbox.GetDamageMultiplier(hitChar);
			var smult = OwnerObject.StunDoneMult*StunDoneMult*hitbox.GetStunMultiplier(hitChar);
			
			var dirvec = hitbox.KnockbackDir(hitChar)*kmult;
			var skb = dirvec*hitbox.SetKnockback;
			var vkb = dirvec*hitbox.VarKnockback;
			var damage = hitbox.Damage*dmult;
			var sstun = hitbox.SetStun*smult;
			var vstun = hitbox.VarStun*smult;
 			
			var data = new HitData(skb, vkb, damage, sstun, vstun, hitbox.SetHitPause, hitbox.VarHitPause, hitbox.SetHitLag, hitbox.VarHitLag, hitbox, hurtbox);
			
			hitChar.HandleGettingHit(data);
			OwnerObject.HandleHitting(data);
			HitIgnoreList.Add(hitChar);
			GD.Print($"{(hitChar as Node2D).Name} was hit by {hitbox.Name}");
		}
		if(HitList.Count > 0) DidHit();
		HitList.Clear();
	}
	
	public void ApplySettings(string setting)
	{
		CurrentCollisionSetting = setting;
		foreach(var h in Hurtboxes) h.ChangeState(setting);
	}
	
	public virtual void HitEvent(Hitbox hitbox, Hurtbox hurtbox) {}
	public virtual void DidHit() => Destruct();
	
	public virtual void HandleGettingHit(HitData data)
	{
		Audio.Play(data.Hitter.HitSound, Position);
		Destruct();
	}
	
	public virtual bool CanGenerallyHit(IHittable hitObject) => OwnerObject.CanHit(hitObject) && !HitIgnoreList.Contains(hitObject);
	public virtual bool CanGenerallyBeHitBy(IHitter hitter) => true;
	public virtual bool CanGenerallyBeHitBy(IAttacker attacker) => true;
	
	public bool CanHit(IHittable hitObject) => CanGenerallyHit(hitObject)&&hitObject.CanGenerallyBeHitBy(this);
	public bool CanBeHitBy(IHitter hitter) => CanGenerallyBeHitBy(hitter)&&hitter.CanGenerallyHit(this);
	public bool CanBeHitBy(IAttacker attacker) => CanGenerallyBeHitBy(attacker)&&attacker.CanGenerallyHit(this);
}
